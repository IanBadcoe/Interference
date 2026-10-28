
using System.Collections.Generic;
using System.Numerics;
using Godot_Util;
using MNL = MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths;

public class State
{
    MNL.Vector<Complex> InternalVector;

    public MNL.Vector<Complex> Vector => InternalVector.Clone();        // horrible compromise, we need to expose Vector so we can write operator * on Matrix4x4 but MNL.Vector has no way of being made read-only

    public Complex E => InternalVector[0];
    public Complex N => InternalVector[1];
    public Complex W => InternalVector[2];
    public Complex S => InternalVector[3];

    public State(Complex e = default, Complex n = default,
                 Complex w = default, Complex s = default)
    {
        InternalVector = MNL.Vector<Complex>.Build.Dense(4);

        InternalVector[0] = e;
        InternalVector[1] = n;
        InternalVector[2] = w;
        InternalVector[3] = s;
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

    public Complex this[int idx] => InternalVector[idx];

    public Complex this[Directions.Direction dir] => this[(int)dir / 2];

    public static State operator +(State lhs, State rhs)
    {
        return new(lhs.InternalVector + rhs.InternalVector);
    }
}