using UnityEngine;

public static class FloatExtensions
{
    public static float ApplyPercentChange(this float value, float percent)
    {
        float multiplier = 1.0f + Mathf.Abs(percent) / 100.0f;
        float result = value;
        if (percent > 0) result *= multiplier;
        else result /= multiplier;
        return result;
    } 
}
