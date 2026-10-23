using Interference.Code.QuantumMaths;

namespace Interference;

public static class Quantum
{
    public static Directions.Direction FromGodotAngleDegrees(float angleDegrees)
    {
        // Direction enum goes anticlockwise for Maths Reasons. Godot goes clockwise so we need a minus
        return (Directions.Direction)((8 - (angleDegrees / 45)) % 8);
    }
}