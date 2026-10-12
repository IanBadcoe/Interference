using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Interference.Player;

public partial class PlayerCharacter : CharacterBody2D
{
    [Export]
    private Camera2D Camera { get; set; }

    /// <summary>
    /// Distance away from the character that we probe for usable objects
    /// i.e. the max distance we can use from
    /// </summary>
    [Export]
    private float UseTargetProbeDistance { get; set; } = 16.0f;

    /// <summary>
    /// Distance away from the character that a held object is positioned
    /// </summary>
    [Export]
    private float HoldDistance { get; set; } = 8.0f;
    
    /// <summary>
    /// Distance in front of the character that we attempt to place down held objects
    /// </summary>
    [Export]
    private float PlacementDistance { get; set; } = 20.0f;

    [Export]
    private Node2D HoldSlot { get; set; }

    [Export]
    private Sprite2D PlacementPreview { get; set; }

    [Export]
    public int MovementSpeed { get; set; } = 100;

    public float LastMovementDirection { get; private set; } = 0;

    public Grabbable Held { get; private set; }

    public bool IsHoldingObject => Held != null;

    private bool _attemptUse = false;
    private bool _canPutDown = false;

    private void ProcessInput()
    {
        var inputDirection = Input.GetVector(PlayerInputActions.Left,
            PlayerInputActions.Right,
            PlayerInputActions.Up,
            PlayerInputActions.Down);
        Velocity = inputDirection * MovementSpeed;

        if (!inputDirection.IsZeroApprox())
        {
            LastMovementDirection = inputDirection.Angle();
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Debug.Assert(Camera != null);

        Camera.MakeCurrent();
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed(PlayerInputActions.Use))
        {
            _attemptUse = true;
        }
    }

    public override void _Process(double delta)
    {
        PlacementPreview.Visible = IsHoldingObject;

        if (Held != null)
        {
            var position = GetGlobalPosition() + Vector2.FromAngle(LastMovementDirection) * PlacementDistance;

            var cellCentre = new Vector2((float)Math.Round((position.X / Globals.CellSize)) * Globals.CellSize,
                (float)Math.Round(position.Y / Globals.CellSize) * Globals.CellSize);

            PlacementPreview.SetGlobalPosition(cellCentre);
            
            PlacementPreview.SelfModulate = _canPutDown ? Colors.Black : Colors.Red;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessInput();
        MoveAndSlide();

        HoldSlot.SetPosition(Vector2.FromAngle(LastMovementDirection) * HoldDistance);

        var spaceState = GetWorld2D().DirectSpaceState;

        if (IsHoldingObject)
        {
            var parameters = new PhysicsShapeQueryParameters2D();
            parameters.Shape = Held.PlacementTestShape.Shape;
            parameters.Transform = new Transform2D(0, Vector2.Zero, 0, PlacementPreview.GetGlobalPosition());
            parameters.CollisionMask = (uint)CollisionLayers.PlayerMovement;
            _canPutDown = spaceState.IntersectShape(parameters, 1).Count == 0;
        }

        if (_attemptUse)
        {
            if (IsHoldingObject)
            {
                if (_canPutDown)
                {
                    Held.Reparent(GetTree().CurrentScene);
                    Held.OnPutDown();
                    Held.SetGlobalPosition(PlacementPreview.GetGlobalPosition());

                    Held = null;
                }
            }
            else
            {
                var query = PhysicsRayQueryParameters2D.Create(Transform.Origin,
                    Transform.Origin + Vector2.FromAngle(LastMovementDirection) * UseTargetProbeDistance,
                    (uint)CollisionLayers.Usable);

                var result = spaceState.IntersectRay(query);

                var grabbable = result.GetValueOrDefault("collider").As<Grabbable>();

                if (grabbable != null)
                {
                    Held = grabbable;
                    Held.Reparent(HoldSlot, false);
                    Held.SetPosition(Vector2.Zero);

                    grabbable.OnGrabbed();
                }
            }

            _attemptUse = false;
        }
    }
}