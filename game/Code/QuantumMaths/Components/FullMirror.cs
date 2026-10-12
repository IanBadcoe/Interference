using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths.Components;

public class FullMirror(Directions.Direction direction, Directions.MirrorType type) : Component
{
    public Directions.Direction Direction { get; private init; } = direction;
    public Directions.MirrorType Type { get; private init; } = type;

    Matrix4x4 Matrix => Directions.BuildMirrorMatrix(Direction, Type);

    public override void Calculate()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}