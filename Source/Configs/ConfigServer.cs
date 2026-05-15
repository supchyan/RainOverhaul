using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RainOverhaul.Source.Configs;

public class ConfigServer : ModConfig
{
    public static ConfigServer Instance;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("RainWorldModeSettings")]
    [DefaultValue(false)]
    public bool isRainWorldMode; // RW mode toggle (cycle system, rain damage and etc.)

    [DefaultValue(true)]
    public bool rainWorldAffectsTownNPSs; // if RW mode rain can damage town npcs

    [DefaultValue(true)]
    public bool rainWorldAffectsOtherNPCs; // if RW mode rain can damage npcs beyond town flag.

    [Header("MiscSettings")]
    [DefaultValue(true)]
    public bool isNoxusBossSupport; // WOTG support toggle

    [Header("DebugMenu")]
    [DefaultValue(false)]
    public bool isRainWorldModeHasInfiniteRainCycle; // if RW mode has an infinite rain cycle.

}