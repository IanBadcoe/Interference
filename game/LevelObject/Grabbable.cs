using Godot;

public partial class Grabbable : StaticBody2D
{
	[Signal]
	public delegate void GrabbedEventHandler();
	
	[Signal]
	public delegate void PutDownEventHandler();
	
	[Export]
	public CollisionShape2D PlacementTestShape { get; private set; }
	
	[Export]
	public Sprite2D Sprite { get; private set; }

	[Export]
	public int RotationIncrementDegrees { get; set; } = 45; // TODO: Rotate controls
	
	private uint _originalCollisionLayers = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_originalCollisionLayers = CollisionLayer;
	}

	public void OnGrabbed()
	{
		CollisionLayer = 0;
		Sprite.SelfModulate = new Color(Colors.White, 0.5f);
		EmitSignalGrabbed();
	}
	
	public void OnPutDown()
	{
		CollisionLayer = _originalCollisionLayers;
		Sprite.SelfModulate = Colors.White;
		EmitSignalPutDown();
	}
}
