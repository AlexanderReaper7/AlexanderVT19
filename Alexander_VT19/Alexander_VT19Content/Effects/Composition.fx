float2 GBufferTextureSize;
sampler Albedo : register(s0);
sampler LightMap : register(s1);

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
    output.UV = input.UV - float2(1.0f / GBufferTextureSize.xy);
    //Return
    return output;
}

//Pixel Shader
float4 PS(VSO input) : COLOR0
{
    //Sample Albedo
    float3 Color = tex2D(Albedo, input.UV).xyz;
    //Sample Light Map
    float4 Lighting = tex2D(LightMap, input.UV);
    //Accumulate to Final Color
    float4 output = float4(Color.xyz * Lighting.xyz + Lighting.w, 1);
    //Return
    return output;
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