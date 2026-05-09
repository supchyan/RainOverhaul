using RainOverhaul.Source.Managers;
using Terraria.ModLoader;


namespace RainOverhaul.Source;
public class RainOverhaul : Mod
{
    public override void Load()
    {
        FilterManager.RainFilter.Load();
        FilterManager.AdditionalRainFilter.Load();
        FilterManager.QuakeFilter.Load();
    }
}