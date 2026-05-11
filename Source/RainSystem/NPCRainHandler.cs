using RainOverhaul.Source.Buffs;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Managers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RainOverhaul.Source.RainSystem;     
public class NPCRainHandler : GlobalNPC
{
    /// <summary>
    /// Damage control of the NPCs under the rain.
    /// </summary>
    private bool IsNpcLosingLife { get; set; } = false;
    /// <summary>
    /// True whenever NPC cannot get a rain damage.
    /// </summary>
    private bool IsNpcInSafePlace { get; set; } = false;

    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        IsNpcLosingLife  = false;
        IsNpcInSafePlace = true;
    }
    public override void AI(NPC npc)
    {
        var wallTile = Main.tile[npc.Center.ToTileCoordinates()];
        // Returns true if tile is a wall type
        bool hasWallCollision = wallTile.WallType > WallID.None;

        for (int y = Main.screenPosition.ToTileCoordinates().Y; y < npc.Top.ToTileCoordinates().Y; y++)
        {
            var solidTile = Main.tile[npc.Center.ToTileCoordinates().X, y];

            var isSolidTile = solidTile.HasTile && Main.tileSolid[solidTile.TileType];

            var safeGeneral     = isSolidTile || hasWallCollision;
            var safeTownNPC     = npc.townNPC && !ConfigServer.Instance.rainWorldAffectsTownNPSs;
            var safeOtherNPC    = !npc.townNPC && !ConfigServer.Instance.rainWorldAffectsOtherNPCs;

            IsNpcInSafePlace = safeGeneral || safeTownNPC || safeOtherNPC;

            if (IsNpcInSafePlace)
            {
                break;
            }
        }

        // let npc handle rain logic near the any player
        bool isCloseToPlayer = (Main.LocalPlayer.Center - npc.Center).Length() < 1500f;

        IsNpcLosingLife = npc.active &&
                ConfigServer.Instance.isRainWorldMode &&
                Main.raining && !IsNpcInSafePlace &&
                PlayerManager.IsPlayerInRainArea && isCloseToPlayer;

        if (IsNpcLosingLife)
        {
            npc.AddBuff(ModContent.BuffType<RainSystemDebuff>(), 2);
        }
    }
}