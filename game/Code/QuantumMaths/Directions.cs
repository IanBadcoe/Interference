using System.Collections.Generic;
using System.Numerics;
using Godot;

namespace Interference.Code.QuantumMaths;

public static class Directions
{
    // N.B. We only support beams going in orthogonal directions, the other directions are used to define component
    // orientations only
    public enum Direction
    {
        E = 0,
        NE = 1,
        N = 2,
        NW = 3,
        W = 4,
        SW = 5,
        S = 6,
        SE = 7
    }

    public static IEnumerable<Direction> AllDirections => [ Direction.E, Direction.NE, Direction.N, Direction.NW,
                                                            Direction.W, Direction.SW, Direction.S, Direction.SE ];

    public static IEnumerable<Direction> OrthogonalDirections => [ Direction.E, Direction.N, Direction.W, Direction.S ];

    public static IEnumerable<Direction> DiagonalDirections => [ Direction.NE, Direction.NW, Direction.SW, Direction.SE ];

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