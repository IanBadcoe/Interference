namespace Interference.Code.QuantumMaths.Components;

public class BeamSplitter(Directions.Direction direction) : Component(direction)
{
    Matrix4x4 Matrix => Directions.BuildSplitterMatrix(Direction);

    protected override void CalculateInner()
    {
        var state = GatherInputs();

        var outputs = Matrix * state;

        ScatterOutputs(outputs);
    }
}