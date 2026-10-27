namespace Interference.Code.QuantumMaths.Components;

public class BeamSplitter(Directions.Direction direction, Calculator.BeamSplitterType type) : Component(direction)
{
    public Calculator.BeamSplitterType Type { get; private set; } = type;      // can be read/write if we ever need that

    Matrix4x4 Matrix => Calculator.BuildSplitterMatrix(Direction, Type);

    protected override void CalculateInner()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}