using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public static class Calculator
{
    public const int MAX_PASSES = 100;
    
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
}