using Microsoft.Xna.Framework;
using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Enums;
using RainOverhaul.Source.Graphics;
using RainOverhaul.Source.Managers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Systems; 
public class PlayerRainSystem : ModPlayer
{
    /// <summary>
    /// Current vanilla rain value.
    /// </summary>
    private float MaxRaining                    { get; set; } = 0f;
    /// <summary>
    /// Rain intensity used in vanilla rain system.
    /// </summary>
    private float RainIntensity                     { get; set; } = 0f;
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
    /// True whenever quake sound is intended.
    /// </summary>
    public static bool QuakeSoundCondition      { get; private set; } = false;
    /// <summary>
    /// True whenever rain sound is intended.
    /// </summary>
    public static bool RainSoundCondition       { get; private set; } = false;
    /// <summary>
    /// True whenever dim rain sound is intended.
    /// </summary>
    public static bool DimRainSoundCondition    { get; private set; } = false;
    /// <summary>
    /// Current cycle state in RainWorld mode.
    /// </summary>
    public static CycleState RW_CurrentCycle    { get; private set; } = CycleState.Clear;
    /// <summary>
    /// Rain intensity in Rain World mode.
    /// </summary>
    private static float RW_RainIntensity       { get; set; } = 0f;
    /// <summary>
    /// Rain transition offset in Rain World mode.
    /// </summary>
    private float RW_RainOffset                 { get; set; } = 0f;
    /// <summary>
    /// Quake intensity offset value in RainWorld mode.
    /// </summary>
    private float RW_QuakeOffset                { get; set; } = 0f;
    /// <summary>
    /// Quake intensity value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensity             { get; set; } = 0f;
    /// <summary>
    /// Quake intensity max value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensityMaxValue     { get; } = 5.07f;
    /// <summary>
    /// Quake intensity min value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensityMinValue     { get; } = 4.07f;

    // In-game world time values used in RainWorld mode
    private int CycleClearTimeEnd   { get; } = 51700; // time when quake cycle starts 
    private int CycleQuakeTimeEnd   { get; } = 53999; // time when rain cycle starts 
    private int CycleRainTimeEnd    { get; } = 16200; // time when clear cycle starts

    // Lerp offset for transitions between effects.
    private float GeneralTransition { get; } = .04f;

    /// <summary>
    /// In loop smoothly increases RW_RainForce value until it's limit.
    /// </summary>
    private void IncreaseRainOffset()
    {
        RW_RainOffset = MathHelper.Lerp(RW_RainOffset, 1f, GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly decreases RW_RainOffset value.
    /// </summary>
    private void DecreaseRainOffset()
    {
        RW_RainOffset = MathHelper.Lerp(RW_RainOffset, 0f, GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly increases RW_QuakeIntensity value until it's limit.
    /// </summary>
    private void IncreaseQuakeIntensity()
    {
        RW_QuakeIntensity = MathHelper.Lerp(RW_QuakeIntensity, RW_QuakeIntensityMaxValue, GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly decreases RW_QuakeIntensity value.
    /// </summary>
    private void DecreaseQuakeIntensity()
    {
        RW_QuakeIntensity = MathHelper.Lerp(RW_QuakeIntensity, 0f, GeneralTransition);
    }
    /// <summary>
    /// Lowers RW_QuakeIntensity whenever needed.
    /// </summary>
    private void SoftenQuakeIntensity()
    {
        RW_QuakeIntensity = MathHelper.Lerp(RW_QuakeIntensity, ConfigClient.Instance.quakeImpulseInSafeArea, GeneralTransition);
    }

    public override void OnEnterWorld()
    {
        base.OnEnterWorld();

        // activate effects
        EffectsController.RainEffect.Instance.Activate();
        EffectsController.AlternateRainEffect.Instance.Activate();
        EffectsController.QuakeEffect.Instance.Activate();

        if (Main.netMode == NetmodeID.Server && Main.maxRaining == 0)
        {
            Main.StartRain();
            Main.SyncRain();
        }
    }

    public override void PostUpdate()
    {
        // Sync rain with server all time 
        Main.SyncRain();

        var wallTile = Main.tile[Main.LocalPlayer.Center.ToTileCoordinates()];
        // Returns true if tile is a wall type
        bool hasWallCollision = wallTile.WallType > WallID.None;

        for (int y = Main.screenPosition.ToTileCoordinates().Y; y < Main.LocalPlayer.Top.ToTileCoordinates().Y; y++)
        {
            var solidTile = Main.tile[Main.LocalPlayer.Center.ToTileCoordinates().X, y];
            var isSolidTile = solidTile.HasTile && Main.tileSolid[solidTile.TileType];

            IsPlayerInSafePlace = isSolidTile || hasWallCollision;

            // skip further loop since found a proper tile
            if (IsPlayerInSafePlace)
            {
                break;
            }
        }

        bool isPlayerUnderRain = !IsPlayerInSafePlace && PlayerManager.IsPlayerInRainArea;

        ExtraRainIntensity = Main.LocalPlayer.ZoneBeach || Main.LocalPlayer.ZoneJungle ? 1.4f : 1f;

        MaxRaining = MathHelper.Lerp(MaxRaining, Main.maxRaining, .01f);

        RainIntensity       = MaxRaining * ConfigClient.Instance.rainIntensity;
        RW_RainIntensity    = MaxRaining * 2.5f;

        if(!ConfigServer.Instance.isRainWorldMode)
        {
            // set neutral cycle state
            RW_CurrentCycle = CycleState.Clear;
            
            if(isPlayerUnderRain) 
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
        } 
        else // when rain mode enabled:
        {
            // This controls sound effects in the rain system
            QuakeSoundCondition     = RW_CurrentCycle == CycleState.Quake;
            RainSoundCondition      = PlayerManager.IsPlayerInRainArea;
            DimRainSoundCondition   = PlayerManager.IsPlayerInRainArea && IsPlayerInSafePlace;

            if(isPlayerUnderRain && !Main.LocalPlayer.dead && !Main.LocalPlayer.immune && Main.LocalPlayer.active)
            {
                int fValue = (int)Math.Round(RW_RainIntensity * 20f);

                if(fValue > 0)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<RainSystemDebuff>(), 2);
                }
            }

            var rw_rainIntensity = RW_RainOffset * RW_RainIntensity;
            var rw_monochromeIntensity = RW_RainOffset;

            // update alternate rain effect
            EffectsController.AlternateRainEffect.Instance.SetParameter("RainIntensity", rw_rainIntensity);
            EffectsController.AlternateRainEffect.Instance.SetParameter("RainDirection", 2f * Main.windSpeedCurrent);

            // update rain effects
            EffectsController.RainEffect.Instance.SetParameter("RainIntensity", rw_rainIntensity);
            EffectsController.RainEffect.Instance.SetParameter("MonochromeIntensity", rw_monochromeIntensity);
            EffectsController.RainEffect.Instance.SetParameter("RainDirection", -2f * Main.windSpeedCurrent);

            // disable quake effects
            EffectsController.QuakeEffect.Instance.SetParameter("QuakeIntensity", RW_QuakeIntensity);

            // Custom rain behavior when in "RainWorld" mode
            switch (RW_CurrentCycle)
            {
                case CycleState.Clear:
                    DecreaseQuakeIntensity();
                    DecreaseRainOffset();

                    // disable vanilla rain dust
                    if (Main.maxRaining != 0f)
                    {
                        Main.maxRaining = 0f;
                    }

                    // don't change cycles during rift eclipse
                    if (PlayerManager.IsRiftEclipse)
                    {
                        break;
                    }

                    Main.raining = false;

                    // Cycle state swap
                    if (Main.time >= CycleClearTimeEnd && Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Quake;
                    }
                    if(Main.time < CycleRainTimeEnd && !Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Rain;
                    }

                break;

                case CycleState.Quake:
                    if (PlayerManager.IsRiftEclipse)
                    {
                        RW_CurrentCycle = CycleState.Clear;
                        break;
                    }

                    Main.raining = false;
                    
                    if (Main.maxRaining != 0f)
                    {
                        Main.maxRaining = 0f;
                    }

                    RW_QuakeOffset = 1f + (float)(Main.time - CycleClearTimeEnd) / 
                                          (float)(Main.dayLength - CycleClearTimeEnd);

                    if (PlayerManager.IsPlayerInQuakeArea)
                    {
                        RW_QuakeIntensity = (float)Math.Sin(
                                MathHelper.ToRadians((float)(Main.time - CycleClearTimeEnd) / 2f)
                            ) * RW_QuakeOffset;
                    }
                    else // if player left certain biome, stop the quake
                    {
                        DecreaseQuakeIntensity();
                    }

                    DecreaseRainOffset();

                    // Cycle state swap
                    if (Main.time < CycleClearTimeEnd && Main.IsItDay() ||
                        Main.time >= CycleRainTimeEnd && !Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Clear;
                    }
                    
                    if(Main.time < CycleRainTimeEnd && !Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Rain;
                    }

                break;

                case CycleState.Rain:
                    if (PlayerManager.IsRiftEclipse)
                    {
                        RW_CurrentCycle = CycleState.Clear;
                        break;
                    }

                    if (!Main.raining)
                    {
                        // force rain to start
                        Main.StartRain();
                    }

                    var time = .05f * (float)Main.time;

                    var sin  = MathF.Sin(time);
                    var sin2 = MathF.Pow(sin, 2);

                    var cos  = MathF.Cos(time);
                    var cos2 = MathF.Pow(cos, 2);

                    Main.windSpeedCurrent = .1f * (sin2 * cos - cos2 * sin);

                    // if player in rain zone
                    if (PlayerManager.IsPlayerInRainArea)
                    {
                        if (!IsPlayerInSafePlace)
                        {
                            IncreaseRainOffset();
                            IncreaseQuakeIntensity();
                        }
                        else
                        {
                            DecreaseRainOffset();
                            SoftenQuakeIntensity();
                        }
                    }
                    else // !isPlayerInRainArea
                    {
                        DecreaseQuakeIntensity();
                        DecreaseRainOffset();
                    }

                    // force limit rain value
                    // [don't use 1.0f, since it breaks the game]
                    if (Main.maxRaining != .97f)
                    {
                        Main.maxRaining = .97f;
                    }

                    // Cycle state swap
                    if (Main.time >= CycleRainTimeEnd && !Main.IsItDay() || 
                        Main.time < CycleClearTimeEnd && Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Clear;
                    }

                    if (Main.time >= CycleClearTimeEnd && Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Quake;
                    }

                break;
            }
        }
    }
    // Damage control of players under the rain 
    public override void UpdateBadLifeRegen()
    {
        var rainIntensity = 550f * Main.maxRaining / (20f * 645f) * 2.5f;

        int fValue = (int)Math.Round(rainIntensity * 20f);

        if (Main.LocalPlayer.HasBuff<RainSystemDebuff>())
        {
            Main.LocalPlayer.lifeRegen -= 200 * fValue;
        }
    }
}