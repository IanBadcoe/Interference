using System;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

// A Beam has a length, and a StateFragment at either end.
//
// The amplitude and phase coming in are in the StateFragment at Start and the direction
// is the direction the beam is going; matching the output port on the component we
// came from).
//
// The StageFragment at End has the same amplitude, but phase-shifted according to the beam length, its direction is
// the same; so it is the reverse of the port we're received by, allowing us to have two beams on the same port if necessary,
// one arriving and one leaving (like with orthogonal reflection, boing: |<--->)

public readonly struct Beam(StateFragment start, float length)
{
    public StateFragment Start { get; private init; } = start;

    public StateFragment End => Start.MoveForward(Length);

    public float Length { get; private init; } = length;
    ///< in wavelengths, so that the phase shift is this * 2Pi

    public Beam(Complex state, BeamDirection direction, float length) : this(new StateFragment(state, direction), length) {}
}