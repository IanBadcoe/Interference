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
    private readonly Dictionary<Directions.Direction, (LaserBeam beam, QuantumComponent? component)> _connections = [];

    private readonly List<PassiveAbsorber> _syntheticAbsorbers = [];
    
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

    private void ConnectInternal(Directions.Direction direction, LaserBeam beam, QuantumComponent? otherComponent)
    {
        if (_connections.TryGetValue(direction, out var tuple))
        {
            tuple.beam.QueueFree();
        }

        _connections[direction] = (beam, otherComponent);
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
        var level = (Level)GetTree().CurrentScene;

        foreach (var syntheticAbsorber in _syntheticAbsorbers)
        {
            level.QuantumComponents.Remove(syntheticAbsorber);
        }

        level.QuantumComponents.Remove(Component);

        foreach (var orthogonalDirection in Directions.OrthogonalDirections)
        {
            if (_connections.TryGetValue(orthogonalDirection, out var tuple))
            {
                tuple.beam.QueueFree();
                _connections.Remove(orthogonalDirection);
                tuple.component?._connections.Remove(Directions.Reverse(orthogonalDirection));
            }
        
            Component.Disconnect(orthogonalDirection);
        }
        
        level.MarkQuantumStateDirty();
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
            if (otherComponent == null)
            {
                // Simulation library doesn't simulate a beam output that doesn't go to another component, so if the
                // beam is hitting a wall or some other non-component blocker we create a PassiveAbsorber to be the other end
                // This absorber doesn't need a godot scene, it's managed entirely by this component and is invisible to
                // the outside world
                var absorber = new PassiveAbsorber();
                var (beam1, beam2) = Component.Connect(absorber, direction,
                    (GetGlobalPosition() - result.position).Length());
                
                var laser = LaserBeam.Create(GetGlobalPosition(), result.position, beam1, beam2);
                GetTree().CurrentScene.AddChild(laser);

                ConnectInternal(direction, laser, null);
                
                _syntheticAbsorbers.Add(absorber);
            }
            else
            {
                var (beam1, beam2) = Component.Connect(otherComponent.Component, direction,
                    (GetGlobalPosition() - otherComponent.GetGlobalPosition()).Length());

                var laser = LaserBeam.Create(GetGlobalPosition(), otherComponent.GetGlobalPosition(), beam1, beam2);
                GetTree().CurrentScene.AddChild(laser);

                ConnectInternal(direction, laser, otherComponent);
                otherComponent.ConnectInternal(Directions.Reverse(direction), laser, this);
            }
        }

        var level = (Level)GetTree().CurrentScene;
        level.QuantumComponents.Add(Component);
        foreach (var syntheticAbsorber in _syntheticAbsorbers)
        {
            level.QuantumComponents.Add(syntheticAbsorber);
        }
    }
}