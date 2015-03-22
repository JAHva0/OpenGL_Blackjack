#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texture;
layout (location = 2) in vec3 normal;

smooth out vec4 theColor;

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

    theColor = vec4(1.0f, 0.0f, 1.0f, 1.0f);
}