using UnityEngine;

public static class FloatExtensions
{
    public static float Remap180(this float value)
    {
        return Mathf.Repeat(value + 180f, 360f) - 180f;
    }
}