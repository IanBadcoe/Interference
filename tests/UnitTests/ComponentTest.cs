using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class ComponentTest
{
    [Fact]
    public void ConnectionCreatesBeamsInBothDirections()
    {
        // any concrete derived class of Component will do...
        Component comp1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        Component comp2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        
        var (beam1, beam2) = comp1.Connect(comp2, Directions.Direction.E, 1);
        
        // we have two beams, one c1.Out --> c2.In and the other c2.Out --> c1.In
        Assert.Equal(beam1, comp1.OutPorts[Directions.Direction.E]);
        Assert.Equal(beam1, comp2.InPorts[Directions.Direction.W]);
        Assert.Equal(comp1, beam1.From);
        Assert.Equal(comp2, beam1.To);
        Assert.Equal(1, beam1.Length);
        
        Assert.Equal(beam2, comp2.OutPorts[Directions.Direction.W]);
        Assert.Equal(beam2, comp1.InPorts[Directions.Direction.E]);
        Assert.Equal(comp2, beam2.From);
        Assert.Equal(comp1, beam2.To);
        Assert.Equal(1, beam2.Length);
    }

    [Fact]
    public void DisconnectionRemovesBeamsInBothDirections()
    {
        {
            Component comp1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
            Component comp2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);

            comp1.Connect(comp2, Directions.Direction.E, 1);
            comp1.Disconnect(Directions.Direction.E);

            Assert.Null(comp1.OutPorts[Directions.Direction.E]);
            Assert.Null(comp2.InPorts[Directions.Direction.W]);
        }
        
        {
            // shouldn't matter which component asks for the disconnection
            Component comp1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
            Component comp2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);

            comp1.Connect(comp2, Directions.Direction.E, 1);
            comp2.Disconnect(Directions.Direction.W);

            Assert.Null(comp1.OutPorts[Directions.Direction.E]);
            Assert.Null(comp2.InPorts[Directions.Direction.W]);
        }
        
        {
            // should work the same if we ask for a connection to null
            Component comp1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
            Component comp2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);

            comp1.Connect(comp2, Directions.Direction.E, 1);
            var (beam1, beam2) = comp1.Connect(null, Directions.Direction.E, 0);

            // and we shouldn't get any beams back...
            Assert.Null(beam1);
            Assert.Null(beam2);
            
            Assert.Null(comp1.OutPorts[Directions.Direction.E]);
            Assert.Null(comp2.InPorts[Directions.Direction.W]);
        }
    }

    [Fact]
    public void ConnectionDisconnectsPreviouslyConnectedComponents()
    {
        var prior_1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        var prior_2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        
        var comp1 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        var comp2 = new FullMirror(Directions.Direction.N, Calculator.MirrorType.OneSided);
        
        comp1.Connect(prior_1, Directions.Direction.E, 1);
        prior_2.Connect(comp2, Directions.Direction.E, 1);
        
        //   comp1 -(going to connect)- comp2
        //        \__________ _________/
        //         __________X_________             both these prior connections should be disconnected
        //        /                    \
        // prior_1                      prior_2

        comp1.Connect(comp2, Directions.Direction.E, 1);
        
        Assert.Null(prior_1.InPorts[Directions.Direction.W]);
        Assert.Null(prior_2.OutPorts[Directions.Direction.E]);
    }
}