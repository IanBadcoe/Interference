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

    public enum MirrorType
    {
        OneSided,
        TwoSided
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

    public static Matrix4x4 BuildMirrorMatrix(Direction direction, MirrorType type)
    {
        // array rows and columns are in the order Right, Up, Left, Down...

        var values = direction switch
        {
            Direction.Right => new Complex[,]
            {
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Direction.UpAndRight => new Complex[,]
            {
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Direction.Up => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Direction.UpAndLeft => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero },
                { Complex.Zero, Complex.One, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Direction.Left => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            Direction.DownAndLeft => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One },
                { Complex.Zero, Complex.Zero, Complex.One, Complex.Zero }
            },
            Direction.Down => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One }
            },
            Direction.DownAndRight => new Complex[,]
            {
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.One },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                { Complex.One, Complex.Zero, Complex.Zero, Complex.Zero }
            },
            _ => null
        };

        var ret = new Matrix4x4(values);

        if (type == MirrorType.TwoSided)
        {
            ret -= BuildMirrorMatrix(Reverse(direction), MirrorType.OneSided);
        }

        return ret;
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

    public static Matrix4x4 BuildSplitterMatrix(Direction direction)
    {
        return (BuildMirrorMatrix(direction, MirrorType.TwoSided)
                + BuildTransmissionMatrix()) / Mathf.Sqrt2;
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