using Godot;
using System;

/// <summary>
/// Provides easy access to scenes that we want to instantiate at runtime
/// </summary>
public partial class PackedScenes : Node
{
    public static PackedScenes Instance { get; private set; }
    
    [Export] public PackedScene BeamSource { get; set; }
    [Export] public PackedScene LaserBeam { get; set; }
    [Export] public PackedScene PassiveAbsorber { get; set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
