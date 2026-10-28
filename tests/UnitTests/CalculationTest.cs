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
        var source = new BeamSource(Directions.Direction.E, amplitude);
        var target = new FullMirror(Directions.Direction.NW, Calculator.MirrorType.TwoSided);
        source.Connect(target, Directions.Direction.E, 1);

        Calculator.Calculate([source, target]);
        
        Assert.Equal(target.InPorts[Directions.Direction.W].Start, amplitude);
    }
    
    [Fact]
    public void AnLShapeConverges()
    {
        //             -----
        //               |
        //               |
        // [source]=-----/

        var amplitude = new Complex(3, 4);
        
        var source = new BeamSource(Directions.Direction.E, amplitude);
        var mirror = new FullMirror(Directions.Direction.NW, Calculator.MirrorType.TwoSided);
        var absorber = new PassiveAbsorber();
        
        source.Connect(mirror, Directions.Direction.E, 1);
        mirror.Connect(absorber, Directions.Direction.N, 1);

        Calculator.Calculate([source, mirror, absorber]);
        
        Assert.Equal(absorber.InPorts[Directions.Direction.S].Start, amplitude);
    }

    [Fact]
    public void ACrossOfMirrorsConverges()
    {
        //                            ----- mirror1
        //                              |
        //                              | sideways_b_dir 
        //                              v
        //                               
        //                              ^
        //               sideways_f_dir |
        //                              |
        //            f_dir    b_dir   \                       |
        // [source]=------->   <------- \ ------->   <---------| mirror2
        //                               \  f_dir      b_dir   |
        //                           /  |
        //                          /   | sideways_b_dir
        //              splitter_dir    v
        //
        //                              ^
        //                              | sideways_f_dir
        //                              | 
        //                            ----- mirror3
        
        foreach (var splitter_type in new []
                     { Calculator.BeamSplitterType.HalfSilveredMirror, Calculator.BeamSplitterType.PrismCube })
        {
            foreach (var splitter_dir in Directions.DiagonalDirections)
            {
                var f_dir = Directions.Add(splitter_dir, 3);
                var b_dir = Directions.Subtract(splitter_dir, 1);
                var sideways_f_dir = Directions.Subtract(splitter_dir, 3);
                var sideways_b_dir = Directions.Add(splitter_dir, 1);

                var source = new BeamSource(f_dir, Complex.One);
                var splitter = new BeamSplitter(splitter_dir, splitter_type);
                var mirror1 = new FullMirror(sideways_b_dir, Calculator.MirrorType.OneSided, "m1");
                var mirror2 = new FullMirror(b_dir, Calculator.MirrorType.OneSided, "m2");
                var mirror3 = new FullMirror(sideways_f_dir, Calculator.MirrorType.OneSided, "m3");

                splitter.Connect(source, b_dir, 1);
                splitter.Connect(mirror1, sideways_f_dir, 1);
                splitter.Connect(mirror2, f_dir, 1);
                splitter.Connect(mirror3, sideways_b_dir, 1);

                var passes = Calculator.Calculate([source, splitter, mirror1, mirror2, mirror3]);

                Assert.True(passes < Calculator.MAX_PASSES);

                // this is the only way out, if we have converged, we need ultimately to have come this way
                Assert.Equal(1, source.InAmplitudes[f_dir].Magnitude, 1e-3);
            }
        }
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

        foreach (var splitter_dir in Directions.DiagonalDirections)
        {
            var reflect_dir = Directions.Subtract(splitter_dir, 1);
            var transmit_dir = Directions.Subtract(splitter_dir, 3);
            var loop_dir = Directions.Add(splitter_dir, 3);
            var in_dir = Directions.Add(splitter_dir, 1);

            var source = new BeamSource(transmit_dir, Complex.One);
            var splitter = new BeamSplitter(splitter_dir, Calculator.BeamSplitterType.HalfSilveredMirror);
            var m1 = new FullMirror(Directions.Add(splitter_dir, 2), Calculator.MirrorType.OneSided);
            var m2 = new FullMirror(splitter_dir, Calculator.MirrorType.OneSided);
            var m3 = new FullMirror(Directions.Add(splitter_dir, -2), Calculator.MirrorType.OneSided);
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
                var splitter = new BeamSplitter(Directions.Add(dir, splitter_rot), Calculator.BeamSplitterType.HalfSilveredMirror);
                
                foreach(var m1_flip in new [] { 0, 4 })
                {
                    var m1 = new FullMirror(Directions.Add(dir, 2 + m1_flip), Calculator.MirrorType.TwoSided);
                    
                    foreach(var m2_flip in new [] { 0, 4 })
                    {
                        var m2 = new FullMirror(Directions.Add(dir, m2_flip), Calculator.MirrorType.TwoSided);
                        
                        foreach(var m3_flip in new [] { 0, 4 })
                        {
                            var m3 = new FullMirror(Directions.Add(dir, m3_flip - 2), Calculator.MirrorType.TwoSided);

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