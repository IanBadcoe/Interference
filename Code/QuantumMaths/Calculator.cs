using System.Collections.Generic;
using System.Numerics;
using Godot;
using Interference.Code.QuantumMaths;

public static class Calculator
{
    public static void Calculate(IEnumerable<Component> components)
    {
        while(!CalculationStep(components))
            ;
    }

    private static bool CalculationStep(IEnumerable<Component> components)
    {
        bool completed = true;

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
        float phase_shift = (distance - Mathf.Floor(distance)) * 2 * Mathf.Pi;

        Complex ret_state = state;

        if (phase_shift != 0)   ///< let's avoid rounding errors if we do not need to do a calculation
        {
            ret_state *= Complex.FromPolarCoordinates(1.0, phase_shift);
        }

        return ret_state;
    }
}