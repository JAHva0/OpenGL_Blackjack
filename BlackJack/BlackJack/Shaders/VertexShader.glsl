#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texture;
layout (location = 2) in vec3 normal;

out vec3 fragVert;
out vec2 fragTexCoord;
out vec3 fragNormal;

uniform mat4 model;

layout(std140) uniform GlobalCamera
{
	mat4 cameraView;
	mat4 cameraPerspective;
};

void main()
{   
	gl_Position = cameraPerspective * cameraView * model * vec4(position, 1.0);

    fragVert = position;
	fragTexCoord = texture;
	fragNormal = normal;
}