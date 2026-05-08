using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Buffs; 
/// <summary>
/// This debuff applies for players and NPCs server is in RW mode.
/// </summary>
public class RainSystemDebuff : ModBuff {
    public override string Texture => "RainOverhaul/Content/Textures/ShelterIcon";

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type]    = true;
        Main.buffNoSave[Type]           = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {   
        base.Update(player, ref buffIndex);

        // rain won't affect player using cute fishron mount
        if(player.mount._type != MountID.CuteFishron)
        {
            player.mount.Dismount(player);

            // applies every 2s
            player.jump = 0;
            player.jumpSpeedBoost = 0;
        }
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);

        npc.lifeRegen -= 120000 / 120;
    }
}