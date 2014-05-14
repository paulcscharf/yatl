
uniform sampler2D diffuseBuffer;
uniform float multiply;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
    fragColor = multiply * texture(diffuseBuffer, p_screencoord);
}
