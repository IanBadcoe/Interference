using Godot;

namespace Interference.LevelObject;

public partial class Triggerable : Interference.LevelObject.Grabbable
{
    [Signal] public delegate void TriggerChangedEventHandler(bool triggered);
}