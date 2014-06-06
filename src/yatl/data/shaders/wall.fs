
in vec3 p_position;
in vec3 p_normal;

out vec4[2] fragColor;


void shadeFront()
{
	vec3 normal = normalize(p_normal);

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
