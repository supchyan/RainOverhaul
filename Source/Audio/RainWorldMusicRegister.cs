using Terraria.ModLoader;

namespace RainOverhaul.Source.Audio; 
public sealed class RainWorldMusicRegister : ILoadable
{
	public void Load(Mod mod)
    {
		MusicLoader.AddMusic(mod, "Content/Sounds/rainAmbience");
        MusicLoader.AddMusic(mod, "Content/Sounds/dimRainAmbience");
    }
	public void Unload() { }
}