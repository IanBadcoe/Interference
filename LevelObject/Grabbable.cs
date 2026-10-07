using Godot;
using System;
using Interference;

public partial class Grabbable : StaticBody2D
{
	private uint _originalCollisionLayers = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_originalCollisionLayers = CollisionLayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnGrabbed()
	{
		CollisionLayer = 0;
	}
	
	public void OnPutDown()
	{
		CollisionLayer = _originalCollisionLayers;
	}
}
