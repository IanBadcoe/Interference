namespace Interference.Code.QuantumMaths.Components;

public class BeamSplitter(Directions.Direction direction) : Component
{
    public Directions.Direction Direction { get; private init; } = direction;

    Matrix4x4 Matrix => Directions.BuildSplitterMatrix(Direction);

    protected override void CalculateInner()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}