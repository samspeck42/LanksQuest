sampler tex;

bool red;
bool green;
bool blue;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(tex, coords);

	if (red)
	{
		color.r = 1.0;
	}
	if (green)
	{
		color.g = 1.0;
	}
	if (blue)
	{
		color.b = 1.0;
	}
	

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
