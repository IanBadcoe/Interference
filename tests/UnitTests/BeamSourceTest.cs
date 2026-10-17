using System.Numerics;
using Interference.Code.QuantumMaths;
using Components = Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class BeamSourceTest
{
    [Fact]
    public void DoesNotCrashWhenRunWithNoPortsConnected()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new Components.BeamSource(dir, new Complex(4, 3));

            Assert.False(source.IsConverged());
            
            source.Calculate();
            Assert.True(source.IsConverged());
        }
    }

    [Fact]
    public void DoesNotCreatePortsWhenRunWithNoPortsConnected()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new Components.BeamSource(dir, new Complex(4, 3));

            source.Calculate();

            foreach (var other_dir in Directions.OrthogonalDirections)
            {
                Assert.Null(source.OutPorts[other_dir]);
                Assert.Null(source.InPorts[other_dir]);
            }
        }
    }

    [Fact]
    public void ConvergesWhenCalculated()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new Components.BeamSource(dir, new Complex(4, 3));

            foreach (var other_dir in Directions.OrthogonalDirections)
            {
                Assert.Null(source.OutPorts[other_dir]);
            }

            source.OutPorts[dir] = new Beam(source, null, 1);
            
            Assert.False(source.IsConverged());

            var passes = 0;
            
            while (!source.IsConverged())
            {
                source.Calculate();
                
                Assert.True(passes++ < 3);      // 3 should be plenty
            }
        }
    }
    
    [Fact]
    public void SetsOutputPortWhenCalculated()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new Components.BeamSource(dir, new Complex(4, 3));

            source.OutPorts[dir] = new Beam(source, null, 1);
            
            source.Calculate();

            Assert.Equal(source.OutPorts[dir].Start, new Complex(4, 3));
        }
    }
}