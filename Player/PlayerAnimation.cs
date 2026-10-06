using Godot;
using System;
using System.Diagnostics;

public partial class PlayerAnimation : AnimatedSprite2D
{
	[Export]
	public PlayerMovement PlayerMovement { get; set; }

	[Export]
	public float IdleAnimDelay { get; set; } = 5;

	private Timer _idleAnimTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(PlayerMovement != null);

		_idleAnimTimer = new Timer();
		_idleAnimTimer.WaitTime = IdleAnimDelay;
		AddChild(_idleAnimTimer);

		_idleAnimTimer.Timeout += () =>
		{
			Animation = "idle";
			Play();
		};

		AnimationFinished += () =>
		{
			if (Animation == "idle")
			{
				Stop();
				_idleAnimTimer.Start();
			}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (PlayerMovement.Velocity.Length() > 0)
		{
			Animation = "run";
			Play(customSpeed: 2);

			FlipH = PlayerMovement.Velocity.X < 0;

			_idleAnimTimer.Stop();
		}
		else
		{
			Animation = "idle";

			if (_idleAnimTimer.IsStopped())
			{
				Stop();
				_idleAnimTimer.Start();
			}
		}
		
	}
}
