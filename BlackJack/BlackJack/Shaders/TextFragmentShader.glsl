#version 330

in vec2 UV;

out vec4 finalColor;

uniform sampler2D textureSampler;
uniform vec3 textColor;

void main()
{
	// Only draw the frag if it is not the background color. 
	if (texture2D(textureSampler, vec2(UV.x, UV.y)) != vec4(0.0f, 0.0f, 0.0f, 1.0f))
	{
		finalColor = vec4(textColor, 1.0f);
	}
}