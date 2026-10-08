using System.Collections.Generic;

namespace Interference.Code.QuantumMaths;

public static class BeamDirections
{
    public enum Direction
    {
        Right,
        Up,
        Left,
        Down,
    }

    public static IEnumerable<Direction> AllDirections => [ Direction.Right, Direction.Up, Direction.Left, Direction.Down ];
}