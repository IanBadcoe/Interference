using Interference.Code.QuantumMaths.Components;

namespace Interference.LevelObject.QuantumComponent;

public partial class PassiveAbsorberComponent : QuantumComponent
{
    public static PassiveAbsorberComponent Create()
    {
        return PackedScenes.Instance.PassiveAbsorber.Instantiate<PassiveAbsorberComponent>();
    }
    
    public override void _Ready()
    {
        Component = new PassiveAbsorber();
        base._Ready();
    }

    protected override void TraceForConnections()
    {
        // A passive absorber will never output a nonzero beam value, so it has no need to go looking for connections.
        // If there's going to be a nonzero beam pointing into this, the component outputting that beam will find us.
        // Otherwise we would end up creating pointless zero amplitude beams between pairs of passive absorbers
    }
}