 float4x4 CamerasViewProjection;
float4x4 LightsViewProjection;
float4x4 World;
float3 LightPos;
float LightPower;
float Ambient;
 
texture Texture;

sampler TextureSampler = sampler_state
{
    texture = <Texture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = mirror;
    AddressV = mirror;
};
texture ShadowMap;

sampler ShadowMapSampler = sampler_state
{
    texture = <ShadowMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};
texture LightTexture;

sampler LightSampler = sampler_state
{
    texture = <LightTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 Position3D : TEXCOORD2;
};

struct PixelShaderOutput
{
    float4 Color : COLOR0;
};

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);
}

VertexShaderOutput SimplestVertexShader(float4 inPos : POSITION0, float3 inNormal : NORMAL0, float2 inTexCoords : TEXCOORD0)
{
    VertexShaderOutput Output = (VertexShaderOutput) 0;
    
    float4x4 preWorldViewProjection = mul(World, CamerasViewProjection);
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.TexCoords = inTexCoords;
    Output.Normal = normalize(mul(inNormal, (float3x3) World));
    Output.Position3D = mul(inPos, World);

    return Output;
}

PixelShaderOutput OurFirstPixelShader(VertexShaderOutput PSIn)
{
    PixelShaderOutput Output = (PixelShaderOutput) 0;

    float diffuseLightingFactor = DotProduct(LightPos, PSIn.Position3D, PSIn.Normal);
    diffuseLightingFactor = saturate(diffuseLightingFactor);
    diffuseLightingFactor *= LightPower;

    PSIn.TexCoords.y--;
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    Output.Color = baseColor * (diffuseLightingFactor + Ambient);

    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 OurFirstPixelShader();
    }
}

struct ShadowMapVertexOutput
{
    float4 Position : POSITION;
    float4 Position2D : TEXCOORD0;
};

struct ShadowMapPixelOutput
{
    float4 Color : COLOR0;
};


ShadowMapVertexOutput ShadowMapVertexShader(float4 inPos : POSITION)
{
    ShadowMapVertexOutput Output = (ShadowMapVertexOutput) 0;
    
    float4x4 preLightsWorldViewProjection = mul(World, LightsViewProjection);

    Output.Position = mul(inPos, preLightsWorldViewProjection);
    Output.Position2D = Output.Position;

    return Output;
}

ShadowMapPixelOutput ShadowMapPixelShader(ShadowMapVertexOutput PSIn)
{
    ShadowMapPixelOutput Output = (ShadowMapPixelOutput) 0;

    Output.Color = PSIn.Position2D.z / PSIn.Position2D.w;

    return Output;
}


technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowMapVertexShader();
        PixelShader = compile ps_2_0 ShadowMapPixelShader();
    }
}


struct ShadowSceneVertexOutput
{
    float4 Position : POSITION;
    float4 Pos2DAsSeenByLight : TEXCOORD0;
    float2 TexCoords : TEXCOORD1;
    float3 Normal : TEXCOORD2;
    float4 Position3D : TEXCOORD3;
};

struct ShadowScenePixelOutput
{
    float4 Color : COLOR0;
};

ShadowSceneVertexOutput ShadowedSceneVertexShader(float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL)
{
    ShadowSceneVertexOutput Output = (ShadowSceneVertexOutput) 0;
    
    float4x4 preWorldViewProjection = mul(World, CamerasViewProjection);
    float4x4 preLightsWorldViewProjection = mul(World, LightsViewProjection);

    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Pos2DAsSeenByLight = mul(inPos, preLightsWorldViewProjection);
    Output.Normal = normalize(mul(inNormal, (float3x3) World));
    Output.Position3D = mul(inPos, World);
    Output.TexCoords = inTexCoords;

    return Output;
}

ShadowScenePixelOutput ShadowedScenePixelShader(ShadowSceneVertexOutput PSIn)
{
    ShadowScenePixelOutput Output = (ShadowScenePixelOutput) 0;

    float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;
    ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y / PSIn.Pos2DAsSeenByLight.w / 2.0f + 0.5f;
    
    float diffuseLightingFactor = 0;
    if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
    {
        float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
        float realDistance = PSIn.Pos2DAsSeenByLight.z / PSIn.Pos2DAsSeenByLight.w;
        if ((realDistance - 1.0f / 100.0f) <= depthStoredInShadowMap)
        {
            diffuseLightingFactor = DotProduct(LightPos, PSIn.Position3D, PSIn.Normal);
            diffuseLightingFactor = saturate(diffuseLightingFactor);
            diffuseLightingFactor *= LightPower;
            
            float lightTextureFactor = tex2D(LightSampler, ProjectedTexCoords).r;
            diffuseLightingFactor *= lightTextureFactor;
        }
    }
        
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    Output.Color = baseColor * (diffuseLightingFactor + Ambient);

    return Output;
}

technique ShadowedScene
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowedSceneVertexShader();
        PixelShader = compile ps_2_0 ShadowedScenePixelShader();
    }
}