#define vec2 float2
#define vec3 float3
#define vec4 float4
#define mat2 float2x2
#define mat3 float3x3
#define mix lerp
#define fract frac
#define mod % // HLSL uses the % operator instead of mod(x, y) and needs to be manually changed
#define iResolution resolution
#define iGlobalTime gameTime
#define iTime gameTime
#define time gameTime


uniform float gameTime; // Total time elapsed in seconds
uniform vec2 touch;
uniform vec2 resolution;

float rand(vec2 co)
{
    return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

float GetLocation(vec2 s, float d)
{
    vec2 f = s * d;

    //s = mix(vec2(0), floor(s*d),step(0.5, f));

    // tris
    f = f % 8.; // because i failed somewhere
    
    f = f + vec2(0, 0.5) * floor(f).x;
    s = fract(f);
    f = floor(f);

    d = s.y - 0.5;
    float l = abs(d) + 0.5 * s.x;
    float ff = f.x + f.y;
    f = mix(f, f + sign(d) * vec2(0, 0.5), step(0.5, l));
    l = mix(ff, ff + sign(d) * 0.5, step(0.5, l));

    return l * rand(vec2(f));
}

vec3 hsv2rgb(float h, float s, float v)
{
    h = fract(h);
    vec3 c = smoothstep(2. / 6., 1. / 6., abs(h - vec3(0.5, 2. / 6., 4. / 6.)));
    c.r = 1. - c.r;
    /*
    vec3 c = vec3(
    smoothstep(1./6., 2./6., abs(h -0.5)),
        1.-smoothstep(1./6., 2./6., abs(h -2./6.)),
        1.-smoothstep(1./6., 2./6., abs(h -4./6.))
        );
    */
    return mix(vec3(s), vec3(1.0), c) * v;
}

vec3 getRandomColor(float f, float t)
{
    return hsv2rgb(f + t, 0.2 + cos(sin(f)) * 0.3, 0.9);
}

void mainImage(out vec4 fragColor, in vec2 fragCoord : TEXCOORD0) : COLOR0
{
    float mx = max(iResolution.x, iResolution.y);
    float t = iTime * 0.3;
    vec2 s = fragCoord.xy / mx + vec2(t, 0) * 0.2;

    float f[3];
    f[0] = GetLocation(s, 12.);
    f[1] = GetLocation(s, 6.);
    f[2] = GetLocation(s, 3.);

    vec3 color = getRandomColor(f[1] * 0.05 + 0.01 * f[0] + 0.9 * f[2], t);

    fragColor = vec4(color, 1.);
}



float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
