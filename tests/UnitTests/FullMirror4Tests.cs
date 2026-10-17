using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

using PortsDict = System.Collections.Generic.Dictionary<Directions.Direction, Beam>;

internal class FullMirror4Tests(Directions.Direction direction, Directions.MirrorType type) : FullMirror(direction, type)
{
    public new PortsDict InPorts => InPortsInner;
    public new PortsDict OutPorts => OutPortsInner;
}