using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Graphics;

public class ROEffectsLoader : ModSystem
{
    private bool isLoaded = false;
    public override void Load()
    {
        base.Load();

        ROEffects.RainEffect.Instance.Load();
        ROEffects.AlternateRainEffect.Instance.Load();
        ROEffects.QuakeEffect.Instance.Load();

        isLoaded = true;
    }
    public override void OnWorldLoad()
    {
        ROEffects.RainEffect.Instance.Activate();
        ROEffects.AlternateRainEffect.Instance.Activate();
        ROEffects.QuakeEffect.Instance.Activate();
    }
    public override void OnWorldUnload()
    {
        ROEffects.RainEffect.Instance.Deactivate();
        ROEffects.AlternateRainEffect.Instance.Deactivate();
        ROEffects.QuakeEffect.Instance.Deactivate();
    }
}
