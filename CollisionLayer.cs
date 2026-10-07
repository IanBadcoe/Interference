using System;

namespace Interference;

/// <summary>
/// Allows accessing collision layers via our configured names. Must be manually kept in sync with the names in the
/// godot project settings
/// </summary>
[Flags]
public enum CollisionLayer
{
    PlayerMovement = 1,
    Usable = 2
}