#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texture;
layout (location = 2) in vec3 normal;

out vec2 UV;

uniform vec3 location;
uniform mat4 rotation;
uniform mat4 scale;

layout(std140) uniform GlobalCamera
{
	mat4 cameraView;
	mat4 cameraPerspective;
};

void main()
{   
    gl_Position = (cameraPerspective * cameraView) * rotation * scale * vec4((position + location), 1.0);

    UV = texture;
}