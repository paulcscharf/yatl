
uniform sampler2D diffuseBuffer;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
    fragColor = texture(diffuseBuffer, p_screencoord);
}
