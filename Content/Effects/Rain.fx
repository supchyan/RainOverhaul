sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;

float RainIntensity;
float RainDirection;
float BlueIntensity;
float MonochromeIntensity;

float4 Main(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float velocity  =  .5;
    float xOffset   =  5.; 
    float yOffset   = .01;

    float4 distort = tex2D(uImage1, float2(
        xOffset * uv.x + uv.y * (RainDirection + sin(2. * uTime) / 8.),
        yOffset * uv.y + uTime * velocity
    ));
    
    float2 _uv          = (uv.xy - .0425 * distort.xy * RainIntensity);
    float4 _color       = tex2D(uImage0, _uv);
    float4 _color_bnw   = tex2D(uImage0, _uv);
    
    _color.r += .002 * BlueIntensity;
    _color.g -= .008 * BlueIntensity;
    _color.b += .006 * BlueIntensity;
    
    float mid = (_color.r + _color.g + _color.b) / 3.0;
    
    _color_bnw.rgb = mid;
    
    return lerp(_color, _color_bnw, MonochromeIntensity);
}
technique RainTechnique
{
    pass Rain
    {
        PixelShader = compile ps_2_0 Main();
    }
}