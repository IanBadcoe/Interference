using System.Collections.Generic;

namespace Interference.Code.QuantumMaths;

public static class BeamDirections
{
    public enum Direction
    {
        Right = 0,
        Up = 1,
        Left = 2,
        Down = 3,
    }

    public static IEnumerable<Direction> AllDirections => [ Direction.Right, Direction.Up, Direction.Left, Direction.Down ];
}