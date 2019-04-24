float2 halfPixel;
sampler Scene : register(s0);
sampler SSAO : register(s1);
//Vertex Input Structure
struct VSI
{
    float3 Position : POSITION0;
    float2 UV : TEXCOORD0;
};
//Vertex Output Structure
struct VSO
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};
//Vertex Shader
VSO VS(VSI input)
{
//Initialize Output
    VSO output;
//Pass Position
    output.Position = float4(input.Position, 1);
//Pass Texcoord's
    output.UV = input.UV - halfPixel;
//Return
    return output;
}
//Pixel Shader
float4 PS(VSO input) : COLOR0
{
//Sample Scene
    float4 scene = tex2D(Scene, input.UV);
//Sample SSAO
    float4 ssao = tex2D(SSAO, input.UV);
//Return
    return (scene * ssao);
}
//Technique
technique Default
{
    pass p0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}