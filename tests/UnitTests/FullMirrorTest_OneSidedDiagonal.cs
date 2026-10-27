using System.Numerics;
using Interference.Code.QuantumMaths;
using Interference.LevelObject.QuantumComponent;

namespace UnitTests;

public partial class FullMirrorTestOneSidedDiagonal : QuantumComponent
{
    [Fact]
    public void DoesNotCrashWithNullPorts()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Calculator.MirrorType.OneSided);

            Assert.False(mirror.IsConverged());
            mirror.Calculate();
        }
    }

    [Fact]
    public void ConvergesWhenCalculated()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Calculator.MirrorType.OneSided);

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
                
                Assert.True(passes++ <= 3);          // 3 should be plenty
            }
        }
    }

    [Fact]
    public void SwapsBeamsOnAdjacentOrthogonalSides()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Calculator.MirrorType.OneSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);

            var in_beam = new Beam(null, mirror, 1)
            {
                Start = new Complex(3, 4)
            };
            mirror.InPorts[beam_dir] = in_beam;

            var in_beam2 = new Beam(null, mirror, 1)
            {
                Start = new Complex(4, 3)
            };
            mirror.InPorts[beam_dir2] = in_beam2;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir] = out_beam;

            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir2] = out_beam2;


            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            // a diagonal mirror swaps beams coming in on each relevant orthogonal for an out beam on the other

            mirror.Calculate();
            Assert.False(mirror.IsConverged()); // we need a second pass to know
            Assert.Equal(out_beam2.Start, new Complex(3, 4));
            Assert.Equal(out_beam.Start, new Complex(4, 3));

            mirror.Calculate();
            Assert.True(mirror.IsConverged()); // now we know we're not changing anymore
            Assert.Equal(out_beam2.Start, new Complex(3, 4)); // and we shouldn't change
            Assert.Equal(out_beam.Start, new Complex(4, 3));
        }
    }

    [Fact]
    public void DoesNotReflectOnOtherSides()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Calculator.MirrorType.OneSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);

            foreach (var in_dir in Directions.OrthogonalDirections)
            {
                var in_beam = new Beam(null, mirror, 1)
                {
                    Start = new Complex(3, 4)
                };

                mirror.InPorts[in_dir] = in_beam;
            }

            foreach (var bad_dir in
                     Directions.OrthogonalDirections.Where(x => x != beam_dir && x != beam_dir2))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            // a diagonal mirror swaps beams coming in on each relevant orthogonal for an out beam on the other

            mirror.Calculate();
            foreach (var bad_dir in
                     Directions.OrthogonalDirections.Where(x => x != beam_dir && x != beam_dir2))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }
}