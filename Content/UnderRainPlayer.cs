using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using ReLogic.Utilities;

namespace RainOverhaul.Content {
    public class UnderRainPlayer:ModPlayer {
        private SoundStyle sDeath = new SoundStyle("RainOverhaul/Content/Sounds/sDeath");
        public SlotId TUUM;

        // Damage control of the players under the rain 
        public override void UpdateBadLifeRegen() {
            float rIntensity = 550*Main.maxRaining/(20.0f * 645.0f)*2.5f;
            int fValue = (int)Math.Round(rIntensity*20);
            if(Main.LocalPlayer.HasBuff<ShelterNotification>()) {
                Main.LocalPlayer.lifeRegen -= 200*fValue;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
            if(Player.HasBuff<ShelterNotification>()) damageSource = PlayerDeathReason.ByCustomReason(Player.name + " " + Language.GetTextValue("Mods.RainOverhaul.RainDeathReason"));
            return true;
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
            if (ModContent.GetInstance<RainConfig>().cCustomDeathSound)
            {
                TUUM = SoundEngine.PlaySound(sDeath with {Volume=1.2f,MaxInstances=3,SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest}, Player.Center);
            }
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			ModPacket packet = Mod.GetPacket();
			packet.Write(Main.LocalPlayer.statLife);
            packet.Write(Main.LocalPlayer.velocity.X);
            packet.Write(Main.LocalPlayer.velocity.Y);
			packet.Write((byte)Main.LocalPlayer.whoAmI);
			packet.Send(toWho, fromWho);
		}
        public override void CopyClientState(ModPlayer targetCopy) {
			UnderRainPlayer clone = (UnderRainPlayer)targetCopy;
			clone.Player.statLife = Main.LocalPlayer.statLife;
            clone.Player.velocity = Main.LocalPlayer.velocity;
		}
        public override void SendClientChanges(ModPlayer clientPlayer) {
			UnderRainPlayer clone = (UnderRainPlayer)clientPlayer;

			if (Main.LocalPlayer.statLife != clone.Player.statLife || Main.LocalPlayer.velocity != clone.Player.velocity) {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
		}
    }
}