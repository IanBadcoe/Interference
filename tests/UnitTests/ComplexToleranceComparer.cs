using System.Numerics;
using Godot;

namespace UnitTests;

public class ComplexToleranceComparer(float tolerance) : IEqualityComparer<Complex>
{
    public double Tolerance { get; } = tolerance;

    public bool WithinTolerance(double a, double b)
    {
        return Mathf.Abs(a - b) <= Tolerance;
    }
    
    public bool Equals(Complex x, Complex y)
    {
        return WithinTolerance(x.Imaginary, y.Imaginary)
               && WithinTolerance(x.Real, y.Real);
    }

    public int GetHashCode(Complex obj)
    {
        return HashCode.Combine(obj.Imaginary, obj.Real);
    }
}