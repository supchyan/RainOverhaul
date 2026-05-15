using Microsoft.Xna.Framework;
using System;

namespace RainOverhaul.Source.Tools;

public static class MathTools
{
    /// <summary>
    /// Emulates Vector2.Lerp(), but for a `float` type.
    /// </summary>
    /// <param name="from">Float reference value./param>
    /// <param name="to">Target value.</param>
    /// <param name="weight">Lerp velocity.</param>
    public static float Lerp(float from, float to, float weight)
    {
        var value = Vector2.Lerp(new Vector2(from, 0f), new Vector2(to, 0f), weight).X;
        return MathF.Round(value, 4);
    }
    /// <summary>
    /// Converts seconds to ticks.
    /// </summary>
    /// <param name="s">Seconds value.</param>
    public static int ToTicks(float s)
    {
        // 1 s = 60 ticks
        return (int)MathF.Round(s * 60f);
    }
}