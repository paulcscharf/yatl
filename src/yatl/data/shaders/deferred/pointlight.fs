
uniform sampler2D positionTexture;
uniform sampler2D normalTexture;

in vec4 p_color;
in vec3 p_lightPosition;
in vec3 p_position;
in float p_lightRange;
in float p_lightIntensity;

out vec4 fragColor;

vec4 getPosition(vec2 uv)
{
	return texture(positionTexture, uv);
}
vec4 getNormal(vec2 uv)
{
	return texture(normalTexture, uv) * 2 - 1;
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

	vec3 diff = p_lightPosition - position;

	float dSquared = dot(diff, diff);

	if(dSquared >  p_lightRange * p_lightRange)
		discard;

	float d = 1 - sqrt(dSquared) / p_lightRange;

	vec3 normal = getNormal(uv).xyz;

	float diffuse = dot(normalize(normal), normalize(diff));

	if (diffuse <= 0)
	{
		discard;
	}

	float height = (position.z * 0.5);

	float heightAlpha = 1 - height * height;

	fragColor = p_color * p_lightIntensity
		* diffuse * d * d * alpha * heightAlpha;
}
