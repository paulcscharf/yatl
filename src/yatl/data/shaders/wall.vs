
uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

in vec3 v_position;
in vec3 v_normal;

out vec3 p_position;
out vec3 p_normal;

void main()
{
	gl_Position = projectionMatrix * modelviewMatrix * vec4(v_position, 1.0);
	p_position = v_position;
	p_normal = v_normal;
}