using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Managers;
using RainOverhaul.Source.RainSystem.Cycles;
using Terraria;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Biomes; 

public class QuakeBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "");
    public override bool IsBiomeActive(Player player)
    {
        return CyclesSystem.RW_CurrentCycle == CycleState.Quake && 
            PlayerManager.IsPlayerInQuakeArea && ConfigServer.Instance.isRainWorldMode;
    }
}