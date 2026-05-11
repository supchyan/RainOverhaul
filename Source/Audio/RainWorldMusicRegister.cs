using Terraria.ModLoader;

namespace RainOverhaul.Source.Audio; 
public sealed class RainWorldMusicRegister : ILoadable
{
	public void Load(Mod mod)
    {
		MusicLoader.AddMusic(mod, "Content/Sounds/sRain");
        MusicLoader.AddMusic(mod, "Content/Sounds/sDimRain");
    }
	public void Unload() { }
}