using System.Numerics;
using Interference.Code.QuantumMaths;
using Components = Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class FullMirrorTest_TwoSidedDiagonal
{
    [Fact]
    public void DoesNotCrashWithNullPorts()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            Assert.False(mirror.IsConverged());
            mirror.Calculate();
        }
    }

    [Fact]
    public void ConvergesWhenCalculated()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var back_dir = Directions.Reverse(dir);

            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);

            foreach (var ortho_dir in new[] { beam_dir, beam_dir2 })
            {
                var in_beam = new Beam(null, mirror, 1)
                {
                    Start = new Complex(3, 4)
                };

                mirror.InPorts[ortho_dir] = in_beam;

                var out_beam = new Beam(mirror, null, 1);
                mirror.OutPorts[ortho_dir] = out_beam;
            }
            

            var passes = 0;

            while (!mirror.IsConverged())
            {
                mirror.Calculate();

                Assert.True(passes++ <= 3); // 3 should be plenty
            }
        }
    }

    [Fact]
    public void SwapsBeamsOnAdjacentDirectionsFrontAndBack()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);
            var beam_dir_rev = Directions.Reverse(beam_dir);
            var beam_dir_rev2 = Directions.Reverse(beam_dir2);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir] = in_beam;
            var in_beam2 = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir2] = in_beam2;
            var in_beam_rev = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir_rev] = in_beam_rev;
            var in_beam_rev2 = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir_rev2] = in_beam_rev2;
            
            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir] = out_beam;
            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir2] = out_beam2;
            var out_beam_rev = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir_rev] = out_beam_rev;
            var out_beam_rev2 = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir_rev2] = out_beam_rev2;
            
            in_beam.Start = new Complex(3, 4);
            in_beam2.Start = new Complex(4, 3);
            in_beam_rev.Start = new Complex(1, 2);
            in_beam_rev2.Start = new Complex(2, 1);

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            // a diagonal mirror swaps beams coming in on each relevant orthogonal for an out beam on the other
            
            mirror.Calculate();
            Assert.False(mirror.IsConverged());                                 // we need a second pass to know
            Assert.Equal(out_beam2.Start, new Complex(3, 4));
            Assert.Equal(out_beam.Start, new Complex(4, 3));
            Assert.Equal(out_beam_rev2.Start, new Complex(-1, -2)); // <- 180 degree phase-shift
            Assert.Equal(out_beam_rev.Start, new Complex(-2, -1));  // <- 180 degree phase-shift

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                                  // now we know we're not changing anymore
            Assert.Equal(out_beam2.Start, new Complex(3, 4));       // and we shouldn't change
            Assert.Equal(out_beam.Start, new Complex(4, 3));
            Assert.Equal(out_beam_rev2.Start, new Complex(-1, -2)); // <- 180 degree phase-shift
            Assert.Equal(out_beam_rev.Start, new Complex(-2, -1));  // <- 180 degree phase-shift
        }
    }

    [Fact]
    public void DiagonalNonReflectTestTwoSided()
    {
        // a diagonal mirror reflects in all four possible beam directions
        // so nothing to test here
        
        // (this just to document that)
    }
}
