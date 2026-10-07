using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Interference.Player;

public partial class PlayerMovement : CharacterBody2D
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
	private float HoldDistance { get; set; } = 16.0f;
	
	[Export]
	private Node2D HoldSlot { get; set; }
	
	[Export]
	public int MovementSpeed { get; set; } = 150;

	public float LastMovementDirection { get; private set; } = 0;
	
	public Grabbable Held { get; private set; }

	private bool _attemptUse = false;

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
			GD.Print("Use");
			
			_attemptUse = true;
		}
	}
	
	public void GetInput()
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

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
		
		HoldSlot.SetPosition(Vector2.FromAngle(LastMovementDirection) * HoldDistance);
		
		if (_attemptUse)
		{
			if (Held == null)
			{
				var spaceState = GetWorld2D().DirectSpaceState;
				var query = PhysicsRayQueryParameters2D.Create(Transform.Origin,
					Transform.Origin + Vector2.FromAngle(LastMovementDirection) * UseTargetProbeDistance,
					(uint)Interference.CollisionLayer.Usable);

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
			else
			{
				//TODO: Put down
			}

			_attemptUse = false;
		}
	}
}