using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RainOverhaul.Source.Configs;

public class ConfigServer : ModConfig
{
    public static ConfigServer Instance;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("RainWorldModeSettings")]
    [DefaultValue(false)]
    public bool isRainWorldMode; // whenever RW mode is enabled (cycle system, rain damage and etc.)

    [DefaultValue(true)]
    public bool rainWorldAffectsTownNPSs; // whenever RW mode rain damages town npcs

    [DefaultValue(true)]
    public bool rainWorldAffectsOtherNPCs; // whenever RW mode rain damages npcs beyond town flag.
}