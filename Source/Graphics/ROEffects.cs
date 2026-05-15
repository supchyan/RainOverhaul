namespace RainOverhaul.Source.Graphics;

public class ROEffects
{
    public class RainEffect : GenericEffect
    {
        public static RainEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Rain", "RainScene");
            base.Load();
            SetImage(NOISE);
        }
    }
    public class AlternateRainEffect : GenericEffect
    {
        public static AlternateRainEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Rain", "AltRainScene");
            base.Load();
            SetImage(PERLIN);
        }
    }
    public class QuakeEffect : GenericEffect
    {
        public static QuakeEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Quake", "QuakeScene");
            base.Load();
        }
    }
}
