using RainOverhaul.Source.Audio;
using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Graphics;
using RainOverhaul.Source.Managers;
using RainOverhaul.Source.Tools;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RainOverhaul.Source.RainSystem.Cycles;

public class CyclesSystem
{
    /// <summary>
    /// Current cycle state in RainWorld mode.
    /// </summary>
    public static CycleState RW_CurrentCycle { get; private set; } = CycleState.Clear;
    /// <summary>
    /// True whenever rain world mode have to lock Cycle to Rain.
    /// </summary>
    private static bool RW_IsInfiniteRain => ConfigServer.Instance.isRainWorldModeHasInfiniteRainCycle;
    /// <summary>
    /// Rain intensity in Rain World mode.
    /// </summary>
    private float RW_RainIntensity { get; set; } = 0f;
    /// <summary>
    /// Rain transition offset in Rain World mode.
    /// </summary>
    private float RW_RainOffset { get; set; } = 0f;
    /// <summary>
    /// Quake intensity value in RainWorld mode.
    /// </summary>
    private float RW_QuakeIntensity { get; set; } = 0f;
    /// <summary>
    /// Quake intensity value in RainWorld mode.
    /// </summary>
    private float RW_QuakeOffset { get; set; } = 0f;
    /// <summary>
    /// Quake intensity max value in RainWorld mode.
    /// </summary>
    private static float RW_QuakeIntensityMaxValue => 5f;
    /// <summary>
    /// Quake intensity min value in RainWorld mode.
    /// </summary>
    private static float RW_QuakeIntensityMinValue => ConfigClient.Instance.quakeIntensityInSafeArea;

    /// <summary>
    /// Lerp offset for transitions between cycles' effects.
    /// </summary>
    private static float RW_GeneralTransition => .04f;

    /// <summary>
    /// Rain cycle begin time.
    /// </summary>
    private static double RW_BeginRainTime => 0;
    /// <summary>
    /// Rain cycle end time.
    /// </summary>
    private static double RW_EndRainTime => 27000;
    /// <summary>
    /// Rain Timer. Used in rain modulation during downpour cycle.
    /// </summary>
    private float  RW_RainTime { get; set; } = 0f;

    /// <summary>
    /// Modulation value during Rain Cycle to make rain feel more alive.
    /// </summary>
    private float RW_Modulation { get; set; } = 1f;

    /// <summary>
    /// Calculated intensity for the rain effect.
    /// </summary>
    private float RW_FinalRainIntensity  => RW_RainOffset  * RW_RainIntensity  * RW_Modulation;
    /// <summary>
    /// Calculated intensity for the quake effect.
    /// </summary>
    private float RW_FinalQuakeIntensity => RW_QuakeOffset * RW_QuakeIntensity * RW_Modulation;
    /// <summary>
    /// Intensity value for the monochrome effect.
    /// </summary>
    private float RW_MonochromeIntensity => RW_RainOffset;

    /// <summary>
    /// Equals to 0f during the Clear cycle, 1f otherwise.
    /// Affects final sound volume of ambient sound effects.
    /// </summary>
    private float SoundOffset { get; set; } = 0f;
    /// <summary>
    /// Calculated volume value for the rain ambience sounds.
    /// </summary>
    private float AmbienceVolume => RW_Modulation * RW_RainOffset * SoundOffset;
    /// <summary>
    /// Calculated volume value for the dimmed rain ambience sounds.
    /// </summary>
    private float DimAmbienceVolume => RW_Modulation * (1f - RW_RainOffset) * SoundOffset;
    private SlotId AmbienceSlot { get; set; }
    private SlotId DimAmbienceSlot { get; set; }

    private void ModulateRainAmbience()
    {
        SoundEngine.TryGetActiveSound(AmbienceSlot, out ActiveSound result);

        if (result == null)
        {
            AmbienceSlot = SoundEngine.PlaySound(ROSoundStyle.RainAmbience with { MaxInstances = 1, IsLooped = true, Type = SoundType.Ambient });
        }
        else
        {
            result.Volume = AmbienceVolume;
        }
    }
    private void ModulateDimRainAmbience()
    {
        SoundEngine.TryGetActiveSound(DimAmbienceSlot, out ActiveSound result);

        if (result == null)
        {
            DimAmbienceSlot = SoundEngine.PlaySound(ROSoundStyle.DimRainAmbience with { MaxInstances = 1, IsLooped = true, Type = SoundType.Ambient });
        }
        else
        {
            result.Volume = DimAmbienceVolume;
        }
    }
    /// <summary>
    /// Smoothly increases SoundOffset value.
    /// </summary>
    private void IncreaseSoundOffset()
    {
        SoundOffset = MathTools.Lerp(SoundOffset, 1f, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly nullifies SoundOffset value.
    /// </summary>
    private void DecreaseSoundOffset()
    {
        SoundOffset = MathTools.Lerp(SoundOffset, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly increases RW_RainForce value.
    /// </summary>
    private void IncreaseRainOffset()
    {
        RW_RainOffset = MathTools.Lerp(RW_RainOffset, 1f, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly nullifies RW_RainOffset value.
    /// </summary>
    private void DecreaseRainOffset()
    {
        RW_RainOffset = MathTools.Lerp(RW_RainOffset, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly increases RW_QuakeOffset value.
    /// </summary>
    private void IncreaseQuakeOffset()
    {
        RW_QuakeOffset = MathTools.Lerp(RW_QuakeOffset, RW_QuakeIntensityMaxValue, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly nullifies RW_QuakeOffset value.
    /// </summary>
    private void DecreaseQuakeOffset()
    {
        RW_QuakeOffset = MathTools.Lerp(RW_QuakeOffset, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// Lowers (softens) RW_QuakeOffset whenever needed.
    /// </summary>
    private void SoftenQuakeOffset()
    {
        RW_QuakeOffset = MathTools.Lerp(RW_QuakeOffset, RW_QuakeIntensityMinValue, RW_GeneralTransition);
    }
    /// <summary>
    /// Smoothly nullifies RW_Modulation value.
    /// </summary>
    private void DecreaseModulation()
    {
        RW_Modulation = MathTools.Lerp(RW_Modulation, 0f, RW_GeneralTransition);
    }
    /// <summary>
    /// Swaps to rain cycle if possible.
    /// </summary>
    private void TrySwapCycle()
    {
        if (RW_IsInfiniteRain)
        {
            RW_CurrentCycle = CycleState.Rain;
            return;
        }

        if (RW_BeginRainTime < Main.time && Main.time < RW_EndRainTime)
        {
            RW_CurrentCycle = Main.dayTime ? CycleState.Rain : CycleState.Clear;
        }
        else
        {
            RW_CurrentCycle = CycleState.Clear;
        }
    }
    /// <summary>
    /// Resets cycles, setting their state to Clear.
    /// </summary>
    public void ForceClear()
    {
        SoundEngine.TryGetActiveSound(AmbienceSlot, out ActiveSound _ambienceSlot);
        _ambienceSlot?.Stop();

        SoundEngine.TryGetActiveSound(DimAmbienceSlot, out ActiveSound _dimAmbienceSlot);
        _dimAmbienceSlot?.Stop();

        RW_CurrentCycle = CycleState.Clear;
    }
    /// <summary>
    /// Returns floating value for pseudo-chaotic rain/wind fluctuations.
    /// </summary>
    private float GetFloatingValue()
    {
        var rainTime = .05f * (float)Main.time;

        var sin = MathF.Sin(rainTime);
        var sin2 = MathF.Pow(sin, 2);

        var cos = MathF.Cos(rainTime);
        var cos2 = MathF.Pow(cos, 2);

        return .1f * (sin2 * cos - cos2 * sin);
    }
    private void ModulateRain()
    {
        // max modulation value in locked rain mode.
        if (RW_IsInfiniteRain)
        {
            RW_Modulation = 1f;
            return;
        }

        var e = 2.7182f; // just e
        var d = .001f;   // velocity to reach the limit of rain fluctuations from 0 to 1. [.05f is remomended peak]
        var f = .08f;    // fluctuations frequency
        var c = 1f;      // chaotic coefficient. has to be a whole uneven number.
        var t = RW_RainTime;

        var A = 1f - MathF.Pow(e, -.001f * t);
        var B = MathF.Pow(e, -d * t) * MathF.Sin(f * t);

        // override vanilla rw mode rain intensity
        RW_Modulation = A * (MathF.Pow(B, c) + 1f);

        RW_Modulation = RW_Modulation > 1f ? 1f : RW_Modulation;
        RW_Modulation = RW_Modulation < 0f ? 0f : RW_Modulation;

        if (RW_RainTime < 0f)
        {
            Main.NewText(Language.GetText("Mods.RainOverhaul.NegativeRainTimeException"));
        }
    }
    public void Update()
    {
        RW_RainIntensity  = MathTools.Lerp(RW_RainIntensity, Main.maxRaining * 2.5f, RW_GeneralTransition);
        RW_QuakeIntensity = MathTools.Lerp(RW_QuakeIntensity, Main.maxRaining, RW_GeneralTransition);

        RW_RainTime = (float)( RW_EndRainTime * (1f - (RW_EndRainTime - Main.time) / RW_EndRainTime) ); // 27000 ... 0 -> 1 ... 0 -> | 27000 * (1 - y/27000) | -> 0... 27000

        Main.cloudAlpha = RW_Modulation;
        Main.maxRaining = RW_Modulation;

        if (PlayerManager.IsPlayerUnderRain && PlayerManager.IsValidPlayer)
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<RainSystemDebuff>(), 2);
        }

        // update alternate rain effect
        ROEffects.AlternateRainEffect.Instance.SetParameter("RainIntensity", RW_FinalRainIntensity);
        ROEffects.AlternateRainEffect.Instance.SetParameter("RainDirection", 2f * Main.windSpeedCurrent);

        // update rain effects
        ROEffects.RainEffect.Instance.SetParameter("RainIntensity", RW_FinalRainIntensity);
        ROEffects.RainEffect.Instance.SetParameter("MonochromeIntensity", RW_MonochromeIntensity);
        ROEffects.RainEffect.Instance.SetParameter("RainDirection", -2f * Main.windSpeedCurrent);

        // update quake effects
        ROEffects.QuakeEffect.Instance.SetParameter("QuakeIntensity", RW_FinalQuakeIntensity);

        // update rain ambience sounds
        ModulateRainAmbience();
        ModulateDimRainAmbience();

        // Custom rain behavior when in "RainWorld" mode
        switch (RW_CurrentCycle)
        {
            case CycleState.Clear:
                DecreaseQuakeOffset();
                DecreaseRainOffset();
                DecreaseSoundOffset();
                DecreaseModulation();

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

                TrySwapCycle();

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

                ModulateRain();

                if (PlayerManager.IsPlayerInRainArea)
                {
                    if (PlayerManager.IsPlayerInSafePlace)
                    {
                        DecreaseRainOffset();
                        SoftenQuakeOffset();
                    }
                    else
                    {
                        IncreaseQuakeOffset();
                        IncreaseRainOffset();
                    }

                    IncreaseSoundOffset();
                }
                else // !isPlayerInRainArea
                {
                    DecreaseQuakeOffset();
                    DecreaseRainOffset();
                    DecreaseSoundOffset();
                }

                Main.windSpeedCurrent = .5f * RW_RainIntensity * GetFloatingValue();

                TrySwapCycle();

                break;
        }
    }
}
