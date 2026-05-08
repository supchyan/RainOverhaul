using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace RainOverhaul;
public class RainOverhaul : Mod
{
    public override void Load() {
		Ref<Effect> RainRef = new Ref<Effect>(ModContent.Request<Effect>("RainOverhaul/Content/Effects/RainFilter", AssetRequestMode.ImmediateLoad).Value);
		Filters.Scene["RainFilter"] = new Filter(new ScreenShaderData(RainRef, "RainFilter"), EffectPriority.VeryHigh);
		Filters.Scene["RainFilter"].Load();

		Ref<Effect> ShakeRef = new Ref<Effect>(ModContent.Request<Effect>("RainOverhaul/Content/Effects/RainShake", AssetRequestMode.ImmediateLoad).Value);
		Filters.Scene["RainShake"] = new Filter(new ScreenShaderData(ShakeRef, "RainShake"), EffectPriority.VeryHigh);
		Filters.Scene["RainShake"].Load();
    }
}