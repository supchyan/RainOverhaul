sampler uImage0 : register(s0);

float uTime;

float QuakeIntensity;

float4 Main(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{ 
    float offset = 3.7;
    
    uv.x += QuakeIntensity * sin(offset * 16. * uTime) / 1000.;
    uv.y += QuakeIntensity * sin(offset * 4.  * uTime) / 1000.;
    
    float4 _color = tex2D(uImage0, uv);
    return _color;
}
technique ShakeTechnique
{
    pass Quake
    {
        PixelShader = compile ps_2_0 Main();
    }
}