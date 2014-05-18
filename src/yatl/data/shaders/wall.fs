
in vec3 p_position;
in vec3 p_normal;

out vec4 fragColor;

void main()
{
	float r = p_position.z * 0.5;

	fragColor = vec4(r, r, r, 1);
}