using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot_Util.Ext;
using Godot;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace Interference.LevelObject.QuantumComponent;

/// <summary>
/// Wraps a component from the quantum simulation. Handles configuring the component's connections and reporting back simulation results.
/// </summary>
public abstract partial class QuantumComponent : Node2D
{
    [Export]
    private PhysicsBody2D _objectRoot;
    
    protected Component Component;

#nullable enable
    private readonly Dictionary<Directions.Direction, (LaserBeam beam, QuantumComponent? component, PassiveAbsorber? syntheticAbsorber)> _connections = [];
    
    public override void _Ready()
    {
        // Subclass should create the component before calling us
        Debug.Assert(Component != null);

        GetTree().CreateTimer(0.1, processInPhysics: true).Timeout += TraceForConnections;

        var grabbable = this.Ancestors().OfType<Grabbable>().FirstOrDefault();
        if (grabbable != null)
        {
            grabbable.Grabbed += OnGrabbed;
            grabbable.PutDown += OnPutDown;
        }
    }

    private void OnGrabbed()
    {
        GetTree().CreateTimer(0.1, processInPhysics: true).Timeout += () =>
        {
            // Grabbable script will have removed our collision, so connected components won't just hit us again
            ConnectedComponentsRetrace();
            DestroyConnectionState();
        };
    }
    
    private void OnPutDown()
    {
        Component.Direction = Quantum.FromGodotAngleDegrees(GetGlobalRotationDegrees());
        GetTree().CreateTimer(0.1, processInPhysics: true).Timeout += TraceForConnections;
    }

    public override void _ExitTree()
    {
        GetTree().CreateTimer(0.1, processInPhysics: true).Timeout += () =>
        {
            // Grabbable script will have removed our collision, so connected components won't just hit us again
            ConnectedComponentsRetrace();
            DestroyConnectionState();
        };
    }

    /// <summary>
    /// Sets up the connection to the given other component or to a new dummy PassiveAbsorber if otherComponent is null.
    /// </summary>
    private void ConnectInternal(Directions.Direction direction, Vector2 endPosition, QuantumComponent? otherComponent)
    {
        DisconnectInternal(direction);
        otherComponent?.DisconnectInternal(Directions.Reverse(direction));
        
        var level = (Level)GetTree().CurrentScene;
        
        // Simulation library doesn't simulate a beam output that doesn't go to another component, so if the
        // beam is hitting a wall or some other non-component blocker we create a PassiveAbsorber to be the other end
        // This absorber doesn't need a godot scene, it's managed entirely by this component and is invisible to
        // the outside world
        
        var otherInnerComponent = otherComponent?.Component;
        PassiveAbsorber? absorber = null;
        if (otherInnerComponent == null)
        {
            absorber = new PassiveAbsorber();
            level.QuantumComponents.Add(absorber);
        }

        var (beam1, beam2) = Component.Connect(otherInnerComponent ?? absorber, direction,
            (GetGlobalPosition() - endPosition).Length());
                
        var laser = LaserBeam.Create(GetGlobalPosition(), endPosition, beam1, beam2);
        GetTree().CurrentScene.AddChild(laser);

        _connections[direction] = (laser, otherComponent, absorber);
        if (otherComponent != null)
        {
            otherComponent._connections[Directions.Reverse(direction)] = (laser, this, null);
        }
        
        level.MarkQuantumStateDirty();
    }

    private bool DisconnectInternal(Directions.Direction direction)
    {
        if (!_connections.TryGetValue(direction, out var tuple))
        {
            return false;
        }

        tuple.beam.QueueFree();
        _connections.Remove(direction);
        tuple.component?._connections.Remove(Directions.Reverse(direction));

        var level = (Level)GetTree().CurrentScene;
            
        if (tuple.syntheticAbsorber != null)
        {
            level.QuantumComponents.Remove(tuple.syntheticAbsorber);
        }
            
        level.MarkQuantumStateDirty();

        return true;

    }

    private void ConnectedComponentsRetrace()
    {
        // Need to copy collection as other components retracing can modify it 
        foreach (var component in _connections.Select(x => x.Value.component).ToList())
        {
            component?.TraceForConnections();
        }
    }

    private void DestroyConnectionState()
    {
        foreach (var orthogonalDirection in Directions.OrthogonalDirections)
        {
            DisconnectInternal(orthogonalDirection);
        }
    }

    /// <summary>
    /// Discards any existing connection data, traces orthogonally for components that should connect to this, sets up
    /// the simulation state and creates LaserBeam objects to represent each connection found.
    /// Creates synthetic PassiveAbsorber components to connect to for any beam that would hit a blocker
    /// </summary>
    protected virtual void TraceForConnections()
    {
        DestroyConnectionState();
        
        var spaceState = GetWorld2D().DirectSpaceState;
        for (var angle = 0; angle < 360; angle += 90)
        {
            var parameters = new PhysicsRayQueryParameters2D();
            parameters.From = GetGlobalPosition();
            parameters.To = GetGlobalPosition() + Vector2.FromAngle(Mathf.DegToRad(angle)) * Globals.MaxBeamDistance;
            parameters.CollisionMask = (uint)CollisionLayers.BeamObstruction;
            parameters.Exclude = [_objectRoot.GetRid()];
            parameters.CollideWithAreas = true;
            var result = Globals.ParseCollisionResult(spaceState.IntersectRay(parameters));

            var otherComponent = result.collider?.Children<QuantumComponent>(true).FirstOrDefault();
            var direction = Quantum.FromGodotAngleDegrees(angle);

            ConnectInternal(direction, otherComponent?.GetGlobalPosition() ?? result.position, otherComponent);
        }

        var level = (Level)GetTree().CurrentScene;
        level.QuantumComponents.Add(Component);
    }
}