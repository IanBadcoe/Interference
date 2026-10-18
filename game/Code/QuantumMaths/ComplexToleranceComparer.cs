using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public class ComplexToleranceComparer(float tolerance = ComplexToleranceComparer.DEFAULT_TOLERANCE) : IEqualityComparer<Complex>
{
    public const float DEFAULT_TOLERANCE = 1e-3f;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public double Tolerance { get; } = tolerance;

    bool WithinTolerance(double a, double b)
    {
        return Mathf.Abs(a - b) <= Tolerance;
    }
    
    public bool Equals(Complex x, Complex y)
    {
        // let it crash if either are null, that's never our intent here...
        
        // square tolerance, could use (x - y).Magnitude for circular
        // but does it matter, take-home is "small difference" either way
        return WithinTolerance(x.Imaginary, y.Imaginary)
               && WithinTolerance(x.Real, y.Real);
    }

    public int GetHashCode(Complex obj)
    {
        return obj.GetHashCode();
    }
}