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

internal class BeamSplitter4Tests(Directions.Direction direction) : BeamSplitter(direction)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}

internal class FullMirror4Tests(Directions.Direction direction, Directions.MirrorType type) : FullMirror(direction, type)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}





