using System.Numerics;
using Interference.Code.QuantumMaths;

namespace UnitTests;

public class FullMirrorTest_TwoSidedOrthogonal
{
    [Fact]
    public void DoesNotCrashWithNullPorts()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            Assert.False(mirror.IsConverged());
            mirror.Calculate();
        }
    }

    [Fact]
    public void ConvergesWhenCalculated()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var back_dir = Directions.Reverse(dir);

            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            var in_beam = new Beam(null, mirror, 1)
            {
                Start = new Complex(3, 4)
            };
            mirror.InPorts[dir] = in_beam;

            var in_beam2 = new Beam(null, mirror, 1)
            {
                Start = new Complex(4, 3)
            };
            mirror.InPorts[back_dir] = in_beam2;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;

            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[back_dir] = out_beam2;

            var passes = 0;

            while (!mirror.IsConverged())
            {
                mirror.Calculate();

                Assert.True(passes++ < 3); // 3 should be plenty
            }
        }
    }

    [Fact]
    public void ReflectsOnTheSideItFacesAndTheBack()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var back_dir = Directions.Reverse(dir);

            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.TwoSided);

            var in_beam = new Beam(null, mirror, 1)
            {
                Start = new Complex(3, 4)
            };
            mirror.InPorts[dir] = in_beam;

            var in_beam2 = new Beam(null, mirror, 1)
            {
                Start = new Complex(4, 3)
            };
            mirror.InPorts[back_dir] = in_beam2;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;

            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[back_dir] = out_beam2;

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            mirror.Calculate();
            Assert.False(mirror.IsConverged()); // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(3, 4));
            Assert.Equal(out_beam2.Start, new Complex(-4, -3)); // <- phase-shifted 180 degrees

            mirror.Calculate();
            Assert.True(mirror.IsConverged()); // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(3, 4)); // and we shouldn't change
            Assert.Equal(out_beam2.Start, new Complex(-4, -3)); // <- phase-shifted 180 degrees
        }
    }

    [Fact]
    public void DoesNotReflectOnOtherSides()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var back_dir = Directions.Reverse(dir);

            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.OneSided);

            foreach (var in_dir in Directions.OrthogonalDirections)
            {
                var in_beam = new Beam(null, mirror, 1)
                {
                    Start = new Complex(3, 4)
                };

                mirror.InPorts[in_dir] = in_beam;
            }

            foreach (var bad_dir in
                     Directions.OrthogonalDirections.Where(x => x != dir && x != back_dir))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }


            mirror.Calculate();
            Assert.True(mirror.IsConverged()); // now we know we're not changing anymore
            foreach (var bad_dir in
                     Directions.OrthogonalDirections.Where(x => x != dir && x != back_dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }
}