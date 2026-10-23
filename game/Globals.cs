using System.Numerics;
using Godot;
using Godot.Collections;
using Vector2 = Godot.Vector2;

namespace Interference;

public static class Globals
{
    /// <summary>
    /// Cell size of the grid on which the world and manipulatable objects are laid out
    /// </summary>
    public static int CellSize => 16;

    public static int MaxBeamDistance = 1280;

    public static (Node2D collider, Vector2 position, int colliderId, Rid rid, Vector2 normal, int shapeIndex) ParseCollisionResult(Dictionary result)
    {
        if (result.Count == 0)
        {
            return (null, Vector2.Zero, 0, new Rid(), Vector2.Zero, 0);
        }

        return (collider: result["collider"].As<Node2D>(), position: result["position"].AsVector2(), colliderId: result["collider_id"].AsInt32(), rid: result["rid"].AsRid(),
            normal: result["normal"].AsVector2(), shapeIndex: result["shape"].AsInt32());
    }
}