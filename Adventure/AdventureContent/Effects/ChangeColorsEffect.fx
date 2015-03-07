sampler tex;

float red;
float green;
float blue;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(tex, coords);

	
	color.r += red;
	if (color.r > 1.0)
		color.r = 1.0;
	if (color.r < 0.0)
		color.r = 0.0;

	color.g += green;
	if (color.g > 1.0)
		color.g = 1.0;
	if (color.g < 0.0)
		color.g = 0.0;
	
	color.b += blue;
	if (color.b > 1.0)
		color.b = 1.0;
	if (color.b < 0.0)
		color.b = 0.0;


    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
