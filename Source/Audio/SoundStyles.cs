using Terraria.Audio;

namespace RainOverhaul.Source.Audio;

/// <summary>
/// Contains mod sound styles 
/// </summary>
internal class SoundStyles
{
    public static SoundStyle EnterSound { get; } = new("RainOverhaul/Content/Sounds/sEnter");
    public static SoundStyle DeathSound { get; } = new("RainOverhaul/Content/Sounds/sDeath");
}
