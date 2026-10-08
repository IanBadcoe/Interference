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