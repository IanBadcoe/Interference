using Godot;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace Interference.LevelObject.QuantumComponent;

public partial class MirrorComponent : QuantumComponent
{
    [Export]
    private Directions.MirrorType MirrorType { get; set; }
    
    public override void _Ready()
    {
        Component = new FullMirror(Quantum.FromGodotAngleDegrees(GetGlobalRotation()), MirrorType);
        base._Ready();
    }
}