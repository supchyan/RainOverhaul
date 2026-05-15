using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace RainOverhaul.Source.Buffs; 
/// <summary>
/// This debuff applies for players and NPCs server is in RW mode.
/// </summary>
public class RainSystemDebuff : ModBuff
{
    public override string Texture => "RainOverhaul/Content/Textures/shelterIcon";

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type]    = true;
        Main.buffNoSave[Type]           = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);

        if (Main.maxRaining > .4f)
        {
            // rain won't affect player using cute fishron mount
            if (player.mount._type != MountID.CuteFishron)
            {
                player.mount.Dismount(player);

                player.velocity += new Vector2(0, MathF.Abs(player.velocity.Y));
            }

            player.lifeRegen -= (int)MathF.Round(Main.maxRaining * (24000f / 120f));
        }
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);

        if (Main.maxRaining > .4f)
        {
            npc.lifeRegen -= (int)MathF.Round(Main.maxRaining * (120000f / 120f));
        }
    }
}