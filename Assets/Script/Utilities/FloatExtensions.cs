using UnityEngine;

public static class FloatExtensions
{
    private const float PercentageDivider = 100f;

    public static float ApplyPercentChange(this float value, float percent)
    {
        if (Mathf.Approximately(percent, 0f))
        {
            return value;
        }

        var multiplier = 1f + Mathf.Abs(percent) / PercentageDivider;
        return percent > 0f ? value * multiplier : value / multiplier;
    }
}
