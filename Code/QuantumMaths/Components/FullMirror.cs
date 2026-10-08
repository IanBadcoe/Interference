using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths.Components;

class FullMirror(MirrorDirections.Direction direction, MirrorDirections.MirrorType type) : Component
{
    public MirrorDirections.Direction Direction { get; private init; } = direction;
    public MirrorDirections.MirrorType Type { get; private init; } = type;

    Matrix4x4 Matrix => MirrorDirections.BuildMirrorMatrix(Direction, Type);

    public override void Calculate()
    {
        State state = GatherInputs();

        State outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}