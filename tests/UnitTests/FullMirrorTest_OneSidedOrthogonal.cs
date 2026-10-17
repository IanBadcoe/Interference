using System.Numerics;
using Interference.Code.QuantumMaths;

namespace UnitTests;

public class FullMirrorTest_OneSidedOrthogonal
{
    [Fact]
    public void DoesNotCrashWithNullPorts()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.OneSided);

            Assert.False(mirror.IsConverged());
            mirror.Calculate();
        }
    }

    [Fact]
    public void ConvergesWhenCalculated()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.OneSided);

            var in_beam = new Beam(null, mirror, 1)
            {
                Start = new Complex(3, 4)
            };
            mirror.InPorts[dir] = in_beam;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;

            var passes = 0;

            while (!mirror.IsConverged())
            {
                mirror.Calculate();
                
                Assert.True(passes++ < 3);          // 3 should be plenty
            }
        }
    }

    [Fact]
    public void ReflectsOnTheSideItFaces()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.OneSided);

            var in_beam = new Beam(null, mirror, 1)
            {
                Start = new Complex(3, 4)
            };
            mirror.InPorts[dir] = in_beam;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;
            
            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            mirror.Calculate();
            Assert.False(mirror.IsConverged()); // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(3, 4));

            mirror.Calculate();
            Assert.True(mirror.IsConverged()); // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(3, 4)); // and we shouldn't change
        }
    }

    [Fact]
    public void DoesNotReflectOnSidesItDoesNotFace()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new FullMirror4Tests(dir, Directions.MirrorType.OneSided);

            foreach(var in_dir in Directions.OrthogonalDirections)
            {
                var in_beam = new Beam(null, mirror, 1)
                {
                    Start = new Complex(3, 4)
                };
                
                mirror.InPorts[in_dir] = in_beam;
            }

            foreach (var bad_dir in Directions.OrthogonalDirections.Where(x => x != dir))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }
            
            mirror.Calculate();
            Assert.True(mirror.IsConverged()); // now we know we're not changing anymore
            foreach (var bad_dir in Directions.OrthogonalDirections.Where(x => x != dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }
}