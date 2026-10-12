using System.Numerics;
using Godot;
using Interference.Code.QuantumMaths;
using Components = Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class FullMirrorTest
{
    [Fact]
    public void OrthogonalReflectTest()
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
            Assert.False(mirror.IsConverged());                             // we need to second pass to know
            Assert.Equal(out_beam.Start, new Complex(-3, -4));
            
            mirror.Calculate();
            Assert.True(mirror.IsConverged());                              // now we know we're not changing anymore
            Assert.Equal(out_beam.Start, new Complex(-3, -4));  // and we shouldn't change
        }
    }
}
