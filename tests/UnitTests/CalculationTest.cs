using System.Numerics;
using Interference.Code.QuantumMaths;
using Interference.Code.QuantumMaths.Components;

namespace UnitTests;

public class CalculationTest
{
    // if we converge to one part in a thousand on component inputs
    // there's a leeway for a little more imprecision on component outputs
    readonly ComplexToleranceComparer Tol = new(ComplexToleranceComparer.DEFAULT_TOLERANCE * 1.1f);
        
    [Fact]
    public void ASingleBeamConnectingTwoComponentsConverges()
    {
        // [source]=-----\
        var amplitude = new Complex(3, 4);
        var source = new BeamSource(Directions.Direction.Right, amplitude);
        var target = new FullMirror(Directions.Direction.UpAndLeft, Directions.MirrorType.TwoSided);
        source.Connect(target, Directions.Direction.Right, 1);

        Calculator.Calculate([source, target]);
        
        Assert.Equal(target.InPorts[Directions.Direction.Left].Start, amplitude);
    }
    
    [Fact]
    public void ALShapeConverges()
    {
        //             -----
        //               |
        //               |
        // [source]=-----/

        var amplitude = new Complex(3, 4);
        
        var source = new BeamSource(Directions.Direction.Right, amplitude);
        var mirror = new FullMirror(Directions.Direction.UpAndLeft, Directions.MirrorType.TwoSided);
        var absorber = new PassiveAbsorber();
        
        source.Connect(mirror, Directions.Direction.Right, 1);
        mirror.Connect(absorber, Directions.Direction.Up, 1);

        Calculator.Calculate([source, mirror, absorber]);
        
        Assert.Equal(absorber.InPorts[Directions.Direction.Down].Start, amplitude);
    }

    [Fact]
    public void ACrossOfMirrorsConverges()
    {
        //             -----
        //               |
        //               |      |
        // [source]=-----\------|
        //               |      |
        //               |
        //             -----
    }
    
    [Fact]
    public void ASimpleLoopOfMirrorsConverges()
    {
        //            d
        //            ^ 
        //            |
        // source --> s --> m1
        //            ^     |
        //            |     v
        //           m3 <-- m2
        //
        // s = splitter, oriented to send half the source's signal into the loop
        // d = ultimate dest
        // m1-3 = mirrors oriented to form loop
        //
        // directions around components:
        //                                     d
        //                                     |
        //                                     v
        //                                     loop_dir
        //             splitter normal = dir
        //                                  \  reflect_dir
        //                                   \ ^                         
        //                                    \|/                               \
        // source--> transmit_dir    in_dir <--s--> transmit_dir       in_dir <--m1
        //                                    /|                                /|\
        //                                     v                               / v
        //                                     loop_dir        normal = dir + 2  loop_dir
        //                          
        //                                     reflect_dir       normal = dir    reflect_dir
        //                                     ^                             \   ^
        //                                     |   normal = dir - 2           \  |
        //                                     | /                             \ |
        //                                    \|/                               \|/
        //                                    m3-->transmit_dir        in_dir -->m2
        //                                      \                               /

        foreach (var dir in Directions.DiagonalDirections)
        {
            var reflect_dir = Directions.Subtract(dir, 1);
            var transmit_dir = Directions.Subtract(dir, 3);
            var loop_dir = Directions.Add(dir, 3);
            var in_dir = Directions.Add(dir, 1);

            var source = new BeamSource(transmit_dir, Complex.One);
            var splitter = new BeamSplitter(dir);
            var m1 = new FullMirror(Directions.Add(dir, 2), Directions.MirrorType.OneSided);
            var m2 = new FullMirror(dir, Directions.MirrorType.OneSided);
            var m3 = new FullMirror(Directions.Add(dir, -2), Directions.MirrorType.OneSided);
            var dest = new PassiveAbsorber();

            splitter.Connect(source, in_dir, 1);
            splitter.Connect(m1, transmit_dir, 1);
            splitter.Connect(m3, loop_dir, 1);
            splitter.Connect(dest, reflect_dir, 1);

            m1.Connect(m2, loop_dir, 1);
            
            m2.Connect(m3, in_dir, 1);

            var passes = Calculator.Calculate([source, splitter, m1, m2, m3, dest]);
            
            Assert.True(passes < Calculator.MAX_PASSES);
            
            Assert.Equal(dest.InAmplitudes[loop_dir], Complex.One, Tol);
        }        
    }

    bool WithinTolerance(double x, double y, double tolerance)
    {
        return Math.Abs(x - y) <= tolerance;
    }
    
    [Fact]
    public void ComplexLoopsOfMirrorsConverge()
    {
        // as in ASimpleLoopOfMirrorsConverges except.
        // - all mirrors two-sided
        // - I will test all combos of flipped and unflipped mirrors
        //   (adding and removing phase-shifts)
        // - I will test all 4 diagonal orientations of the beam splitter
        //   (they all send something to the loop and might (eventually) to dest,
        //    although I imagine some combos will route all the signal back to the
        //    source...)
        //
        //            d
        //            ^ 
        //            |
        // source --> s --> m1
        //            ^     |
        //            |     v
        //           m3 <-- m2
        //
        // see ASimpleLoopOfMirrorsConverges for the names of the directions

        foreach (var dir in Directions.DiagonalDirections)          // rotate the whole apparatus
        {
            var reflect_dir = Directions.Subtract(dir, 1);
            var transmit_dir = Directions.Subtract(dir, 3);
            var loop_dir = Directions.Add(dir, 3);
            var in_dir = Directions.Add(dir, 1);

            var source = new BeamSource(transmit_dir, Complex.One);
            var dest = new PassiveAbsorber();

            foreach(var splitter_rot in new[] { 0, 2, 4, 6 })
            {
                var splitter = new BeamSplitter(Directions.Add(dir, splitter_rot));
                
                foreach(var m1_flip in new [] { 0, 4 })
                {
                    var m1 = new FullMirror(Directions.Add(dir, 2 + m1_flip), Directions.MirrorType.TwoSided);
                    
                    foreach(var m2_flip in new [] { 0, 4 })
                    {
                        var m2 = new FullMirror(Directions.Add(dir, m2_flip), Directions.MirrorType.TwoSided);
                        
                        foreach(var m3_flip in new [] { 0, 4 })
                        {
                            var m3 = new FullMirror(Directions.Add(dir, m3_flip - 2), Directions.MirrorType.TwoSided);

                            splitter.Connect(source, in_dir, 1);
                            splitter.Connect(m1, transmit_dir, 1);
                            splitter.Connect(m3, loop_dir, 1);
                            splitter.Connect(dest, reflect_dir, 1);

                            m1.Connect(m2, loop_dir, 1);

                            m2.Connect(m3, in_dir, 1);

                            var passes = Calculator.Calculate([source, splitter, m1, m2, m3, dest]);

                            Assert.True(passes < Calculator.MAX_PASSES);

                            // var dest_mag = dest.InAmplitudes[loop_dir].Magnitude;
                            // var source_mag = source.InAmplitudes[transmit_dir].Magnitude;

                            var dest_amp = dest.InAmplitudes[loop_dir];
                            var source_amp = source.InAmplitudes[transmit_dir];

                            // we expect an amplitude of +/- 1 coming out either towards dest, or back towards
                            // source, depending how the phase-shifts cancel out
                            Assert.True(WithinTolerance(Math.Abs(dest_amp.Real), 1, 1.1e-3) != WithinTolerance(Math.Abs(source_amp.Real), 1, 1.1e-3));
                        }
                    }
                }
            }        
        }        
    }
}