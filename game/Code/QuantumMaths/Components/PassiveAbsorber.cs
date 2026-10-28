namespace Interference.Code.QuantumMaths.Components;

public class PassiveAbsorber(string name = "anon absorber")
    : Component(Directions.Direction.E, name) // Direction is irrelevant for this
{
    protected override void CalculateInner()
    {
        // do nothing
    }
}