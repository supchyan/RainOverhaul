using Microsoft.Xna.Framework;
using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Enums;
using RainOverhaul.Source.Managers;
using System;
using Terraria;
using Terraria.Graphics.Effects;
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
    private float Intensity                     { get; set; } = 0f;
    /// <summary>
    /// Transition property for vanilla rain mode shader.
    /// </summary>
    private float VanillaTransition             { get; set; } = 0f;
    /// <summary>
    /// Additional rain intensity used when player in Ocean or Jungle biomes.
    /// </summary>
    private float ExtraIntensity                { get; set; } = 0f;
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
    /// Rain intensity used in "rain world"'s rain system.
    /// </summary>
    private static float RW_Intensity       { get; set; } = 0f;
    /// <summary>
    /// Rain force value in RainWorld mode.
    /// </summary>
    private float RW_RainForce              { get; set; } = 0f;
    /// <summary>
    /// Quake strength value in RainWorld mode.
    /// </summary>
    private float RW_QuakeStrength          { get; set; } = 0f;
    /// <summary>
    /// Quake impulse value in RainWorld mode.
    /// </summary>
    private float RW_QuakeImpulse           { get; set; } = 0f;
    /// <summary>
    /// Quake impulse max value in RainWorld mode.
    /// </summary>
    private float RW_QuakeImpulseMaxValue   { get; } = 5.07f;
    /// <summary>
    /// Quake impulse min value in RainWorld mode.
    /// </summary>
    private float RW_QuakeImpulseMinValue   { get; } = 4.07f;

    // In-game world time values used in RainWorld mode
    private int CycleClearTimeEnd   { get; } = 51700; // time when quake cycle starts 
    private int CycleQuakeTimeEnd   { get; } = 53999; // time when rain cycle starts 
    private int CycleRainTimeEnd    { get; } = 16200; // time when clear cycle starts

    /// <summary>
    /// In loop smoothly increases RainForce value until it's limit.
    /// </summary>
    private void IncreaseRainForce()
    {
        RW_RainForce = RW_RainForce < 1f ? RW_RainForce + .01f : 1f;
    }
    /// <summary>
    /// In loop smoothly decreases RainForce value.
    /// </summary>
    private void DecreaseRainForce()
    {
        RW_RainForce = RW_RainForce > 0f ? RW_RainForce - .01f : 0f;
    }

    /// <summary>
    /// In loop smoothly increases QuakeImpulse value until it's limit.
    /// </summary>
    private void IncreaseQuakeImpulse()
    {
        // increase quake effect
        // += .1f and -= .1f important to fix glitchy transitions
        // between cycle states
        if (RW_QuakeImpulse < RW_QuakeImpulseMinValue)
        {
            RW_QuakeImpulse = RW_QuakeImpulseMinValue;
        }
        else if (RW_QuakeImpulse < RW_QuakeImpulseMaxValue)
        {
            RW_QuakeImpulse += .1f;
        }
        else
        {
            RW_QuakeImpulse -= .1f;
        }
    }
    /// <summary>
    /// In loop smoothly decreases QuakeImpulse value.
    /// </summary>
    private void DecreaseQuakeImpulse()
    {
        if (RW_QuakeImpulse > 0f)
        {
            RW_QuakeImpulse -= .1f;
        }
        else
        {
            RW_QuakeImpulse = 0f;
        }
    }
    /// <summary>
    /// Lowers QuakeImpulse whenever needed.
    /// </summary>
    private void SoftenQuakeImpulse()
    {
        if (RW_QuakeImpulse > ConfigClient.Instance.quakeImpulseInSafeArea)
        {
            RW_QuakeImpulse -= .1f;
        }
        else
        {
            RW_QuakeImpulse = ConfigClient.Instance.quakeImpulseInSafeArea;
        }
    }

    public override void OnEnterWorld()
    {
        base.OnEnterWorld();

        // enable shaders
        Filters.Scene.Activate("RainFilter");
        Filters.Scene.Activate("RainShake");

        if (Main.netMode == NetmodeID.Server && Main.maxRaining == 0)
        {
            Main.StartRain();
            Main.SyncRain();
        }
    }

    // Rain logic
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

        ExtraIntensity = Main.LocalPlayer.ZoneBeach || Main.LocalPlayer.ZoneJungle ? 1.4f : 1f;

        MaxRaining = MathHelper.Lerp(MaxRaining, Main.maxRaining, .01f);

        Intensity       = 550f * MaxRaining / (20f * 645f) * ConfigClient.Instance.rainIntensity;
        RW_Intensity    = 550f * MaxRaining / (20f * 645f) * 2.5f;

        if(!ConfigServer.Instance.isRainWorldMode)
        {
            // set neutral cycle state
            RW_CurrentCycle = CycleState.Clear;
            
            if(isPlayerUnderRain) 
            {
                VanillaTransition = VanillaTransition < 1f ? VanillaTransition + .005f : 1f;
            } 
            else
            {
                VanillaTransition = VanillaTransition > 0f ? VanillaTransition - .005f : 0f;
            }

            // corelate rain filter based on current max raining value
            Filters.Scene["RainFilter"].GetShader().UseOpacity(Intensity * VanillaTransition * ExtraIntensity)
                .UseIntensity(VanillaTransition * ConfigClient.Instance.blueFilterIntensity).UseProgress(-4f * Main.windSpeedCurrent);

            // disable quake effects
            Filters.Scene["RainShake"].GetShader()
                .UseOpacity(0f).UseIntensity(0f);
        } 
        else // when rain mode enabled:
        {
            // This controls sound effects in the rain system
            QuakeSoundCondition     = RW_CurrentCycle == CycleState.Quake;
            RainSoundCondition      = PlayerManager.IsPlayerInRainArea;
            DimRainSoundCondition   = PlayerManager.IsPlayerInRainArea && IsPlayerInSafePlace;

            if(isPlayerUnderRain && !Main.LocalPlayer.dead && !Main.LocalPlayer.immune && Main.LocalPlayer.active)
            {
                int fValue = (int)Math.Round(RW_Intensity * 20f);

                if(fValue > 0)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<RainSystemDebuff>(), 2);
                }
            }

            // corelate rain filter based on current RW mode rain force
            Filters.Scene["RainFilter"].GetShader()
                .UseOpacity(.1f * RW_RainForce)
                .UseIntensity(RW_RainForce).UseProgress(-4f * Main.windSpeedCurrent);

            // use quake effects in RW mode
            Filters.Scene["RainShake"].GetShader()
                .UseOpacity(RW_QuakeImpulse).UseIntensity(3.7f);

            // Custom rain behavior when in "RainWorld" mode
            switch(RW_CurrentCycle)
            {
                case CycleState.Clear:
                    Main.raining = false;

                    DecreaseQuakeImpulse();
                    DecreaseRainForce();

                    // disable vanilla rain dust
                    if(Main.maxRaining != 0f)
                    {
                        Main.maxRaining = 0f;
                    }

                    // Cycle state swap
                    if(Main.time >= CycleClearTimeEnd && Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Quake;
                    }
                    if(Main.time < CycleRainTimeEnd && !Main.IsItDay())
                    {
                        RW_CurrentCycle = CycleState.Rain;
                    }

                break;

                case CycleState.Quake:
                    Main.raining = false;
                    
                    if (Main.maxRaining != 0f)
                    {
                        Main.maxRaining = 0f;
                    }

                    RW_QuakeStrength = 1f + (float)(Main.time - CycleClearTimeEnd) / 
                                            (float)(Main.dayLength - CycleClearTimeEnd);

                    bool quakeCondition =   !Main.LocalPlayer.ZoneUnderworldHeight &&
                                            !Main.LocalPlayer.ZoneNormalSpace &&
                                            !Main.LocalPlayer.ZoneSandstorm &&
                                            !Main.LocalPlayer.ZoneSnow;

                    if (quakeCondition)
                    {
                        RW_QuakeImpulse = (float)Math.Sin(
                                MathHelper.ToRadians((float)(Main.time - CycleClearTimeEnd) / 2f)
                            ) * RW_QuakeStrength;
                    }
                    else // if player left certain biome, stop the quake
                    {
                        DecreaseQuakeImpulse();
                    }

                    DecreaseRainForce();

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
                    if (!Main.raining)
                    {
                        // force rain to start
                        Main.StartRain();
                    }

                    Main.windSpeedCurrent = .1f * MathF.Sin(.05f * (float)Main.time);

                    // if player in rain zone
                    if (PlayerManager.IsPlayerInRainArea)
                    {
                        if (!IsPlayerInSafePlace)
                        {
                            IncreaseRainForce();
                            IncreaseQuakeImpulse();
                        }
                        else
                        {
                            DecreaseRainForce();
                            SoftenQuakeImpulse();
                        }
                    }
                    else // !isPlayerInRainArea
                    {
                        DecreaseQuakeImpulse();
                        DecreaseRainForce();
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