using UnityEngine;

public static class Vector2Extensions
{
    public static float SignedAngleZ(this Vector2 dir)
    { 
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        return angle;
    }
}
