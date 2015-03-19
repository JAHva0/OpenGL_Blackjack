#version 330

layout (location = 0) in vec4 position;
layout (location = 1) in vec4 color;

smooth out vec4 theColor;

layout(std140) uniform GlobalCamera
{
	mat4 cameraView;
	mat4 cameraPerspective;
};

uniform vec3 location;

void main()
{   
    gl_Position = (cameraPerspective * cameraView) * position;

    theColor = color;
}