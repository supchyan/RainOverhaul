using RainOverhaul.Source.Systems;
using Terraria;
using Terraria.ModLoader;
using RainOverhaul.Source.Configs;

namespace RainOverhaul.Source.Audio; 

/// <summary>
/// Contains custom biomes logic activating during RW mode cycles.
/// </summary>
public class RainWorldBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/sRain");

    public override bool IsBiomeActive(Player player)
    {
        return PlayerRainSystem.RainSoundCondition && ConfigServer.Instance.isRainWorldMode;
    }
}
public class DimRainWorldBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/sDimRain");
    public override bool IsBiomeActive(Player player)
    {
        return PlayerRainSystem.DimRainSoundCondition && ConfigServer.Instance.isRainWorldMode;
    }
}
public sealed class RainWorldMusicRegister : ILoadable
{
	public void Load(Mod mod)
    {
		MusicLoader.AddMusic(mod, "Content/Sounds/sRain");
        MusicLoader.AddMusic(mod, "Content/Sounds/sDimRain");
    }
	public void Unload() { }
}