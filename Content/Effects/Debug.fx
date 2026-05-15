sampler uImage0 : register(s0);

float4 Main(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, uv);
    return color;
}
technique DebugTechnique
{
    pass Debug
    {
        PixelShader = compile ps_2_0 Main();
    }
}