namespace Interference.Code.QuantumMaths.Components;

public class FullMirror(Directions.Direction direction, Directions.MirrorType type) : Component(direction)
{
    public Directions.MirrorType Type { get; private init; } = type;

    Matrix4x4 Matrix => Directions.BuildMirrorMatrix(Direction, Type);

    protected override void CalculateInner()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}