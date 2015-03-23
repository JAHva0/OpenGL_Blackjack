#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texture;
layout (location = 2) in vec3 normal;

out vec2 UV;
out vec3 Position_worldspace;
out vec3 Normal_cameraspace;
out vec3 EyeDirection_cameraspace;
out vec3 LightDirection_cameraspace;

uniform mat4 modelMatrix;

layout(std140) uniform GlobalCamera
{
	mat4 cameraView;
	mat4 cameraPerspective;
};

layout(std140) uniform Light
{
	vec3 lightPosition;
	vec3 lightColor;
};

void main()
{   
	// The location of the vertex in clip space
	mat4 mvp = cameraPerspective * cameraView * modelMatrix;
	gl_Position = mvp * vec4(position, 1.0);

	// The position of the vertex in worldspace - modelMatrix * position
	Position_worldspace = (modelMatrix * vec4(position, 1.0f)).xyz;

	// Vector that points from the vertex to the camera, in camera space
	// (In camera space, the origin is at (0,0,0)
	vec3 vertexPosition_cameraspace = (cameraView * modelMatrix * vec4(position, 1.0f)).xyz;
	EyeDirection_cameraspace = vec3(0,0,0) - vertexPosition_cameraspace;

	// Vector that points from the vertex to the light, in camera space.
	// (ModelMatrix is omitted, because it is Identity)
	vec3 lightPosition_camerspace = (cameraView * vec4(lightPosition, 1.0f)).xyz;
	LightDirection_cameraspace = lightPosition_camerspace + EyeDirection_cameraspace;

	// Normal of the vertex, in camera space
	Normal_cameraspace = (cameraView * modelMatrix * vec4(normal, 0.0f)).xyz; // Only correct if we did not scale the model. 

	UV = texture;
}