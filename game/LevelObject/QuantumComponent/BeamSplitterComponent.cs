using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace Interference.LevelObject.QuantumComponent;

public partial class BeamSplitterComponent : QuantumComponent
{
    public override void _Ready()
    {
        Component = new BeamSplitter(Quantum.FromGodotAngleDegrees(GetGlobalRotation()),
            Calculator.BeamSplitterType.HalfSilveredMirror);
        
        base._Ready();
    }
}