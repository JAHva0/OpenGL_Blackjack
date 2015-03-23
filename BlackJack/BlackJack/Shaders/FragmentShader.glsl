#version 330

in vec2 UV;
in vec3 color;

uniform sampler2D textureSampler;

out vec4 finalColor;

void main()
{	
	vec4 outputColor = texture(textureSampler, UV);
	finalColor = vec4((outputColor.rgb * color), outputColor.a);
}