using Microsoft.Xna.Framework;
using RainOverhaul.Source.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RainOverhaul.Source.TitleScreen;

public class TitleScreenRain : ModSystem
{
    private bool isContentLoaded = false;
    public override void Load()
    {
        On_Main.DoUpdate += Update;
    }
    public override void PostSetupContent()
    {
        isContentLoaded = true;
    }
    private void Update(On_Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
    {
        orig(self, ref gameTime);

        // [TODO:]
        // Draw rain in main menu sometimes...
        //if (!Main.gameMenu || !isContentLoaded)
        //{
        //    EffectsController.MenuRainEffect.Instance.Deactivate();
        //    return;
        //}
        //else
        //{
        //    EffectsController.MenuRainEffect.Instance.Activate();
        //    EffectsController.MenuRainEffect.Instance.SetParameter("RainIntensity", 1f);
        //}
    }
}
