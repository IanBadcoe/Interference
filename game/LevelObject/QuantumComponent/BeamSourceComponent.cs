using System.Numerics;
using Interference.Code.QuantumMaths.Components;

namespace Interference.LevelObject.QuantumComponent;

public partial class BeamSourceComponent : QuantumComponent
{
    public override void _Ready()
    {
        Component = new BeamSource(Quantum.FromGodotAngleDegrees(GetGlobalRotationDegrees()), Complex.One);
        
        base._Ready();
    }
}