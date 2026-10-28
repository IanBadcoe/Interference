namespace Interference.Code.QuantumMaths.Components;

public class FullMirror(Directions.Direction direction, Calculator.MirrorType type, string name = "anon mirror")
    : Component(direction, name)
{
    public Calculator.MirrorType Type { get; private init; } = type;

    Matrix4x4 Matrix => Calculator.BuildMirrorMatrix(Direction, Type);

    protected override void CalculateInner()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}