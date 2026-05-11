using RainOverhaul.Source.Graphics;
using Terraria.ModLoader;

namespace RainOverhaul.Source.RainSystem;

public class RainSystemEffectsLoader : ModSystem
{
    public override void OnWorldLoad()
    {
        EffectsController.RainEffect.Instance.Activate();
        EffectsController.AlternateRainEffect.Instance.Activate();
        EffectsController.QuakeEffect.Instance.Activate();
    }
    public override void OnWorldUnload()
    {
        EffectsController.RainEffect.Instance.Deactivate();
        EffectsController.AlternateRainEffect.Instance.Deactivate();
        EffectsController.QuakeEffect.Instance.Deactivate();
    }
}
