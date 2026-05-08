using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace RainOverhaul.Source.UI
{
    public class UIAssets
    {
        public static readonly Asset<Texture2D> CycleClearAsset = ModContent.Request<Texture2D>("RainOverhaul/Content/Textures/Cycles/cClear");
        public static readonly Asset<Texture2D> CycleQuakeAsset = ModContent.Request<Texture2D>("RainOverhaul/Content/Textures/Cycles/cQuake");
        public static readonly Asset<Texture2D> CycleRainAsset = ModContent.Request<Texture2D>("RainOverhaul/Content/Textures/Cycles/cRain");
        public static readonly Asset<Texture2D> ConfigIndicatorBackgroundAsset = ModContent.Request<Texture2D>("RainOverhaul/Content/Textures/ConfigIndicatorBackground");
    }
}
