using Terraria;
using Terraria.UI;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using RainOverhaul.Source.Systems;
using RainOverhaul.Source.Configs;
using RainOverhaul.Source.Enums;

namespace RainOverhaul.Source.UI;

public class UICycle : UIState
{
    private UIElement Container		{ get; set; } = new UIElement();

    private UIImage CycleClearImage { get; set; } = new UIImage(UIAssets.CycleClearAsset);
    private UIImage CycleQuakeImage { get; set; } = new UIImage(UIAssets.CycleQuakeAsset);
    private UIImage CycleRainImage	{ get; set; } = new UIImage(UIAssets.CycleRainAsset);

    /// <summary>
    /// Time property used in GUI animations.
    /// </summary>
    private int Time { get; set; } = 0;
    /// <summary>
    /// CycleState cache used to control animation timers.
    /// </summary>
	private CycleState CycleStateCache { get; set; } = CycleState.Clear;
    /// <summary>
    /// Sound playied when RainWorld mode cycle changes.
    /// </summary>
	public SoundStyle CycleSwapSound { get; set; } = new("RainOverhaul/Content/Sounds/sCycleSwap");

    public override void OnInitialize()
    {
        CycleClearImage.Color	= Color.Transparent;
        CycleQuakeImage.Color	= Color.Transparent;
        CycleRainImage.Color	= Color.Transparent;

        Container.Append(CycleClearImage);
        Container.Append(CycleQuakeImage);
        Container.Append(CycleRainImage);

        Append(Container);
    }
    public override void Update(GameTime gameTime)
    {
        float xPos = Main.screenWidth  * ConfigClient.Instance.cycleIndicatorPosition.X;
        float yPos = Main.screenHeight * ConfigClient.Instance.cycleIndicatorPosition.Y;

        float IconWidth  = 64f;
        float IconHeight = 64f;

        if(xPos > Main.screenWidth - IconWidth)
        {
            xPos = Main.screenWidth - (int)IconWidth;
        }

        if(yPos > Main.screenHeight - IconHeight)
        {
            yPos = Main.screenHeight - (int)IconHeight;
        }

        SetIconRect(CycleClearImage, xPos, yPos, IconWidth, IconHeight);
        SetIconRect(CycleQuakeImage, xPos, yPos, IconWidth, IconHeight);
        SetIconRect(CycleRainImage,  xPos, yPos, IconWidth, IconHeight);

		Time++;

		if(CycleStateCache != PlayerRainSystem.RW_CurrentCycle)
        {
            CycleStateCache = PlayerRainSystem.RW_CurrentCycle;
            Time = 1;

			SoundEngine.PlaySound(CycleSwapSound);
		}

		float e = 2.71828f;
		float IconScale = 0.7f + Time / 960f;

		var IconOpacity = Color.White * (float)Math.Pow(1f / (Time / 40f), e);

		switch (PlayerRainSystem.RW_CurrentCycle)
        { 
			case CycleState.Clear:
                CycleClearImage.Color       = IconOpacity;
                CycleClearImage.ImageScale  = IconScale;

                CycleQuakeImage.Color   = Color.Transparent;
                CycleRainImage.Color    = Color.Transparent;
            break;

			case CycleState.Quake:
                CycleQuakeImage.Color       = IconOpacity;
                CycleQuakeImage.ImageScale  = IconScale;

				CycleClearImage.Color   = Color.Transparent;
                CycleRainImage.Color    = Color.Transparent;
            break;

			case CycleState.Rain:
                CycleRainImage.Color        = IconOpacity;
                CycleRainImage.ImageScale   = IconScale;

                CycleClearImage.Color = Color.Transparent;
                CycleQuakeImage.Color = Color.Transparent;
            break;
		}
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
		if (ConfigServer.Instance.isRainWorldMode) 
		{
			base.Draw(spriteBatch);
		}
	}
    private void SetIconRect(UIImage icon, float x, float y, float width, float height)
    {
        icon.Left   .Set(x, 0f);
        icon.Top    .Set(y, 0f);
        icon.Width  .Set(width, 0f);
        icon.Height .Set(height, 0f);
    }
}