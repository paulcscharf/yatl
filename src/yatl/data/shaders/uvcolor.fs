
uniform sampler2D diffuseTexture;

in vec4 p_color;
in vec2 p_texcoord;

out vec4 fragColor;

void main()
{
	vec4 c = p_color * texture(diffuseTexture, p_texcoord);

	if (c.r < 0.01 &&
		c.g < 0.01 &&
		c.b < 0.01 &&
		c.a < 0.01)
		discard;

    fragColor = c;
}