
const int SAMPLERADIUS = 3;
const float SAMPLESUM = 254;

uniform sampler2D diffuseBuffer;
uniform float pixelHeight;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
	

	float[SAMPLERADIUS + 1] SAMPLEWEIGHT = float[](70, 56, 28, 8);

	vec4 argb = texture(diffuseBuffer, p_screencoord) * SAMPLEWEIGHT[0];

	for (int i = 1; i < SAMPLERADIUS; i++)
	{
		float offset = (i + 0.5) * pixelHeight;
		vec4 c = texture(diffuseBuffer, p_screencoord + vec2(0, offset))
			   + texture(diffuseBuffer, p_screencoord - vec2(0, offset));
		argb += c * SAMPLEWEIGHT[i];
	}

    fragColor = argb / SAMPLESUM;
}