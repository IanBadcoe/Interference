using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Interference.Code.QuantumMaths;

using PortsDict = Dictionary<BeamDirections.Direction, Beam>;
using IReadonlyPortsDict = IReadOnlyDictionary<BeamDirections.Direction, Beam>;

public abstract class Component
{
    public const double ConvergenceThreshold = 0.001;

    // Ports are named for the direction they face, not the direction of the beam they are receiving
    // So InPorts[Right] receives a Left-going beam,
    // and OutPorts[Right] emits a Right-going one
    //
    // (currently the only note of a beam direction is the orientation of the ports is connects
    //  but if we need that in the Beam too, it should be trivial to add...)
    public PortsDict InPorts { get; set; } = new()
    {
        { BeamDirections.Direction.Right, null },
        { BeamDirections.Direction.Up, null },
        { BeamDirections.Direction.Left, null },
        { BeamDirections.Direction.Down, null },
    };

    protected PortsDict OutPortsInternal = new()
    {
        { BeamDirections.Direction.Right, null },
        { BeamDirections.Direction.Up, null },
        { BeamDirections.Direction.Left, null },
        { BeamDirections.Direction.Down, null },
    };
    public IReadonlyPortsDict OutPorts => OutPortsInternal;

    // when the InPorts do not change between two evaluation cycles, then (according to this component, at least)
    // we have converged (but we need _all_ components to converge to really know)
    readonly PortsDict InPortArchive = new();

    public abstract void Calculate();

    public bool IsConverged()
    {
        bool ret = true;

        foreach(var dir in BeamDirections.AllDirections)
        {
            if ((InPortArchive[dir].Start - InPorts[dir].Start).Magnitude > ConvergenceThreshold)
            {
                ret = false;
            }

            InPortArchive[dir] = InPorts[dir];
        }

        return ret;
    }

    protected State GatherInputs()
    {
        return new State(InPorts.OrderBy(x => x.Key).Select(x => x.Value != null ? x.Value.End : new Complex()));
    }

    protected void ScatterOutputs(State state)
    {
        foreach(var dir in BeamDirections.AllDirections)
        {
            var port = OutPorts[dir];

            if (port != null)
            {
                port.Start = state[dir];
            }
        }
    }
}
