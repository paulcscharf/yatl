
uniform sampler2D positionTexture;
uniform sampler2D normalTexture;

in vec2 p_screencoord;

out vec4 fragColor;

void main()
{
    vec3 normal = texture(normalTexture, p_screencoord).xyz * 2 - 1;
    vec3 position = texture(positionTexture, p_screencoord).xyz;

    vec3 light = vec3(0, 0, 1);

    float f = max(0, dot(normal, normalize(light - position)));

    fragColor = vec4(f, f, f, 1);
}
