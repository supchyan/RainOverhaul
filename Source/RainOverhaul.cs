using RainOverhaul.Source.Graphics;
using Terraria.ModLoader;


namespace RainOverhaul.Source;
public class RainOverhaul : Mod
{
    public override void Load()
    {
        EffectsController.MenuRainEffect.Instance.Load();
        EffectsController.RainEffect.Instance.Load();
        EffectsController.AlternateRainEffect.Instance.Load();
        EffectsController.QuakeEffect.Instance.Load();
    }
}