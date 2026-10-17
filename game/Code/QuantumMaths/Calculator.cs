using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public static class Calculator
{
    const int MAX_PASSES = 100;
    
    public static int Calculate(ICollection<Component> components)
    {
        var count = 0;
        while(!CalculationStep(components) && count < MAX_PASSES)
            count++;

        return count;
    }

    private static bool CalculationStep(ICollection<Component> components)
    {
        var completed = true;

        foreach(var comp in components)
        {
            if (!CalculateComponentStep(comp))
            {
                completed = false;
            }
        }

        return completed;
    }

    private static bool CalculateComponentStep(Component comp)
    {
        comp.Calculate();

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