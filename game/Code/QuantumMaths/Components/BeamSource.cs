using System.Numerics;

namespace Interference.Code.QuantumMaths.Components;

public class BeamSource(Directions.Direction direction, Complex state, string name = "anon source")
    : Component(direction, name)
{
    public Complex State { get; private init; } = state;

    public override bool IsConverged() => HasCalculated;        // we always converge after one pass as inputs are ignored
    
    protected override void CalculateInner()
    {
        // all InPorts are ignored

        // our OutPorts only have a value in the direction we face
        // and that has a fixed value
        var out_beam = OutPorts[Direction];

        if (out_beam != null)
        {
            out_beam.Start = State;
        }
        
        // nothing else changes, so we'll now always be converged
    }
}