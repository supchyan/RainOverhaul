using Terraria.Audio;

namespace RainOverhaul.Source.Audio;

/// <summary>
/// Contains mod sound styles 
/// </summary>
internal class ROSoundStyle
{
    public static SoundStyle Impact { get; } = new("RainOverhaul/Content/Sounds/impact");
    public static SoundStyle Death { get; } = new("RainOverhaul/Content/Sounds/death");
    public static SoundStyle Swap { get; } = new("RainOverhaul/Content/Sounds/swap");
    public static SoundStyle RainAmbience { get; } = new("RainOverhaul/Content/Sounds/rainAmbience");
    public static SoundStyle DimRainAmbience { get; } = new("RainOverhaul/Content/Sounds/dimRainAmbience");
}
