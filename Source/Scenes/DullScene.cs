using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Managers;
using RainOverhaul.Source.RainSystem.Cycles;
using Terraria;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Biomes; 

public class DullScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "");

    public override bool IsSceneEffectActive(Player player)
    {
        return CyclesSystem.RW_CurrentCycle == CycleState.Rain && 
            PlayerManager.IsPlayerInRainArea && 
            ConfigServer.Instance.isRainWorldMode &&
            ConfigClient.Instance.disableBiomesMusicDuringRain;
    }
}