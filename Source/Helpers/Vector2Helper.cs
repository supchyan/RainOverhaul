using Microsoft.Xna.Framework;

namespace RainOverhaul.Source.Helpers;

public static class Vector2Helper
{
    /// <summary>
    /// Adds value to each vector coord.
    /// </summary>
    /// <param name="value">Value to add.</param>
    public static Vector2 Adds(this Vector2 vector, float value)
    {
        vector.X += value;
        vector.Y += value;

        return vector;
    }
    /// <summary>
    /// Multiplies each vector coord by a value specified.
    /// </summary>
    /// <param name="value">Value to add.</param>
    public static Vector2 Muls(this Vector2 vector, float value)
    {
        vector.X *= value;
        vector.Y *= value;

        return vector;
    }
}
