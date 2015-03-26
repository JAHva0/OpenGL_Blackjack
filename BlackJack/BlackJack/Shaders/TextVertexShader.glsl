#version 330

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 texture;

out vec2 UV;

void main()
{
	// Calculate the output position of the text in clip space
    vec2 vertexPosition_homoneneousspace = vertexPosition.xy / vec2(800,450);
    gl_Position =  vec4(vertexPosition_homoneneousspace,0,1);

	UV = texture;
}