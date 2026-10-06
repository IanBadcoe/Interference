using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	[Export]
	public int MovementSpeed { get; set; } = 150;

	public void GetInput()
	{
		var inputDirection = Input.GetVector("left", "right", "up", "down");
		Velocity = inputDirection * MovementSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
	}
}
