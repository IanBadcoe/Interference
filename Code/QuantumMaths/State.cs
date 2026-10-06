
using System.Numerics;
using Godot_Util;

namespace Interference.Code.QuantumMaths;

public class State : MathNet.Numerics.LinearAlgebra.Complex.DenseVector
{
    public State(Complex v1 = default, Complex v2 = default) : base(2)
    {
        Values[0] = v1;
        Values[1] = v2;
    }

    public State(Complex[] values) : base(2)
    {
        Util.Assert(values.Length == 2);

        int idx = 0;

        foreach(var cmp in values)
        {
            Values[idx++] = cmp;
        }
    }
}