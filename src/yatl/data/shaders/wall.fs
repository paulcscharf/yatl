
in vec3 p_position;

in vec3 p_normal00;
in vec3 p_normal01;
in vec3 p_normal10;
in vec3 p_normal11;
in vec2 p_normalUV;

out vec4[2] fragColor;

vec3 getNormal()
{
	return normalize(mix(
		mix(p_normal00, p_normal10, p_normalUV.x),
		mix(p_normal01, p_normal11, p_normalUV.x), p_normalUV.y));
}

void shadeFront()
{
	vec3 normal = getNormal();

	fragColor[0] = vec4(p_position, 1); // position
	fragColor[1] = vec4(0.5 + 0.5 * normal, 0); // normal
}

void shadeBack()
{
	float height = p_position.y;

	fragColor[0] = vec4(p_position, 0); // position
	fragColor[1] = vec4(0, 0, 0, 0); // normal
}

void main()
{
	if(gl_FrontFacing)
		shadeFront();
	else
		shadeBack();

}
