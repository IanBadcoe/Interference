using System.Collections.Generic;
using System.Numerics;

namespace Interference.Code.QuantumMaths;

public static class MirrorDirections
{
    // opposite pairs, such as UpAndRight and DownAndLeft are equivalent if TwoSided is also set
    // otherwise, we only reflect in the direction given here
    public enum Direction
    {
        Right = 0,          /// |
        UpAndRight = 1,     /// /
        Up = 2,             /// -
        UpAndLeft = 3,      /// \
        Left = 4,           /// |
        DownAndLeft = 5,    /// /
        Down = 6,           /// -
        DownAndRight = 7,   /// \
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
        switch(mirror_direction)
        {
            case Direction.Right:
                return [ Direction.DownAndRight, Direction.Right, Direction.UpAndRight ];
            case Direction.UpAndRight:
                return [ Direction.Right, Direction.Up ];
            case Direction.Up:
                return [ Direction.UpAndRight, Direction.Up, Direction.UpAndLeft ];
            case Direction.UpAndLeft:
                return [ Direction.Up, Direction.Left ];
            case Direction.Left:
                return [ Direction.UpAndLeft, Direction.Left, Direction.DownAndLeft ];
            case Direction.DownAndLeft:
                return [ Direction.Left, Direction.Down ];
            case Direction.Down:
                return [ Direction.DownAndLeft, Direction.Down, Direction.DownAndRight ];
            case Direction.DownAndRight:
                return [ Direction.Down, Direction.Right ];
        }

        return [];
    }

    public static Matrix4x4 BuildMirrorMatrix(Direction direction, MirrorType type)
    {
        Complex[,] values = null;

        // array rows and columns are in the order Right, Up, Left, Down...

        switch(direction)
        {
            case Direction.Right:
                values = new Complex[,] { { -Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;

            case Direction.UpAndRight:
                values = new Complex[,] { { Complex.Zero, -Complex.One, Complex.Zero, Complex.Zero },
                                          { -Complex.One, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;

            case Direction.Up:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, -Complex.One, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;

            case Direction.UpAndLeft:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, -Complex.One, Complex.Zero },
                                          { Complex.Zero, -Complex.One, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;

            case Direction.Left:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, -Complex.One, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;

            case Direction.DownAndLeft:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, -Complex.One },
                                          { Complex.Zero, Complex.Zero, -Complex.One, Complex.Zero } };
                break;

            case Direction.Down:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, -Complex.One } };
                break;

            case Direction.DownAndRight:
                values = new Complex[,] { { Complex.Zero, Complex.Zero, Complex.Zero, -Complex.One },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { Complex.Zero, Complex.Zero, Complex.Zero, Complex.Zero },
                                          { -Complex.One, Complex.Zero, Complex.Zero, Complex.Zero } };
                break;
        }

        var ret = new Matrix4x4(values);

        if (type == MirrorType.TwoSided)
        {
            ret += BuildMirrorMatrix(Reverse(direction), MirrorType.OneSided);
        }

        return ret;
    }

    public static Direction Reverse(Direction direction)
    {
        // works because the directions are in rotation order
        return (Direction)(((int)direction + 4) % 8);
    }
}