
using System.Numerics;
using Godot_Util;
using MNL = MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths;

public class State
{
    readonly MNL.Vector<Complex> Vector = null;

    public Complex Right => Vector[0];
    public Complex Up => Vector[1];
    public Complex Left => Vector[2];
    public Complex Down => Vector[3];

    public State(Complex right = default, Complex up = default,
                 Complex left = default, Complex down = default)
    {
        Vector = MNL.Vector<Complex>.Build.Dense(4);

        Vector[0] = right;
        Vector[1] = up;
        Vector[2] = left;
        Vector[3] = down;
    }

    public State(Complex[] values)
    {
        Util.Assert(values.Length == 2);

        Vector = MNL.Vector<Complex>.Build.Dense(4);

        int idx = 0;

        foreach(var cmp in values)
        {
            Vector[idx++] = cmp;
        }
    }

    public State(MNL.Vector<Complex> old)
    {
        Vector = old.Clone();
    }

    public static State operator +(State lhs, State rhs)
    {
        return new(lhs.Vector + rhs.Vector);
    }
}