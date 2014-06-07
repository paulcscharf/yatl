
uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

in vec3 v_position;
in vec3 v_normal00;
in vec3 v_normal01;
in vec3 v_normal10;
in vec3 v_normal11;
in vec2 v_normalUV;

out vec3 p_position;
out vec3 p_normal00;
out vec3 p_normal01;
out vec3 p_normal10;
out vec3 p_normal11;
out vec2 p_normalUV;

void main()
{
	gl_Position = projectionMatrix * modelviewMatrix * vec4(v_position, 1.0);
	p_position = v_position;

    p_normal00 = v_normal00;
    p_normal01 = v_normal01;
    p_normal10 = v_normal10;
    p_normal11 = v_normal11;
    p_normalUV = v_normalUV;
}