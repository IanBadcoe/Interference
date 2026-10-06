using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

// A state fragment consists of a complex value (amplitude and phase) and a direction.
//
// StateFragments enter Beams, get transformed according to the phase-shift for that length and are delivered to the Beams's end.
//
// Similarly Components receive StateFragments via Ports, transform them however is required and deliver them to other Ports...

public enum BeamDirection
{
    Up,
    Right,
    Down,
    Left
}

public struct StateFragment(Complex state, BeamDirection direction)
{
    public BeamDirection Direction { get; private init; } = direction;
    public Complex State { get; private init; } = state;

    public StateFragment MoveForward(float distance)
    {
        float phase_shift = distance - Mathf.Floor(distance);

        Complex ret_state = State;

        if (phase_shift != 0)
        {
            ret_state *= Complex.FromPolarCoordinates(1.0, phase_shift);
        }

        return new StateFragment(ret_state, Direction);
    }
}