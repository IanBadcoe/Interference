using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public static class Directions
{
    // opposite pairs, such as UpAndRight and DownAndLeft are equivalent if TwoSided is also set
    // otherwise, we only reflect in the direction given here
    // N.B. We only support beams going in orthogonal directions, the other directions are used to define component
    // orientations only
    public enum Direction
    {
        Right = 0,          // |
        UpAndRight = 1,     // /
        Up = 2,             // -
        UpAndLeft = 3,      // \
        Left = 4,           // |
        DownAndLeft = 5,    // /
        Down = 6,           // -
        DownAndRight = 7    // \
    }

    public static IEnumerable<Direction> AllDirections => [ Direction.Right, Direction.UpAndRight, Direction.Up, Direction.UpAndLeft,
                                                            Direction.Left, Direction.DownAndLeft, Direction.Down, Direction.DownAndRight ];

    public static IEnumerable<Direction> OrthogonalDirections => [ Direction.Right, Direction.Up, Direction.Left, Direction.Down ];

    public static IEnumerable<Direction> DiagonalDirections => [ Direction.UpAndRight, Direction.UpAndLeft, Direction.DownAndLeft, Direction.DownAndRight ];

    public static IEnumerable<Direction> ReflectedDirections(Direction mirror_direction)
    {
        return mirror_direction switch
        {
            Direction.Right => [Direction.DownAndRight, Direction.Right, Direction.UpAndRight],
            Direction.UpAndRight => [Direction.Right, Direction.Up],
            Direction.Up => [Direction.UpAndRight, Direction.Up, Direction.UpAndLeft],
            Direction.UpAndLeft => [Direction.Up, Direction.Left],
            Direction.Left => [Direction.UpAndLeft, Direction.Left, Direction.DownAndLeft],
            Direction.DownAndLeft => [Direction.Left, Direction.Down],
            Direction.Down => [Direction.DownAndLeft, Direction.Down, Direction.DownAndRight],
            Direction.DownAndRight => [Direction.Down, Direction.Right],
            _ => []
        };
    }

    public static Direction Reverse(Direction direction)
    {
        // works because the directions are in rotation order
        return (Direction)(((int)direction + 4) % 8);
    }

    public static Direction Add(Direction dir, int step)
    {
        if (step < 0)
        {
            return Subtract(dir, -step);
        }
        
        return (Direction)(((int)dir + step) % 8);
    }

    public static Direction Subtract(Direction dir, int step)
    {
        if (step < 0)
        {
            return Add(dir, -step);
        }

        return (Direction)(((int)dir - (step % 8) + 8) % 8);
    }
}