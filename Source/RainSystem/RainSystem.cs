using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Graphics;
using RainOverhaul.Source.Managers;
using RainOverhaul.Source.RainSystem.Cycles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ROMath = RainOverhaul.Source.Math;

namespace RainOverhaul.Source.RainSystem; 
public class RainSystem : ModPlayer
{
    /// <summary>
    /// Current vanilla rain value.
    /// </summary>
    private float MaxRaining                    { get; set; } = 0f;
    /// <summary>
    /// Rain intensity used in vanilla rain system.
    /// </summary>
    private float RainIntensity                 { get; set; } = 0f;
    /// <summary>
    /// Rain transition offset in Ambient mode.
    /// </summary>
    private float RainOffset                    { get; set; } = 0f;
    /// <summary>
    /// Additional rain intensity used when player in Ocean or Jungle biomes.
    /// </summary>
    private float ExtraRainIntensity            { get; set; } = 0f;
    /// <summary>
    /// True whenever player ain't affected by RainWorld mode system.
    /// </summary>
    private bool IsPlayerInSafePlace            { get; set; } = false;

    /// <summary>
    /// Rain world mode system instnace.
    /// </summary>
    CyclesSystem cyclesSystem = new();

    public override void PostUpdate()
    {
        // Sync rain with server all time 
        Main.SyncRain();

        ExtraRainIntensity = Main.LocalPlayer.ZoneBeach || Main.LocalPlayer.ZoneJungle ? 1.4f : 1f;

        MaxRaining = ROMath.MathEx.Lerp(MaxRaining, Main.maxRaining, .01f);

        RainIntensity       = MaxRaining * ConfigClient.Instance.rainIntensity;

        if(!ConfigServer.Instance.isRainWorldMode)
        {
            if(PlayerManager.IsPlayerUnderRain) 
            {
                RainOffset = RainOffset < 1f ? RainOffset + .005f : 1f;
            } 
            else
            {
                RainOffset = RainOffset > 0f ? RainOffset - .005f : 0f;
            }

            // ambient mode intensities
            var rainIntensity = RainOffset * RainIntensity * ExtraRainIntensity;
            var blueIntensity = RainOffset * ConfigClient.Instance.blueFilterIntensity;

            // About AlternateRainEffect and RainEffect:
            // AlternateRainEffect is more like effets background,
            // while RainEffect is the rain effect on foreground.
            // Applying one to another causes the good looking rain effect!

            // update alternate rain effect
            EffectsController.AlternateRainEffect.Instance.SetParameter("RainIntensity", .5f * rainIntensity);
            EffectsController.AlternateRainEffect.Instance.SetParameter("RainDirection", .4f * MathF.Sin(.05f * (float)Main.time));

            // update rain effects
            EffectsController.RainEffect.Instance.SetParameter("RainIntensity", rainIntensity);
            EffectsController.RainEffect.Instance.SetParameter("BlueIntensity", blueIntensity);
            EffectsController.RainEffect.Instance.SetParameter("MonochromeIntensity", 0f);
            EffectsController.RainEffect.Instance.SetParameter("RainDirection", -4f * Main.windSpeedCurrent);

            // disable quake effects
            EffectsController.QuakeEffect.Instance.SetParameter("QuakeIntensity", 0f);

            cyclesSystem.Reset();
        } 
        else // Rain World Mode is enabled vvv
        {
            cyclesSystem.Update();
        }
    }
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();

        if (Main.netMode == NetmodeID.Server && Main.maxRaining == 0)
        {
            Main.StartRain();
            Main.SyncRain();
        }
    }
}