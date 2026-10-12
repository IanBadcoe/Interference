
using System.Collections.Generic;
using System.Numerics;
using Godot_Util;
using MNL = MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths;

public class State
{
    MNL.Vector<Complex> InternalVector;

    public MNL.Vector<Complex> Vector => InternalVector.Clone();        // horrible compromise, we need to expose Vector so we can write operator * on Matrix4x4 but MNL.Vector has no way of being made read-only

    public Complex Right => InternalVector[0];
    public Complex Up => InternalVector[1];
    public Complex Left => InternalVector[2];
    public Complex Down => InternalVector[3];

    public State(Complex right = default, Complex up = default,
                 Complex left = default, Complex down = default)
    {
        InternalVector = MNL.Vector<Complex>.Build.Dense(4);

        InternalVector[0] = right;
        InternalVector[1] = up;
        InternalVector[2] = left;
        InternalVector[3] = down;
    }

    public State(IEnumerable<Complex> values)
    {
        InternalVector = MNL.Vector<Complex>.Build.Dense(4);

        int idx = 0;

        foreach(var cmp in values)
        {
            InternalVector[idx++] = cmp;
        }

        Util.Assert(idx == 4);
    }

    public State(MNL.Vector<Complex> old)
    {
        InternalVector = old.Clone();
    }

    public Complex this[int idx]
    {
        get => InternalVector[idx];
    }

    public Complex this[BeamDirections.Direction dir]
    {
        get => this[(int)dir];
    }

    public static State operator +(State lhs, State rhs)
    {
        return new(lhs.InternalVector + rhs.InternalVector);
    }
}