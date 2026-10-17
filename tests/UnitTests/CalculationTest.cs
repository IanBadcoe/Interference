using System.Numerics;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class CalculationTest
{
    [Fact]
    public void ASingleBeamConnectingTwoComponentsConverges()
    {
        // [source]=-----\
        var amplitude = new Complex(3, 4);
        var source = new BeamSource(Directions.Direction.Right, amplitude);
        var target = new FullMirror(Directions.Direction.UpAndLeft, Directions.MirrorType.TwoSided);
        source.Connect(target, Directions.Direction.Right, 1);

        Calculator.Calculate([source, target]);
        
        Assert.Equal(target.InPorts[Directions.Direction.Left].Start, amplitude);
    }
    
    [Fact]
    public void ALShapeConverges()
    {
        //             -----
        //               |
        //               |
        // [source]=-----/

        var amplitude = new Complex(3, 4);
        
        var source = new BeamSource(Directions.Direction.Right, amplitude);
        var mirror = new FullMirror(Directions.Direction.UpAndLeft, Directions.MirrorType.TwoSided);
        var absorber = new PassiveAbsorber();
        
        source.Connect(mirror, Directions.Direction.Right, 1);
        mirror.Connect(absorber, Directions.Direction.Up, 1);

        Calculator.Calculate([source, mirror, absorber]);
        
        Assert.Equal(absorber.InPorts[Directions.Direction.Down].Start, amplitude);
    }

    [Fact]
    public void ACrossOfMirrorsConverges()
    {
        //             -----
        //               |
        //               |      |
        // [source]=-----\------|
        //               |      |
        //               |
        //             -----
    }
}