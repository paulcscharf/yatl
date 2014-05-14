
const float THRESHOLD = 0.5;

uniform sampler2D diffuseBuffer;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
	vec4 argb = texture(diffuseBuffer, p_screencoord);

    fragColor = max(vec4(0), (argb - THRESHOLD) / THRESHOLD);
    //fragColor = pow(0.5 - 0.5 * cos(argb * 3.14), vec4(2));
}