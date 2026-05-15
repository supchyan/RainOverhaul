using Microsoft.Xna.Framework;
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
    public const string NOISE = "Images/Misc/noise";
    public const string LOCAL_NOISE = "RainOverhaul/Content/Textures/noise";

    private Asset<Effect> Effect    {  get; set; }
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

        Asset<Effect> asset = ModContent.Request<Effect>(
                $"RainOverhaul/Content/Effects/{EffectReference}",
                AssetRequestMode.ImmediateLoad);

        Effect = asset;
    }

    public virtual Asset<Effect> GetEffect()
    {
        return Effect;
    }
    public virtual void Load()
    {
        Filters.Scene[SceneName] = new Filter(
            new ScreenShaderData(GetEffect(), EffectReference), EffectPriority.VeryLow);

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
    public virtual void SetParameter(string parameterName, Vector4 value)
    {
        Filters.Scene[SceneName].GetShader().Shader.Parameters[parameterName].SetValue(value);
    }
    public virtual void SetParameter(string parameterName, int value)
    {
        Filters.Scene[SceneName].GetShader().Shader.Parameters[parameterName].SetValue(value);
    }
}
