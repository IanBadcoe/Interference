using System.Numerics;

namespace Interference.Code.QuantumMaths.Components;

class FullMirror(MirrorDirections.Direction direction) : Component
{
    public MirrorDirections.Direction Direction { get; private init; } = direction;

    public override void Calculate()
    {
        
    }
}