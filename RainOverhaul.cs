using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace RainOverhaul {
	public class RainOverhaul : Mod {
		public override void Load() {
			if (Main.netMode != NetmodeID.Server) {

				Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("RainOverhaul/Content/RainFilter", AssetRequestMode.ImmediateLoad).Value);
				Filters.Scene["RainFilter"] = new Filter(new ScreenShaderData(screenRef, "RainFilter"), EffectPriority.VeryHigh);
				Filters.Scene["RainFilter"].Load();
			
			}
		}
	}
}