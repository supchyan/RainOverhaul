using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Graphics;
using RainOverhaul.Source.Managers;
using System;
using Terraria;
using Terraria.ModLoader;
using ROMath = RainOverhaul.Source.Math;

namespace RainOverhaul.Source.RainSystem.Cycles;

public class CyclesSystem
{
    /// <summary>
    /// Current cycle state in RainWorld mode.
    /// </summary>
    public static CycleState RW_CurrentCycle { get; private set; } = CycleState.Clear;
    /// <summary>
    /// Rain intensity in Rain World mode.
    /// </summary>
    private float RW_RainIntensity { get; set; } = 0f;
    /// <summary>
    /// Rain transition offset in Rain World mode.
    /// </summary>
    private float RW_RainOffset { get; set; } = 0f;
    /// <summary>
    /// Quake intensity offset value in RainWorld mode.
    /// </summary>
    private float RW_QuakeOffset { get; set; } = 0f;
    /// <summary>
    /// Quake intensity value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensity { get; set; } = 0f;
    /// <summary>
    /// Quake intensity max value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensityMaxValue { get; } = 5f;
    /// <summary>
    /// Quake intensity min value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensityMinValue { get; } = 4f;

    /// <summary>
    /// Lerp offset for transitions between cycles' effects.
    /// </summary>
    private float RW_GeneralTransition { get; } = .04f;

    // In-game world time values used in RainWorld mode
    private const int CycleClearTimeEnd = 51700; // time when quake cycle starts 
    private const int CycleQuakeTimeEnd = 53999; // time when rain cycle starts 
    private const int CycleRainTimeEnd  = 16200; // time when clear cycle starts

    /// <summary>
    /// In loop smoothly increases RW_RainForce value until it's limit.
    /// </summary>
    private void IncreaseRainOffset()
    {
        RW_RainOffset = ROMath.MathEx.Lerp(RW_RainOffset, 1f, RW_GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly decreases RW_RainOffset value.
    /// </summary>
    private void DecreaseRainOffset()
    {
        RW_RainOffset = ROMath.MathEx.Lerp(RW_RainOffset, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly increases RW_QuakeIntensity value until it's limit.
    /// </summary>
    private void IncreaseQuakeIntensity()
    {
        RW_QuakeIntensity = ROMath.MathEx.Lerp(RW_QuakeIntensity, RW_QuakeIntensityMaxValue, RW_GeneralTransition);
    }
    /// <summary>
    /// In loop smoothly decreases RW_QuakeIntensity value.
    /// </summary>
    private void DecreaseQuakeIntensity()
    {
        RW_QuakeIntensity = ROMath.MathEx.Lerp(RW_QuakeIntensity, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// Lowers RW_QuakeIntensity whenever needed.
    /// </summary>
    private void SoftenQuakeIntensity()
    {
        RW_QuakeIntensity = ROMath.MathEx.Lerp(RW_QuakeIntensity, ConfigClient.Instance.quakeIntensityInSafeArea, RW_GeneralTransition);
    }
    /// <summary>
    /// Swaps to clear cycle if possible.
    /// </summary>
    private void TrySwapToClearCycle()
    {
        if (Main.time < CycleClearTimeEnd && Main.IsItDay() ||
            Main.time >= CycleRainTimeEnd && !Main.IsItDay())
        {
            RW_CurrentCycle = CycleState.Clear;
        }
    }
    /// <summary>
    /// Swaps to quake cycle if possible.
    /// </summary>
    private void TrySwapToQuakeCycle()
    {
        if (Main.time >= CycleClearTimeEnd && Main.IsItDay())
        {
            RW_CurrentCycle = CycleState.Quake;
        }
    }
    /// <summary>
    /// Swaps to rain cycle if possible.
    /// </summary>
    private void TrySwapToRainCycle()
    {
        if (Main.time < CycleRainTimeEnd && !Main.IsItDay())
        {
            RW_CurrentCycle = CycleState.Rain;
        }
    }
    /// <summary>
    /// Resets cycles, setting their state to Clear.
    /// </summary>
    public void Reset()
    {
        RW_CurrentCycle = CycleState.Clear;
    }
    public void Update()
    {
        RW_RainIntensity = ROMath.MathEx.Lerp(RW_RainIntensity, Main.maxRaining * 2.5f, RW_GeneralTransition);

        if (PlayerManager.IsPlayerUnderRain && PlayerManager.IsValidPlayer)
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<RainSystemDebuff>(), 2);
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

                TrySwapToQuakeCycle();
                TrySwapToRainCycle();

                break;

            case CycleState.Quake:
                if (PlayerManager.IsRiftEclipse)
                {
                    RW_CurrentCycle = CycleState.Clear;
                    break;
                }

                TrySwapToClearCycle();
                TrySwapToRainCycle();

                Main.raining = false;

                if (Main.maxRaining != 0f)
                {
                    Main.maxRaining = 0f;
                }

                if (PlayerManager.IsPlayerInQuakeArea)
                {
                    var quakeTime = (float)(Main.time - CycleClearTimeEnd) / (float)(Main.dayLength - CycleClearTimeEnd);

                    if (quakeTime > 0) // detect quakeTime going to -999 when the night swaps the day, i.e. Main.time eq 54000 -> 0
                    {
                        // [TODO:]
                        // Add rain glimpses such in rain world.
                        var offset = RW_QuakeIntensityMinValue * MathF.Abs(MathF.Cos(2 * MathF.PI * quakeTime));
                        RW_QuakeIntensity = offset;
                    }
                }
                else // if player left certain biome, stop the quake
                {
                    DecreaseQuakeIntensity();
                }

                // stop rain effects
                DecreaseRainOffset();

                break;

            case CycleState.Rain:
                if (PlayerManager.IsRiftEclipse)
                {
                    RW_CurrentCycle = CycleState.Clear;
                    break;
                }

                TrySwapToClearCycle();
                TrySwapToQuakeCycle();

                if (!Main.raining)
                {
                    // force rain to start
                    Main.StartRain();
                }

                var rainTime = .05f * (float)Main.time;

                var sin = MathF.Sin(rainTime);
                var sin2 = MathF.Pow(sin, 2);

                var cos = MathF.Cos(rainTime);
                var cos2 = MathF.Pow(cos, 2);

                Main.windSpeedCurrent = .1f * (sin2 * cos - cos2 * sin);

                // if player in rain zone
                if (PlayerManager.IsPlayerInRainArea)
                {
                    if (PlayerManager.IsPlayerInSafePlace)
                    {
                        DecreaseRainOffset();
                        SoftenQuakeIntensity();
                    }
                    else
                    {
                        IncreaseRainOffset();
                        IncreaseQuakeIntensity();
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

                break;
        }
    }
}
