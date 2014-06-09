
uniform sampler2D positionTexture;

in vec3 p_position;
in vec4 p_color;

out vec4 fragColor;

vec4 getPosition(vec2 uv)
{
	return texture(positionTexture, uv);
}

void main()
{
	vec2 uv = (p_position.xy / p_position.z) * 0.5 + 0.5;

	vec4 positionAlpha = getPosition(uv);

	float alpha = positionAlpha.w;

	if (alpha < 0.001)
	{
		discard;
	}

	vec3 position = positionAlpha.xyz;

	float height = (position.z * 0.5);

	float heightAlpha = 1 - height * height;

	fragColor = p_color * alpha * heightAlpha;
}