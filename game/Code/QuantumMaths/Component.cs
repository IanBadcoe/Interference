using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Godot_Util;

namespace Interference.Code.QuantumMaths;

using PortsDict = Dictionary<Directions.Direction, Beam>;
using ReadOnlyPortsDict = IReadOnlyDictionary<Directions.Direction, Beam>;
using ComplexDict = Dictionary<Directions.Direction, Complex>;
using ReadOnlyComplexDict = IReadOnlyDictionary<Directions.Direction, Complex>;
using ComplexDictionaryKVP = KeyValuePair<Directions.Direction, Complex>;

public abstract class Component
{
    const double ConvergenceThreshold = 0.001;

    static readonly ComplexToleranceComparer Tol = new();

    // Ports are named for the direction they face, not the direction of the beam they are receiving
    // So InPorts[Right] receives a Left-going beam,
    // and OutPorts[Right] emits a Right-going one
    //
    // (currently the only note of a beam direction is the orientation of the ports is connects
    //  but if we need that in the Beam too, it should be trivial to add...)
    protected readonly PortsDict InPortsInner = new()
    {
        { Directions.Direction.Right, null },
        { Directions.Direction.Up, null },
        { Directions.Direction.Left, null },
        { Directions.Direction.Down, null },
    };

    public ReadOnlyPortsDict InPorts => InPortsInner;

    protected readonly PortsDict OutPortsInner = new()
    {
        { Directions.Direction.Right, null },
        { Directions.Direction.Up, null },
        { Directions.Direction.Left, null },
        { Directions.Direction.Down, null },
    };
    
    public ReadOnlyPortsDict OutPorts => OutPortsInner;

    public ReadOnlyComplexDict InAmplitudes
    {
        get
        {
            return new ReadOnlyDictionary<Directions.Direction, Complex>(
                new ComplexDict(InPorts.Select(x
                    => new ComplexDictionaryKVP(x.Key, x.Value?.End ?? Complex.Zero))
                )
            );
        }
    }

    public ReadOnlyComplexDict OutAmplitudes
    {
        get
        {
            return new ReadOnlyDictionary<Directions.Direction, Complex>(
                new ComplexDict(OutPorts.Select(x
                    => new ComplexDictionaryKVP(x.Key, x.Value?.Start ?? Complex.Zero))
                )
            );
        }
    }
    
    // when the InPorts do not change between two evaluation cycles, then (according to this component, at least)
    // we have converged (but we need _all_ components to converge to really know)
    readonly ComplexDict InPortArchive = new()
    {
        { Directions.Direction.Right, Complex.Zero },
        { Directions.Direction.Up, Complex.Zero },
        { Directions.Direction.Left, Complex.Zero },
        { Directions.Direction.Down, Complex.Zero },
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
            var port = InPorts[dir];
            if (port == null) continue;                             // InPorts with no Beam are fixed at zero
            
            var prev_port_start = InPortArchive[dir];       // initialized with zero, if we did not receive
                                                                    // anything different to that, then we didn't
                                                                    // change (yet)
                                                                    
            if (!Tol.Equals(prev_port_start, InPorts[dir].End))
            {
                ret = false;
            }

            InPortArchive[dir] = port.Start;
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
    
#region Ports and Beams

    public (Beam out_beam, Beam in_beam) Connect(
        Component other,
        Directions.Direction direction, float length)
    {
        var out_beam = OutConnectInternal(this, other, direction, length);
        var in_beam = OutConnectInternal(other, this, Directions.Reverse(direction), length);

        return (out_beam, in_beam);
    }

    // ONLY public for unit-testing, should not need in normal code
    static Beam OutConnectInternal(Component start, Component end, Directions.Direction direction, float length)
    {
        if (start == null && end == null)
        {
            throw new ArgumentNullException(nameof(start), "Start and end cannot both be null");
        }

        var rev_direction = Directions.Reverse(direction);

        end?.DisconnectIn(rev_direction);
        start?.DisconnectOut(direction);

        if (start == null || end == null)
        {
            return null;
        }

        var beam = new Beam(start, end, length);
        start.OutPortsInner[direction] = beam;
        end.InPortsInner[rev_direction] = beam;

        return beam;
    }

    void DisconnectIn(Directions.Direction direction)
    {
        var existing_beam = InPorts[direction];

        if (existing_beam != null)
        {
            existing_beam.From.OutPortsInner[Directions.Reverse(direction)] = null;
        }
        
        InPortsInner[direction] = null;
    }

    void DisconnectOut(Directions.Direction direction)
    {
        var existing_beam = OutPorts[direction];

        if (existing_beam != null)
        {
            existing_beam.To.InPortsInner[Directions.Reverse(direction)] = null;
        }
        
        OutPortsInner[direction] = null;
    }
    
    public void Disconnect(Directions.Direction direction)
    {
        Connect(null, direction, 0);
    }
#endregion Ports and Beams
}
