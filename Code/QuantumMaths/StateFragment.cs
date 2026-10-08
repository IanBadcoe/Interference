// using System.Numerics;
// using Godot;

// namespace Interference.Code.QuantumMaths;

// // A state fragment consists of a complex value (amplitude and phase) and a direction.
// //
// // StateFragments enter Beams, get transformed according to the phase-shift for that length and are delivered to the Beams's end.
// //
// // Similarly Components receive StateFragments via Ports, transform them however is required and deliver them to other Ports...

// public struct StateFragment(Complex state, BeamDirections.Direction direction)
// {
//     public readonly BeamDirections.Direction Direction { get; private init; } = direction;
//     public readonly Complex State { get; private init; } = state;

//     // distance - in wavelengths
// }