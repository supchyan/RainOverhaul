using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Managers;
/// <summary>
/// Works with rain related effects.
/// </summary>
public class FilterManager
{
    public class RainFilter
    {
        public static void Load()
        {
            Ref<Effect> RainRef = new(ModContent.Request<Effect>(
                "RainOverhaul/Content/Effects/RainFilter", AssetRequestMode.ImmediateLoad).Value);

            Filters.Scene["RainFilter"] = new Filter(
                new ScreenShaderData(RainRef, "RainFilter"), EffectPriority.VeryHigh);

            Filters.Scene["RainFilter"].Load();
        }
        public static void Activate()
        {
            Filters.Scene.Activate("RainFilter");
        }
        public static void Deactivate()
        {
            Filters.Scene.Deactivate("RainFilter");
        }
        public static void Update(float rainIntensity, float blueIntensity, float rainDirection)
        {
            Filters.Scene["RainFilter"].GetShader().UseOpacity(rainIntensity)
                .UseIntensity(blueIntensity).UseProgress(rainDirection);
        }
    }
    public class AdditionalRainFilter
    {
        public static void Load()
        {
            Ref<Effect> RainRef = new(ModContent.Request<Effect>(
                "RainOverhaul/Content/Effects/RainFilter", AssetRequestMode.ImmediateLoad).Value);

            Filters.Scene["AdditionalRainFilter"] = new Filter(
                new ScreenShaderData(RainRef, "RainFilter"), EffectPriority.VeryHigh);

            Filters.Scene["AdditionalRainFilter"].Load();
        }
        public static void Activate()
        {
            Filters.Scene.Activate("AdditionalRainFilter");
        }
        public static void Deactivate()
        {
            Filters.Scene.Deactivate("AdditionalRainFilter");
        }
        public static void Update(float rainIntensity, float blueIntensity, float rainDirection)
        {
            Filters.Scene["AdditionalRainFilter"].GetShader().UseOpacity(rainIntensity)
                .UseIntensity(blueIntensity).UseProgress(rainDirection);
        }
    }
    public class QuakeFilter
    {
        public static void Load()
        {
            Ref<Effect> ShakeRef = new(ModContent.Request<Effect>(
                "RainOverhaul/Content/Effects/RainShake", AssetRequestMode.ImmediateLoad).Value);

            Filters.Scene["RainShake"] = new Filter(
                new ScreenShaderData(ShakeRef, "RainShake"), EffectPriority.VeryHigh);

            Filters.Scene["RainShake"].Load();
        }
        public static void Activate()
        {
            Filters.Scene.Activate("RainShake");
        }
        public static void Deactivate()
        {
            Filters.Scene.Deactivate("RainShake");
        }
        public static void Update(float quakeImpulse, float quakeOffset)
        {
            Filters.Scene["RainShake"].GetShader().UseOpacity(quakeImpulse)
                .UseIntensity(quakeOffset);
        }
    }
}
