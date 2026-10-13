using System.Numerics;
using Interference.Code.QuantumMaths;
using Components = Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class FullMirrorTest
{
    [Fact]
    public void NullsInOutNoCrashTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            Assert.False(mirror.IsConverged());
            mirror.Calculate();
        }
    }

    [Fact]
    public void OrthogonalReflectOneSideTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[dir] = in_beam;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;

            in_beam.Start = new Complex(3, 4);

            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(-3, -4));

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(-3, -4));  // and we shouldn't change
        }
    }

    [Fact]
    public void OrthogonalNonReflectOneSideTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[dir] = in_beam;

            foreach(var bad_dir in Directions.OrthogonalDirections.Where(x => x != dir))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }
            
            in_beam.Start = new Complex(3, 4);

            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            foreach (var bad_dir in Directions.OrthogonalDirections.Where(x => x != dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            foreach (var bad_dir in Directions.OrthogonalDirections.Where(x => x != dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }

    [Fact]
    public void OrthogonalReflectTwoSidesTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var back_dir = Directions.Reverse(dir);
            
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.TwoSided);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[dir] = in_beam;
            var in_beam2 = new Beam(null, mirror, 1);
            mirror.InPorts[back_dir] = in_beam2;

            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[dir] = out_beam;
            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[back_dir] = out_beam2;

            in_beam.Start = new Complex(3, 4);
            in_beam2.Start = new Complex(4, 3);

            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(-3, -4));
            Assert.Equal(out_beam2.Start, new Complex(-4, -3));

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(-3, -4));  // and we shouldn't change
            Assert.Equal(out_beam2.Start, new Complex(-4, -3));
        }
    }
    
    [Fact]
    public void OrthogonalNonReflectTwoSidesTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var back_dir = Directions.Reverse(dir);

            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[dir] = in_beam;

            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != dir && x != back_dir))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }
            
            in_beam.Start = new Complex(3, 4);

            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != dir && x != back_dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != dir && x != back_dir))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }
}
