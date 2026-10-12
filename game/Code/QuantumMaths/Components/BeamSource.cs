using System.Numerics;

namespace Interference.Code.QuantumMaths.Components;

class BeamSource(Directions.Direction direction, Complex state) : Component
{
    public Directions.Direction Direction { get; private init; } = direction;
    public Complex State { get; private init; } = state;

    public override void Calculate()
    {
        // all InPorts are ignored

        // our OutPorts only have a value in the direction we face
        // and that has a fixed value
        var out_beam = OutPorts[Direction];
        out_beam.Start = State;

        // nothing changes, so we'll always be converged
    }
}