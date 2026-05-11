using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Managers;
using RainOverhaul.Source.RainSystem.Cycles;
using Terraria;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Biomes; 

public class DimRainBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/sDimRain");
    public override bool IsBiomeActive(Player player)
    {
        return CyclesSystem.RW_CurrentCycle == CycleState.Rain && 
            PlayerManager.IsPlayerInRainArea && PlayerManager.IsPlayerInSafePlace && ConfigServer.Instance.isRainWorldMode;
    }
}