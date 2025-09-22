using System.ComponentModel;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework;

namespace RainOverhaul.Content {
    public class RainConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("RainSettings")]
        [DefaultValue(1)]
        [Range(0f, 1f)]
        public float cIntensity;

        [Header("RainWorldExactly")]
        [DefaultValue(typeof(Vector2), "0.04, 0.9")]
        public Vector2 cCycleIndicatorPos;
        
        
        [DefaultValue(true)]
        public bool cCustomDeathSound;
    }
    public class RainConfigServer:ModConfig {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("RainSettingsServer")]
        [DefaultValue(false)]
        public bool cRainWorld;
    }
}