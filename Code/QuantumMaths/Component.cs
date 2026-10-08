using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Interference.Code.QuantumMaths;

using PortsDict = Dictionary<BeamDirections.Direction, Beam>;
using IReadonlyPortsDict = IReadOnlyDictionary<BeamDirections.Direction, Beam>;

public abstract class Component
{
    public const double ConvergenceThreshold = 0.001;

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
}
