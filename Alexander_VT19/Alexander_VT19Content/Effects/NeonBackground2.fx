float2 resolution;
float time;


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = input.Position;
    output.UV = input.UV;
    return output;
}

float s(float2 a)
{
    a = sin(a);
    return a.x + a.y;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 o = float4(0, 0, 0, 1);
    float2 i = input.UV ;
    for (int j = 0; j < 3; j++)
    {
        o[j] = s(float2(16. * i.x, 16. * i.y)) * s(float2(9. * i.x + time, 3. * i.y + time)), i *= float2(-2, 2);
    }
    o = log(o + 1.); 
    o *= .01 / max(o.x, max(o.y, o.z)); 
    return o;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
