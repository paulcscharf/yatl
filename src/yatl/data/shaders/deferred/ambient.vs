
uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;
               
in vec3 v_position;
in vec4 v_color;

out vec3 p_position;
out vec4 p_color;

void main()
{
	vec4 hPosition = projectionMatrix * modelviewMatrix * vec4(v_position, 1.0);

	gl_Position = hPosition;

	p_position = hPosition.xyw;
	p_color = v_color;
}