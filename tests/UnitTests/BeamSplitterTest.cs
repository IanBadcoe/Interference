using System.Numerics;
using Godot;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class BeamSplitterTest
{
    readonly ComplexToleranceComparer Tol = new();

    [Fact]
    public void AnOrthogonalSplitterTransmitsHalfAndReturnsHalf()
    {
        foreach(var dir in Directions.OrthogonalDirections)
        {
            var rev_dir = Directions.Reverse(dir);
            var expected = Complex.One / Mathf.Sqrt2;
        
            var splitter = new BeamSplitter(rev_dir);
            var source = new BeamSource(dir, Complex.One);
            var dest = new PassiveAbsorber();

            source.Connect(splitter, dir, 1);
            splitter.Connect(dest, dir, 1);

            var passes = Calculator.Calculate([source, splitter, dest]);
            
            Assert.True(passes < 3);
            
            // half amplitude returning to source (which ignores it, but we can examine the value)
            Assert.Equal(expected, source.InPorts[dir].End, Tol);
            // half amplitude transmitted to dest
            Assert.Equal(expected, dest.InPorts[rev_dir].End, Tol);
        }
    }
    
    [Fact]
    public void AReversedOrthogonalSplitterTransmitsHalfAndReturnsHalf()
    {
        foreach(var dir in Directions.OrthogonalDirections)
        {
            var rev_dir = Directions.Reverse(dir);
            var expected = Complex.One / Mathf.Sqrt2;
        
            var splitter = new BeamSplitter(dir);
            var source = new BeamSource(dir, Complex.One);
            var dest = new PassiveAbsorber();

            source.Connect(splitter, dir, 1);
            splitter.Connect(dest, dir, 1);

            var passes = Calculator.Calculate([source, splitter, dest]);
            
            Assert.True(passes < 3);
            
            // half amplitude returning to source, phase-shifted (which ignores it, but we can examine the value)
            Assert.Equal(-expected, source.InPorts[dir].End, Tol);
            // half amplitude transmitted to dest
            Assert.Equal(expected, dest.InPorts[rev_dir].End, Tol);
        }
    }
    
    [Fact]
    public void DiagonalSplitterTransmitsHalfAndReflectsHalf()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var expected = Complex.One / Mathf.Sqrt2;

            // these directions are easy to screw up, these two are the directions
            // viewed from the mirror
            var in_dir = Directions.Subtract(dir, 1);
            var reflect_dir = Directions.Add(dir, 1);
            var transmit_dir = Directions.Reverse(in_dir);
            
            //                    S -- sends in direction Reverse(in_dir)
            //                    |
            //                   \|
            //                    \--(reflected)-->D1  
            //                 /  |\                 \
            //                /   |                    receives from direction Reverse(reflect_dir)
            //  mirror detail     v
            //                    d2 -- receives from direction Reverse(transmit_dir)
            //      in_dir
            //        ^
            //        |
            //        M--> reflect_dir
            //        |
            //        v
            //   transmit_dir
            
            var source = new BeamSource(Directions.Reverse(in_dir), Complex.One);
            var splitter = new BeamSplitter(dir);
            var dest1 = new PassiveAbsorber();
            var dest2 = new PassiveAbsorber();
            
            splitter.Connect(source, in_dir, 1);
            splitter.Connect(dest1, reflect_dir, 1);
            splitter.Connect(dest2, transmit_dir, 1);
            
            var passes = Calculator.Calculate([source, splitter, dest1, dest2]);
            
            Assert.True(passes < 3);
            
            Assert.Equal(expected, dest1.InPorts[Directions.Reverse(reflect_dir)].End, Tol);
            Assert.Equal(expected, dest2.InPorts[Directions.Reverse(transmit_dir)].End, Tol);         // transmitted beam arrives at dest the same way the original beam arrived at the splitter
        }
    }
    
    
    [Fact]
    public void ReverseDiagonalSplitterTransmitsHalfAndReflectsHalf()
    {
        foreach (var dir in Directions.DiagonalDirections)
        {
            var expected = Complex.One / Mathf.Sqrt2;

            // these directions are easy to screw up, these two are the directions
            // viewed from the mirror
            var in_dir = Directions.Subtract(dir, 1);
            var reflect_dir = Directions.Add(dir, 1);
            var transmit_dir = Directions.Reverse(in_dir);
            
            //                    S -- sends in direction Reverse(in_dir)
            //                    |
            //                   \|
            //                    \--(reflected)-->D1  
            //                 /  |\                 \
            //                /   |                    receives from direction Reverse(reflect_dir)
            //  mirror detail     v
            //                    d2 -- receives from direction Reverse(transmit_dir)
            //      in_dir
            //        ^
            //        |
            //        M--> reflect_dir
            //        |
            //        v
            //   transmit_dir
            
            var source = new BeamSource(Directions.Reverse(in_dir), Complex.One);
            var splitter = new BeamSplitter(Directions.Reverse(dir));
            var dest1 = new PassiveAbsorber();
            var dest2 = new PassiveAbsorber();
            
            splitter.Connect(source, in_dir, 1);
            splitter.Connect(dest1, reflect_dir, 1);
            splitter.Connect(dest2, transmit_dir, 1);
            
            var passes = Calculator.Calculate([source, splitter, dest1, dest2]);
            
            Assert.True(passes < 3);
            
            Assert.Equal(-expected, dest1.InPorts[Directions.Reverse(reflect_dir)].End, Tol);           // phase-shifted 180 degrees
            Assert.Equal(expected, dest2.InPorts[Directions.Reverse(transmit_dir)].End, Tol);           // transmitted beam arrives at dest the same way the original beam arrived at the splitter
        }
    }
}