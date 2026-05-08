using Microsoft.Xna.Framework;

namespace RainOverhaul.Source.Helpers;

public static class MathHelper
{
    /// <summary>
    /// Emulates Vector2.Lerp(), but for a `float` type.
    /// </summary>
    /// <param name="from">Float reference value./param>
    /// <param name="to">Target value.</param>
    /// <param name="weight">Lerp velocity.</param>
    public static float Lerp(float from, float to, float weight)
    {
        return Vector2.Lerp(new Vector2(from, 0f), new Vector2(to, 0f), weight).X;
    }
}
