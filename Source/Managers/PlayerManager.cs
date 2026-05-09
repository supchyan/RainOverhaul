using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using ReLogic.Utilities;
using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Audio;

namespace RainOverhaul.Source.Managers;

public class PlayerManager : ModPlayer
{
    private SlotId SoundSlot { get; set; }
    /// <summary>
    /// True whenever player is in rain area.
    /// [TODO: add WOTG AoE snow covering support]
    /// </summary>
    public static bool IsPlayerInRainArea 
    { 
        get
        {
            return   Main.LocalPlayer.ZoneRain &&
                    !Main.LocalPlayer.ZoneNormalSpace &&
                    !Main.LocalPlayer.ZoneSandstorm &&
                    !Main.LocalPlayer.ZoneSnow;
        }
    }
    /// <summary>
    /// True whenever player is in quake area.
    /// </summary>
    public static bool IsPlayerInQuakeArea
    {
        get
        {
            return  !Main.LocalPlayer.ZoneUnderworldHeight &&
                    !Main.LocalPlayer.ZoneNormalSpace &&
                    !Main.LocalPlayer.ZoneSandstorm &&
                    !Main.LocalPlayer.ZoneSnow;
        }
    }
    // ---
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();

        if (ConfigServer.Instance.isRainWorldMode)
        {
            SoundEngine.PlaySound(SoundStyles.EnterSound with { Volume = 2f });
        }
    }
    public override bool PreKill(double damage, int hitDirection, bool pvp, 
        ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if(Player.HasBuff<RainSystemDebuff>())
        {
            damageSource = PlayerDeathReason.ByCustomReason($"{Player.name} {Language.GetTextValue("Mods.RainOverhaul.RainDeathReason")}");
        }

        // prevent vanilla sound from being playied
        // whenever the custom sound is enabled
        playSound = !ConfigClient.Instance.deathSoundInAmbientMode;

        return true;
    }
    public override void Kill(double damage, int hitDirection, 
        bool pvp, PlayerDeathReason damageSource)
    {
        // play rain world death sound if enabled in config or player is in RW mode
        if (ConfigClient.Instance.deathSoundInAmbientMode || ConfigServer.Instance.isRainWorldMode)
        {
            SoundSlot = SoundEngine.PlaySound(
                SoundStyles.DeathSound with { 
                    Volume = 1.2f, 
                    MaxInstances = 3, 
                    SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                }, 
                Player.Center
            );
        }
    }
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
		ModPacket packet = Mod.GetPacket();

		packet.Write(Main.LocalPlayer.statLife);
        packet.Write(Main.LocalPlayer.velocity.X);
        packet.Write(Main.LocalPlayer.velocity.Y);
		packet.Write((byte)Main.LocalPlayer.whoAmI);

		packet.Send(toWho, fromWho);
	}
    public override void CopyClientState(ModPlayer targetCopy)
    {
		PlayerManager clone = (PlayerManager)targetCopy;

		clone.Player.statLife = Main.LocalPlayer.statLife;
        clone.Player.velocity = Main.LocalPlayer.velocity;
	}
    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        PlayerManager clone = (PlayerManager)clientPlayer;

		if (Main.LocalPlayer.statLife != clone.Player.statLife || 
            Main.LocalPlayer.velocity != clone.Player.velocity) {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
	}
}