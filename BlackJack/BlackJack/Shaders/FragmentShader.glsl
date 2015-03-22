#version 330

in vec2 UV; // Value from the Vertex 

uniform sampler2D textureSampler;

out vec3 outputColor;

void main()
{
    outputColor = texture(textureSampler, UV).rgb;
}