using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using RainOverhaul.Source.Helpers;
using RainOverhaul.Source.Configs;
using Terraria.UI;
using Terraria.ModLoader.Config;

namespace RainOverhaul.Source.Managers;

internal class UIManager
{
    /// <summary>
    /// Draws a text.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch reference.</param>
    /// <param name="text">Text to be drawn.</param>
    /// <param name="position">Draw position.</param>
    /// <param name="scale">Text size. (Vanilla element label use 0.3f)</param>
    public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = .4f)
    {
        var font = FontAssets.DeathText.Value;

        ChatManager.DrawColorCodedStringWithShadow(
            spriteBatch, font, text, position, color, 0f,
            default, scale * Vector2.One);
    }
    /// <summary>
    /// Returns text size as a Vector2.
    /// </summary>
    /// <param name="text">Text reference.</param>
    /// <param name="scale">Text size reference.</param>
    public static Vector2 GetTextSize(string text, float scale = .4f)
    {
        var font = FontAssets.DeathText.Value;
        return font.MeasureString(text) * scale;
    }
    /// <summary>
    /// Draws a tooltip near the cursor.
    /// </summary>
    /// <param name="elementReference">UIElement reference used to prevent tooltip go beyond the screen.</param>
    /// <param name="spriteBatch">SpriteBatch reference.</param>
    /// <param name="text">Text to be drawn.</param>
    public static void DrawTooltip(UIElement elementReference, SpriteBatch spriteBatch, string text)
    {
        var tooltipPadding = 20f;
        DrawText(spriteBatch, text,
            new Vector2(Main.mouseX, Main.mouseY).Adds(tooltipPadding), Color.White);
    }
}
