using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RainOverhaul.Source.UI;

public class UISystem : ModSystem
{
	private readonly UserInterface userInterface = new();

    private readonly UICycle uiState = new();
	/// <summary>
	/// Clone of gameTime value received from UpdateUI().
	/// </summary>
	private GameTime GameTimeClone { get; set; } = new();

    public override void Load()
	{
		if (!Main.dedServ)
		{
			userInterface.SetState(uiState);
		}
	}
	public override void UpdateUI(GameTime gameTime) {
        GameTimeClone = gameTime;
        userInterface.Update(gameTime);
	}
	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
		int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));

		if (resourceBarIndex != -1)
		{
			layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
				"RainOverhaul: Cycles",
				delegate {
					userInterface.Draw(Main.spriteBatch, GameTimeClone);
					return true;
				},
				InterfaceScaleType.UI)
			);
		}
	}
}