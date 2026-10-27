using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public static class Calculator
{
    public const int MAX_PASSES = 100;

    public enum BeamSplitterType
    {
        HalfSilveredMirror,
        PrismCube
    }

    public enum MirrorType
    {
        OneSided,
        TwoSided
    }

    public static int Calculate(ICollection<Component> components)
    {
        var count = 0;
        while(!CalculationStep(components) && count < MAX_PASSES)
            count++;

        return count;
    }

    private static bool CalculationStep(ICollection<Component> components)
    {
        foreach (var comp in components)
        {
            comp.Calculate();
        }
        
        return components.All(x => x.IsConverged());
    }

    private static bool CalculateComponentStep(Component comp)
    {

        return comp.IsConverged();
    }

    public static Complex MoveForward(Complex state, float distance)
    {
        var phase_shift = (distance - Mathf.Floor(distance)) * 2 * Mathf.Pi;

        var ret_state = state;

        if (phase_shift != 0)   // let's avoid rounding errors if we do not need to do a calculation
        {
            ret_state *= Complex.FromPolarCoordinates(1.0, phase_shift);
        }

        return ret_state;
    }

    public static Complex MoveBackwards(Complex state, float distance)
    {
        var phase_shift = (distance - Mathf.Floor(distance)) * 2 * Mathf.Pi;

        var ret_state = state;

        if (phase_shift != 0)   // let's avoid rounding errors if we do not need to do a calculation
        {
            ret_state *= Complex.FromPolarCoordinates(1.0, -phase_shift);
        }

        return ret_state;
    }
    
    public static Matrix4x4 BuildMirrorMatrix(Directions.Direction direction, Calculator.MirrorType type)
    {
        // array rows and columns are in the order Right, Up, Left, Down...

        var values = direction switch
        {
            Directions.Direction.Right => new Complex[,]
            {
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.UpAndRight => new Complex[,]
            {
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.Up => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.UpAndLeft => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero },
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.Left => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.DownAndLeft => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero }
            },
            Directions.Direction.Down => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One }
            },
            Directions.Direction.DownAndRight => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            _ => null
        };

        var ret = new Matrix4x4(values);

        if (type == Calculator.MirrorType.TwoSided)
        {
            ret -= BuildMirrorMatrix(Directions.Reverse(direction), Calculator.MirrorType.OneSided);
        }

        return ret;
    }

    public static Matrix4x4 BuildSplitterMatrix(Directions.Direction direction, BeamSplitterType type)
    {
        switch(type)
        {
            case BeamSplitterType.HalfSilveredMirror:
                return (BuildMirrorMatrix(direction, Calculator.MirrorType.TwoSided)
                        + BuildTransmissionMatrix()) / Mathf.Sqrt2;
            case BeamSplitterType.PrismCube:
                return BuildPrismCubeSplitterMatrix(direction);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    static Matrix4x4 BuildPrismCubeSplitterMatrix(Directions.Direction direction)
    {
        // complete 180 degree symmetry
        if ((int)direction > 4)
        {
            direction = Directions.Subtract(direction, 4);
        }

        Complex[,] values = direction switch
        {
            Directions.Direction.Right => new[,]
            {
                { Complex.One, Complex.Zero, Complex.ImaginaryOne, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.ImaginaryOne, Complex.Zero, Complex.One, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Directions.Direction.UpAndRight => new[,]
            {
                { Complex.Zero, Complex.ImaginaryOne, Complex.One, Complex.Zero },
                { Complex.ImaginaryOne, Complex.Zero, Complex.Zero, Complex.One },
                { Complex.One, Complex.Zero, Complex.Zero, Complex.ImaginaryOne },
                { Complex.Zero, Complex.One, Complex.ImaginaryOne, Complex.Zero }
            },
            Directions.Direction.Up => new[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.One, Complex.Zero, Complex.ImaginaryOne },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.ImaginaryOne, Complex.Zero, Complex.One }
            },
            Directions.Direction.UpAndLeft => new[,]
            {
                { Complex.Zero, Complex.Zero, Complex.One, Complex.ImaginaryOne },
                { Complex.Zero, Complex.Zero, Complex.ImaginaryOne, Complex.One },
                { Complex.One, Complex.ImaginaryOne, Complex.Zero, Complex.Zero },
                { Complex.ImaginaryOne, Complex.One, Complex.Zero, Complex.Zero }
            },
            _ => throw new InvalidOperationException()
        };

        return new Matrix4x4(values) / Mathf.Sqrt2;
    }

    public static Matrix4x4 BuildTransmissionMatrix()
    {
        var values = new Complex[,]
        {
            { Complex.Zero, Complex.Zero,  Complex.One, Complex.Zero },
            { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One },
            {  Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
            { Complex.Zero,  Complex.One, Complex.Zero, Complex.Zero },
        };

        return new(values);
    }
}