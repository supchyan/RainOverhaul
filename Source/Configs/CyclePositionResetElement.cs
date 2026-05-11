using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RainOverhaul.Source.UI;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace RainOverhaul.Source.Configs;
public class CyclePositionResetElement : ConfigElement
{
    private string Tooltip => 
        Language.GetTextValue(
            "Mods.RainOverhaul.Configs.ConfigClient.cycleIndicatorPositionReset.Tooltip");

    public CyclePositionResetElement()
    {
        Width  = new(200f, 0f);
        Height = new(40f, 0f);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        var dims = GetDimensions();

        var textScale = .3f;
        var textSize = UIManager.GetTextSize(Label, textScale);

        var textCenter = new Vector2(
            dims.X + .5f * (Width.Pixels - textSize.X),
            dims.Y + .5f * (Height.Pixels - textSize.Y)
        );

        Rectangle elementRectangle = new(
            (int)dims.X,
            (int)dims.Y,
            (int)Width.Pixels,
            (int)Height.Pixels
        );

        // Draw element background
        spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            elementRectangle, IsMouseHovering ? new Color(22, 22, 22) : Color.Black);
        
        // Draw element text
        UIManager.DrawText(spriteBatch, Label, textCenter, Color.White, textScale);

        // draw element localized tooltip
        if (IsMouseHovering)
        {
            UIManager.DrawTooltip(this, spriteBatch, Tooltip);
        }
    }
    public override void LeftClick(UIMouseEvent evt)
    {
        CyclePositionElement.Reset();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }
}