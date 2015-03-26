#version 330

in vec2 UV;

out vec4 finalColor;

uniform sampler2D textureSampler;

void main()
{
	finalColor = texture2D(textureSampler, vec2(UV.x, UV.y));
}