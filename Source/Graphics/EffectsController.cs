namespace RainOverhaul.Source.Graphics;

public class EffectsController
{
    public class RainEffect : GenericEffect
    {
        public static RainEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Rain", "JustRainScene");
            base.Load();
            SetImage(NOISE);
        }
    }
    public class AlternateRainEffect : GenericEffect
    {
        public static AlternateRainEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Rain", "JustSecondRainScene");
            base.Load();
            SetImage(PERLIN);
        }
    }
    public class MenuRainEffect : GenericEffect
    {
        public static MenuRainEffect Instance { get; } = new();
        public override void Load()
        {
            Initialize("Rain", "MenuRainScene");
            base.Load();
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
