
uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;
               
in vec3 v_position;
in vec3 v_lightPosition;
in float v_lightRange;
in vec4 v_lightColor;
in float v_lightIntensity;

out vec4 p_color;
out vec3 p_lightPosition;
out vec3 p_position;
out float p_lightRange;
out float p_lightIntensity;

void main()
{
	vec4 hPosition = projectionMatrix * modelviewMatrix * vec4(v_position, 1.0);

	gl_Position = hPosition;

	p_color = v_lightColor;
	p_position = hPosition.xyw;

	p_lightPosition = v_lightPosition;
	p_lightRange = v_lightRange;
	p_lightIntensity = v_lightIntensity;
}