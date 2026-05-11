using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RainOverhaul.Source.Configs;

public class ConfigClient : ModConfig
{
    public static ConfigClient Instance;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("AmbientModeSettings")]
    [DefaultValue(.1f)]
    [Range(0f, .5f)]
    public float rainIntensity; // ambient rain filter intensity (doesn't affect RW mode rain)

    [DefaultValue(.2f)]
    [Range(0f, 1f)]
    public float blueFilterIntensity; // ambient blue filter intensity (doesn't affect RW mode rain)

    [DefaultValue(false)]
    public bool deathSoundInAmbientMode; // whenever rain world death sound is enabled in ambient mode

    [Header("RainWorldModeSettings")]
    [DefaultValue(1f)]
    [Range(0f, 2f)]
    public float quakeImpulseInSafeArea; // RW mode quake intensity while in shelter

    [CustomModConfigItem(typeof(CyclePositionElement))]
    public Vector2 cycleIndicatorPosition;

    [CustomModConfigItem(typeof(CyclePositionResetElement))]
    public object cycleIndicatorPositionReset;
}