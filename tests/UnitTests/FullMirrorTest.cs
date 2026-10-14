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

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)
            
            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(3, 4));

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(3, 4));    // and we shouldn't change
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

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)
            
            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            Assert.Equal(out_beam.Start, new Complex(3, 4));
            Assert.Equal(out_beam2.Start, new Complex(-4, -3)); // <- phase-shifted 180 degrees

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(3, 4));    // and we shouldn't change
            Assert.Equal(out_beam2.Start, new Complex(-4, -3)); // <- phase-shifted 180 degrees
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
    
    [Fact]
    public void DiagonalReflectTestOneSided()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir] = in_beam;
            var in_beam2 = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir2] = in_beam2;
            
            var out_beam = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir] = out_beam;
            var out_beam2 = new Beam(mirror, null, 1);
            mirror.OutPorts[beam_dir2] = out_beam2;
            
            in_beam.Start = new Complex(3, 4);
            in_beam2.Start = new Complex(4, 3);

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            // a diagonal mirror swaps beams coming in on each relevant orthogonal for an out beam on the other
            
            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            Assert.Equal(out_beam2.Start, new Complex(3, 4));
            Assert.Equal(out_beam.Start, new Complex(4, 3));

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam2.Start, new Complex(3, 4));   // and we shouldn't change
            Assert.Equal(out_beam.Start, new Complex(4, 3));
        }
    }
    
    [Fact]
    public void DiagonalNonReflectTestOneSided()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.OneSided);

            var beam_dir = Directions.Add(dir, 1);
            var beam_dir2 = Directions.Subtract(dir, 1);

            var in_beam = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir] = in_beam;
            var in_beam2 = new Beam(null, mirror, 1);
            mirror.InPorts[beam_dir2] = in_beam2;
            
            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != beam_dir && x != beam_dir2))
            {
                mirror.OutPorts[bad_dir] = new Beam(mirror, null, 1);
            }
            
            in_beam.Start = new Complex(3, 4);
            in_beam2.Start = new Complex(4, 3);

            // for the moment we chose a mirror design which phase-shifts the rear and not the front
            // (e.g. back-silvered...)

            // a diagonal mirror swaps beams coming in on each relevant orthogonal for an out beam on the other
            
            mirror.Calculate();
            Assert.False(mirror.IsConverged());                             // we need a second pass to know
            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != beam_dir && x != beam_dir2))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }

            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            foreach(var bad_dir in
                    Directions.OrthogonalDirections.Where(x => x != beam_dir && x != beam_dir2))
            {
                Assert.Equal(mirror.OutPorts[bad_dir].Start, new Complex());
            }
        }
    }
    
    [Fact]
    public void DiagonalReflectTestTwoSided()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var mirror = new Components.FullMirror(dir, Directions.MirrorType.TwoSided);

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
