
uniform float fadePercentage;
uniform vec4 color;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
	vec2 uv = p_screencoord * 2 - 1;

	float d = mix(
		dot(uv, uv),
		max(abs(uv.x), abs(uv.y)),
		0.3);

	d += -0.6 + fadePercentage * 1.6;

	float dAlpha = cos(
		clamp(1 - d, 0, 1) * 3.141592
		) * 0.5 + 0.5;

	if(dAlpha < 0.001)
		discard;

	fragColor = color * dAlpha;
}