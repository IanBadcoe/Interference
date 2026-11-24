using System;
using System.Numerics;
using Godot;
using Interference.Code.QuantumMaths;
using MathNet.Numerics;

namespace Interference.LevelObject.QuantumComponent;

public partial class Detector : Triggerable
{
    public enum TriggerMode
    {
        AnySignal,
        MatchAmplitude,
        MatchAmplitudeAndPhase,
        AmplitudeAbove,
    }
    
    [Export] TriggerMode MyTriggerMode;

    Complex TriggerValue = new(0.5, 0);

    [Export]
    public float TriggerAmplitude
    {
        get => (float)TriggerValue.Magnitude;

        set => TriggerValue = Complex.FromPolarCoordinates(value, TriggerPhase);
    }

    [Export] 
    public float TriggerPhase
    {
        get => (float)TriggerValue.Phase;

        set => TriggerValue = Complex.FromPolarCoordinates(TriggerAmplitude, value);
    }
    
    [Export] public float MatchThreshold = 0.001f;
    
    [Export] public QuantumComponent Component;

    [Export] public bool IsTriggered;

    bool IsTriggered_Pending;

    public override void _Process(double delta)
    {
        base._Process(delta);

        CheckTriggered();
    }

    void CheckTriggered()
    {
        var observed = Component.Component.InAmplitudes[Component.Component.Direction];

        var triggered = MyTriggerMode switch
        {
            TriggerMode.AnySignal => observed.Magnitude > MatchThreshold,
            TriggerMode.MatchAmplitude => Mathf.Abs(TriggerAmplitude - observed.Magnitude) < MatchThreshold,
            TriggerMode.MatchAmplitudeAndPhase => Calculator.ComplexEquality(TriggerValue, observed, 0.001f),
            TriggerMode.AmplitudeAbove => observed.MagnitudeSquared() > TriggerAmplitude * TriggerAmplitude,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (triggered == IsTriggered_Pending) return;
        
        IsTriggered_Pending = triggered;

        // IsTriggered_Pending allows us to avoid duplicate signals by noting the real last-seen trigger state
        // but not have IsTriggered change before anyone is told
        //
        // if this is correctly working, then the effect will be that the detector detects reliably and without
        // duplicate notifications, and just takes 1s to respond to changes 
        
        // is it possible to unit-test this???

        GetTree().CreateTimer(1).Timeout += () =>
        {
            IsTriggered = IsTriggered_Pending;
            EmitSignal(Triggerable.SignalName.TriggerChanged, IsTriggered);
        };
    }
}