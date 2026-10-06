using System.Numerics;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Interference.Code.QuantumMaths;

public class Component : DenseMatrix
{
    public Component(Complex v11 = default, Complex v12 = default, Complex v21 = default, Complex v22 = default) : base(2)
    {
        // Values[]
    }
}
