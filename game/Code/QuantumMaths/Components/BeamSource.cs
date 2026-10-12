using System.Numerics;

namespace Interference.Code.QuantumMaths.Components;

class BeamSource(BeamDirections.Direction direction, Complex state) : Component
{
    public BeamDirections.Direction Direction { get; private init; } = direction;
    public Complex State { get; private init; } = state;

    public override void Calculate()
    {
        // all InPorts are ignored

        // our OutPorts only have a value in the direction we face
        // and that has a fixed value
        var out_beam = OutPortsInternal[Direction];
        out_beam.Start = State;

        (OutPortsInternal[Direction]).Start = State;

        // nothing changes, so we'll always be converged
    }
}