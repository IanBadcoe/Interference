using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400;

	public void GetInput()
	{
		var inputDirection = Input.GetVector("left", "right", "up", "down");
		Velocity = inputDirection * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
	}
}
