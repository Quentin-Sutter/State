using UnityEngine;

public static class Vector2Extensions
{
    public static float SignedAngleZ(this Vector2 dir)
    {
        if (dir.sqrMagnitude <= Mathf.Epsilon)
        {
            return 0f;
        }

        return Vector2.SignedAngle(Vector2.up, dir);
    }
}
