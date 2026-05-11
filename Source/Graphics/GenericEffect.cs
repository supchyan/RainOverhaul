using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace RainOverhaul.Source.Graphics;

public class GenericEffect
{
    public const string PERLIN  = "Images/Misc/Perlin";
    public const string NOISE   = "Images/Misc/noise";

    private string EffectReference  { get; set; }
    private string SceneName        { get; set; }


    /// <param name="effectReference">  Effect reference name in `RainOverhaul/Content/Effects` directory.
    ///                                 This have to be a shader pass name as well.</param>
    ///                                 
    /// <param name="sceneName">        Scene name used as a reference for in-game calls.</param>
    public virtual void Initialize(string effectReference, string sceneName)
    {
        EffectReference = effectReference;
              SceneName = sceneName;
    }

    public virtual void Load()
    {
        Ref<Effect> RainRef = new(ModContent.Request<Effect>(
                $"RainOverhaul/Content/Effects/{EffectReference}", 
                AssetRequestMode.ImmediateLoad).Value);
        
        Filters.Scene[SceneName] = new Filter(
            new ScreenShaderData(RainRef, EffectReference), EffectPriority.VeryHigh);

        Filters.Scene[SceneName].Load();
    }
    public virtual void Activate()
    {
        if (!Filters.Scene[SceneName].IsActive())
        {
            Filters.Scene.Activate(SceneName);
        }
    }
    public virtual void Deactivate()
    {
        if (Filters.Scene[SceneName].IsActive())
        {
            Filters.Scene.Deactivate(SceneName);
        }
    }
    public virtual void SetImage(string uImage = "")
    {
        Filters.Scene[SceneName].GetShader()
            .UseImage(uImage, 0, SamplerState.LinearWrap);
    }
    public virtual void SetParameter(string parameterName, float value)
    {
        Filters.Scene[SceneName].GetShader().Shader.Parameters[parameterName].SetValue(value);
    }
}
