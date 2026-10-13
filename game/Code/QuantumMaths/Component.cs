using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot_Util;

namespace Interference.Code.QuantumMaths;

using PortsDict = Dictionary<Directions.Direction, Beam>;

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
        { Directions.Direction.Right, null },
        { Directions.Direction.Up, null },
        { Directions.Direction.Left, null },
        { Directions.Direction.Down, null },
    };

    public PortsDict OutPorts = new()
    {
        { Directions.Direction.Right, null },
        { Directions.Direction.Up, null },
        { Directions.Direction.Left, null },
        { Directions.Direction.Down, null },
    };

    // when the InPorts do not change between two evaluation cycles, then (according to this component, at least)
    // we have converged (but we need _all_ components to converge to really know)
    readonly PortsDict OutPortArchive = new()
    {
        { Directions.Direction.Right, null },
        { Directions.Direction.Up, null },
        { Directions.Direction.Left, null },
        { Directions.Direction.Down, null },
    };

    protected bool HasCalculated { get; private set; }

    public void Calculate()
    {
        CalculateInner();

        HasCalculated = true;
    }
    
    protected abstract void CalculateInner();

    public virtual bool IsConverged()
    {
        if (!HasCalculated)
        {
            return false;
        }
        
        var ret = true;

        foreach(var dir in Directions.OrthogonalDirections)
        {
            var port = OutPorts[dir];
            if (port == null) continue;
            
            var prev_port = OutPortArchive[dir];
            if (prev_port == null
                || (prev_port.Start - port.Start).Magnitude > ConvergenceThreshold)
            {
                ret = false;
            }

            OutPortArchive[dir] = port;
        }

        return ret;
    }

    protected State GatherInputs()
    {
        Util.Assert(InPorts.Count == 4);
        
        return new State(InPorts.OrderBy(x => x.Key).Select(x => x.Value != null ? x.Value.End : new Complex()));
    }

    protected void ScatterOutputs(State state)
    {
        Util.Assert(OutPorts.Count == 4);
        
        foreach(var dir in Directions.OrthogonalDirections)
        {
            var port = OutPorts[dir];

            if (port != null)
            {
                port.Start = state[dir];
            }
        }
    }
}
