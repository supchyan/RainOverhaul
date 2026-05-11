using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RainOverhaul.Source.Audio;
using RainOverhaul.Source.Math;
using RainOverhaul.Source.UI;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace RainOverhaul.Source.Configs;
public class CyclePositionElement : ConfigElement
{
    private float IndicatorSize     { get; } = 16;
    /// <summary>
    /// Top Left padding used by a label.
    /// [Also it used by a ValueSavedText]
    /// </summary>
    private float LabelPadding      { get; } = 10f;
    /// <summary>
    /// Element tooltip info retrieved from the localization file.
    /// </summary>
    private string Tooltip =>
            Language.GetTextValue(
                "Mods.RainOverhaul.Configs.ConfigClient.cycleIndicatorPosition.Tooltip");
    /// <summary>
    /// Text shown when indicator position value was modified.
    /// </summary>
    private string ValueUpdatedText =>
            Language.GetTextValue("Mods.RainOverhaul.Configs.ConfigClient.cycleIndicatorPosition.ValueUpdatedText");

    /// <summary>
    /// Color used by a `ValueUpdatedText`.
    /// </summary>
    private Color ValueSavedTextColor { get; set; } = Color.Transparent;
    /// <summary>
    /// Screen size value limited to 0...1 for width and height.
    /// </summary>
    private Vector2 ScreenSize
    { 
        get
        {
            float aspectRatio = (Main.screenWidth > Main.screenHeight) ?
                    (float)Main.screenHeight / (float)Main.screenWidth :
                    (float)Main.screenWidth / (float)Main.screenHeight ;

            float width  = (Main.screenWidth > Main.screenHeight) ? 1f : aspectRatio;
            float height = (Main.screenWidth > Main.screenHeight) ? aspectRatio : 1f;

            return new(width, height);
        }
    }
    /// <summary>
    /// Drawable screen limit value for it's width / height 
    /// used to be multiplied by a ScreenSize.
    /// </summary>
    private float ScreenMaxValue { get; } = 100;
    /// <summary>
    /// Target vector for IndicatorVector property.
    /// Used in Lerp() function to add indicator smoothness.
    /// </summary>
    private static Vector2 IndicatorVectorTarget { get; set; } = ConfigClient.Instance.cycleIndicatorPosition;
    /// <summary>
    /// Determines position of cycles indicator in range from 0...1 for each coord.
    /// </summary>
    private static Vector2 IndicatorVector { get; set; } = ConfigClient.Instance.cycleIndicatorPosition;

    // used to update indicator texture value
    private int indicatorFrame = 0;

    // used to update alpha channel of ValueSavedTextColor
    private byte valueSavedTextAlpha = 0;

    // used in SaveChanges() to properly handle user element changes
    private static bool isElementUpdatedAutomatically = false;

    public CyclePositionElement()
    {
        Width  = new(200f, 0f);
        Height = new(200f, 0f);

        // Assumes this is only possible when config file opened at the first time.
        // Read long description in SaveChanges() as well.
        if (IndicatorVector == Vector2.Zero && IndicatorVectorTarget == Vector2.Zero)
        {
            Reset();
        }
    }
    public override void OnActivate()
    {
        // set flag to true, since element just activated
        // so no user interacted with it
        isElementUpdatedAutomatically = true;
    }
    /// <summary>
    /// Resets indicator position to defaults.
    /// </summary>
    public static void Reset()
    {
        // something called update, so set it to false
        isElementUpdatedAutomatically = false;

        // reset values
        IndicatorVectorTarget   = new Vector2(.04f, .9f);
        IndicatorVector         = new Vector2(.04f, .9f);
    }
    /// <summary>
    /// Returns indicator texture relying on animation frames.
    /// </summary>
    private Texture2D GetIndicatorTexture()
    {
        indicatorFrame++;

        if (indicatorFrame < 60)
        {
            return UIAssets.CycleClearAsset.Value;
        }
        else if (indicatorFrame < 120)
        {
            return UIAssets.CycleQuakeAsset.Value;
        }
        else if (indicatorFrame < 180)
        {
            return UIAssets.CycleRainAsset.Value;
        }
        else
        {
            indicatorFrame = 0;
        }

        return UIAssets.CycleClearAsset.Value;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        var dims = GetDimensions();

        Rectangle elementRectangle = new(
            (int)dims.X,
            (int)dims.Y,
            (int)Width.Pixels,
            (int)Height.Pixels
        );

        // Draw element solid background
        spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            elementRectangle, Color.Black);

        // Draw element texture background above the solid
        spriteBatch.Draw(UIAssets.ConfigIndicatorBackgroundAsset.Value,
            elementRectangle, new Color(22,22,22));

        // define drawable screen rectangle
        RectangleEx screen = new()
        {
            Width  = (int)MathF.Round(ScreenMaxValue * ScreenSize.X),
            Height = (int)MathF.Round(ScreenMaxValue * ScreenSize.Y),
        };

        // set screen TopLeft corner
        screen.TopLeftPosition = new Position(
            dims.X + .5f *  (Width.Pixels - screen.Width),
            dims.Y + .5f * (Height.Pixels - screen.Height)
        );

        // drawable screen rectangle
        RectangleEx visibleScreen = new()
        {
            Width  = screen.Width + (int)(IndicatorSize),
            Height = screen.Height + (int)(IndicatorSize),
        };

        // set visibleScreen TopLeft corner
        visibleScreen.TopLeftPosition = new Position(
            screen.TopLeftPosition.X - (int)(.5f * IndicatorSize),
            screen.TopLeftPosition.Y - (int)(.5f * IndicatorSize)
        );

        // Draw visible screen area
        // [it's bigger than actual screen for UX purposes]
        spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            visibleScreen.ToRectangle(), Color.White);

        if (Main.mouseLeft && IsMouseHovering)
        {
            // update public vector data,
            // mapping private _IndicatorVector value
            // in a way like: x0 ... x1 -> 0.00 ... 1.00
            IndicatorVectorTarget = new Vector2(
                MathF.Round((float)(Main.mouseX - screen.TopLeftPosition.X) / screen.Width,  2),
                MathF.Round((float)(Main.mouseY - screen.TopLeftPosition.Y) / screen.Height, 2)
            );

            // set flag to false, since user touched the element
            // and config will be updated "from the element" locally
            isElementUpdatedAutomatically = false;
        }

        IndicatorVector = Vector2.Lerp(IndicatorVector, IndicatorVectorTarget, .08f);

        if (!Main.mouseLeft && ConfigClient.Instance.cycleIndicatorPosition != IndicatorVectorTarget)
        {
            SaveChanges();
        }

        // prevent vector overflow mapped values [0 ... 1]
        if (IndicatorVectorTarget.X > 1f)
        {
            IndicatorVectorTarget = new(1f, IndicatorVectorTarget.Y);
        }

        if (IndicatorVectorTarget.X < 0f)
        {
            IndicatorVectorTarget = new(0f, IndicatorVectorTarget.Y);
        }

        if (IndicatorVectorTarget.Y > 1f)
        {
            IndicatorVectorTarget = new(IndicatorVectorTarget.X, 1f);
        }

        if (IndicatorVectorTarget.Y < 0f)
        {
            IndicatorVectorTarget = new(IndicatorVectorTarget.X, 0f);
        }

        Rectangle indicatorRectangle = new(
            screen.TopLeftPosition.X + (int)(screen.Width  * IndicatorVector.X) - (int)(.5f * IndicatorSize),
            screen.TopLeftPosition.Y + (int)(screen.Height * IndicatorVector.Y) - (int)(.5f * IndicatorSize), 
            (int)IndicatorSize, 
            (int)IndicatorSize
        );

        Texture2D indicatorTexture = GetIndicatorTexture();

        // Draw cycle indicator
        spriteBatch.Draw(indicatorTexture, indicatorRectangle, Color.Red);

        if (visibleScreen.IsMouseInside()) // draw coords when mouse over screen view
        {
            var tooltipInfoX = (int)MathF.Round(Main.screenWidth  * IndicatorVector.X);
            var tooltipInfoY = (int)MathF.Round(Main.screenHeight * IndicatorVector.Y);

            UIManager.DrawTooltip(this, spriteBatch, $"({tooltipInfoX}, {tooltipInfoY})");

        }
        // draw element localized tooltip, when cursor hovering the border
        if (IsMouseHovering && Main.mouseY - dims.Y < 40f) 
        {
            UIManager.DrawTooltip(this, spriteBatch, Tooltip);
        }

        // Draw element localized label
        UIManager.DrawText(spriteBatch, Label,
            new Vector2(dims.X + LabelPadding, dims.Y + LabelPadding), Color.White, .3f);

        var valueSavedTextSize = UIManager.GetTextSize(ValueUpdatedText, .3f);

        // Draw element saved text
        UIManager.DrawText(spriteBatch, ValueUpdatedText,
            new Vector2(
                dims.X + .5f * (Width.Pixels - valueSavedTextSize.X), 
                dims.Y + Height.Pixels - valueSavedTextSize.Y - LabelPadding
                ), ValueSavedTextColor, .3f);

        // update ValueSavedTextColor alpha channel
        if (valueSavedTextAlpha > 0)
        {
            valueSavedTextAlpha--;
        }
        else
        {
            valueSavedTextAlpha = 0;
        }
        ValueSavedTextColor = new Color(
            0,
            valueSavedTextAlpha,
            0, 
            valueSavedTextAlpha
        );
    }
    /// <summary>
    /// Saves config element changes in specified property.
    /// </summary>
    private void SaveChanges()
    {
        // [!READ THIS]
        // Assumes that user cannot reach Vector2.Zero manually.
        // This is fix of "first config load" bug when values of this element equals to Vector2.Zero.
        // Vector2.Zero forces vectors to reset their "default" values calling `Reset()`.
        // That's a crutches indeed, but visually looks ok for users, so live this this.
        // ...
        // Somehow I've no idea how to make custom config GUI better,
        // so let me know, if you know!
        if (IndicatorVectorTarget == Vector2.Zero)
        {
            IndicatorVectorTarget = new Vector2(.0001f, .0001f);
        }
        ConfigClient.Instance.cycleIndicatorPosition = IndicatorVectorTarget;

        // write value into a config property
        var result = ConfigClient.Instance.SaveChanges();

        if (result == ConfigSaveResult.Success)
        {
            // play saved sound only when value updated by the user
            // and not by the config itself
            if (!isElementUpdatedAutomatically)
            {
                // set ValueSavedTextColor visible
                valueSavedTextAlpha = 255;

                // play some rainworld sound
                SoundEngine.PlaySound(ROSoundStyle.DeathSound with { Volume = 3f });
            }
        }
    }
}