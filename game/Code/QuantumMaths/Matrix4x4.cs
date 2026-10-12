using System;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Interference.Code.QuantumMaths;

using Matrix = Matrix<Complex>;

public class Matrix4x4
{
    readonly Matrix Matrix = null;

    public Matrix4x4()
    {
        Matrix = Matrix.Build.Dense(4, 4);
    }

    public Matrix4x4(Complex[,] values)
    {
        // matrices are column major, but declaring and formatting an array literal to be read as a matrix
        // leads to a row-major arrangement, so swap things here so we can have out literals readable
        Matrix = Matrix.Build.Dense(4, 4, (i, j) => values[j, i]);
    }

    public Matrix4x4(Matrix matrix)
    {
        Matrix = matrix;
    }

    public static Matrix4x4 operator +(Matrix4x4 lhs, Matrix4x4 rhs)
    {
        return new(lhs.Matrix + rhs.Matrix);
    }

    public static State operator *(Matrix4x4 lhs, State rhs)
    {
        return new State(lhs.Matrix * rhs.Vector);
    }
}