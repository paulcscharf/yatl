
in vec3 p_position;
in vec3 p_normal;

out vec4 fragColor;


void shadeFront()
{

	fragColor = vec4(0.5 + 0.5 * p_normal, 1);
}

void shadeBack()
{
	fragColor = vec4(0, 0, 0, 1);
}

void main()
{
	if(gl_FrontFacing)
		shadeFront();
	else
		shadeBack();

}
