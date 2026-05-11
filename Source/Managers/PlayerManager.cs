using RainOverhaul.Source.Audio;
using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Managers;

public class PlayerManager : ModPlayer
{
    /// <summary>
    /// True whenever WOTG's Rift Eclipse is happening.
    /// </summary>
    public static bool IsRiftEclipse
    {
        get
        {
            if (!ConfigServer.Instance.isNoxusBossSupport)
            {
                return false;
            }

            if (!ModLoader.TryGetMod("NoxusBoss", out Mod NoxusBoss))
            {
                return false;
            }

            NoxusBoss.TryFind<ModSystem>("RiftEclipseManagementSystem", out ModSystem RiftEclipseManagementSystem);

            bool RiftEclipseOngoing = (bool)RiftEclipseManagementSystem.GetType()
                .GetProperty("RiftEclipseOngoing").GetValue(null);

            return RiftEclipseOngoing;
        }
    }
    /// <summary>
    /// True whenever player is in safe place
    /// (i. e. cannot be affected by rain).
    /// </summary>
    public static bool IsPlayerInSafePlace { get; private set; }
    /// <summary>
    /// True whenever player is in rain area.
    /// </summary>
    public static bool IsPlayerInRainArea 
    { 
        get
        {
            return   Main.LocalPlayer.ZoneRain &&
                    !Main.LocalPlayer.ZoneNormalSpace &&
                    !Main.LocalPlayer.ZoneSandstorm &&
                    !Main.LocalPlayer.ZoneSnow &&
                    !IsRiftEclipse;
        }
    }
    /// <summary>
    /// True whenever player is under the rain.
    /// </summary>
    public static bool IsPlayerUnderRain => !IsPlayerInSafePlace && IsPlayerInRainArea;
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
                    !Main.LocalPlayer.ZoneSnow &&
                    !IsRiftEclipse;
        }
    }
    /// <summary>
    /// True whenever player is mortal and exist in world.
    /// </summary>
    public static bool IsValidPlayer => !Main.LocalPlayer.dead && !Main.LocalPlayer.immune && Main.LocalPlayer.active;
    private SlotId DeathSoundSlot { get; set; }
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();

        if (ConfigServer.Instance.isRainWorldMode)
        {
            SoundEngine.PlaySound(ROSoundStyle.EnterSound with { Volume = 2f });
        }
    }
    public override void PostUpdate()
    {
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
            DeathSoundSlot = SoundEngine.PlaySound(
                ROSoundStyle.DeathSound with { 
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