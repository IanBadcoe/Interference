using System.Numerics;
using Interference.Code.QuantumMaths;

namespace UnitTests;

public class BeamTest
{
    readonly ComplexToleranceComparer Tol = new(1e-6f);
    
    [Fact]
    public void ZeroPhaseTest()
    {
        // length is in wavelengths, so any integer should give zero phase-shift
        {
            var beam = new Beam(null, null, 1);

            beam.Start = new(3, 4);
            Assert.True(beam.End == new Complex(3, 4));

            beam.Start = new(1, 2);
            Assert.True(beam.End == new Complex(1, 2));
        }

        {
            var beam = new Beam(null, null, 2);

            beam.Start = new(3, 4);
            Assert.True(beam.End == new Complex(3, 4));
        }
                
        {
            var beam = new Beam(null, null, 27213);
                
            beam.Start = new(3, 4);
            Assert.True(beam.End == new Complex(3, 4));
        }
    }

    [Fact]
    public void PhaseShiftTest()
    {
        // length is in wavelengths, so any integer should give zero phase-shift
        // and the direction of any shift is clockwise (-ve) because we are *retarding* the wave
        {
            var beam = new Beam(null, null, 0.25f);                   // 1/4 wavelength => 1/4 turn anticlockwise => Pi/2 degrees

            beam.Start = new(1, 0);
            Assert.Equal(beam.End, new Complex(0, -1), Tol);     // Pi/2 degrees anticlockwise

            beam.Start = new(1, 2);
            Assert.Equal(beam.End, new Complex(2, -1), Tol);
        }

        {
            var beam = new Beam(null, null, 0.5f);                      // 1/2 wavelength => 1/2 turn anticlockwise => Pi degrees

            beam.Start = new(1, 0);
            Assert.Equal(beam.End, new Complex(-1, 0), Tol);      // Pi/2 degrees anticlockwise

            beam.Start = new(1, 2);
            Assert.Equal(beam.End, new Complex(-1, -2), Tol);
        }

        {
            var beam = new Beam(null, null, 0.75f);                     // 3/4 wavelength => 3/4 turn anticlockwise => 3*Pi/2 degrees

            beam.Start = new(1, 0);
            Assert.Equal(beam.End, new Complex(0, 1), Tol);   // 3*Pi/2 degrees anticlockwise

            beam.Start = new(1, 2);
            Assert.Equal(beam.End, new Complex(-2, 1), Tol);
        }    }
}