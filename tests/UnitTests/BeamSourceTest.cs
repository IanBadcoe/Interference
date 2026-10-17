using System.Numerics;
using Interference.Code.QuantumMaths;
using Components = Interference.Code.QuantumMaths.Components;

namespace UnitTests;

using PortsDict = System.Collections.Generic.Dictionary<Directions.Direction, Beam>;


internal class BeamSource4Tests(Directions.Direction direction, Complex state) : Components.BeamSource(direction, state)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}

public class BeamSourceTest
{
    [Fact]
    public void BeamOutputNullTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new Components.BeamSource(dir, new Complex(4, 3));

            foreach (var other_dir in Directions.OrthogonalDirections)
            {
                Assert.Null(source.OutPorts[other_dir]);
            }
            
            Assert.False(source.IsConverged());
            
            source.Calculate();
            Assert.True(source.IsConverged());
        }
    }

    [Fact]
    public void BeamOutputOutputTest()
    {
        foreach (var dir in Directions.OrthogonalDirections)
        {
            var source = new BeamSource4Tests(dir, new Complex(4, 3));

            foreach (var other_dir in Directions.OrthogonalDirections)
            {
                Assert.Null(source.OutPorts[other_dir]);
            }

            source.OutPorts[dir] = new Beam(source, null, 1);
            
            Assert.False(source.IsConverged());
            
            source.Calculate();
            Assert.True(source.IsConverged());
            
            Assert.Equal(source.OutPorts[dir].Start, new Complex(4, 3));
        }
    }
}