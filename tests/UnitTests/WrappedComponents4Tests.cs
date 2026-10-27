using System.Numerics;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

using PortsDict = System.Collections.Generic.Dictionary<Directions.Direction, Beam>;

internal class BeamSource4Tests(Directions.Direction direction, Complex state) : BeamSource(direction, state)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}

internal class BeamSplitter4Tests(Directions.Direction direction) : BeamSplitter(direction, Calculator.BeamSplitterType.HalfSilveredMirror)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}

internal class FullMirror4Tests(Directions.Direction direction, Calculator.MirrorType type) : FullMirror(direction, type)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}





