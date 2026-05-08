using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;

namespace RainOverhaul; 
public class RainOverhaul : Mod {
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
    public override void Load() {
		Ref<Effect> RainRef = new Ref<Effect>(ModContent.Request<Effect>("RainOverhaul/Content/Effects/RainFilter", AssetRequestMode.ImmediateLoad).Value);
		Filters.Scene["RainFilter"] = new Filter(new ScreenShaderData(RainRef, "RainFilter"), EffectPriority.VeryHigh);
		Filters.Scene["RainFilter"].Load();

		Ref<Effect> ShakeRef = new Ref<Effect>(ModContent.Request<Effect>("RainOverhaul/Content/Effects/RainShake", AssetRequestMode.ImmediateLoad).Value);
		Filters.Scene["RainShake"] = new Filter(new ScreenShaderData(ShakeRef, "RainShake"), EffectPriority.VeryHigh);
		Filters.Scene["RainShake"].Load();
    }
}