#version 330

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texture;
layout (location = 2) in vec3 normal;

out vec2 UV;
out vec3 color;

uniform mat4 modelMatrix;

layout(std140) uniform GlobalCamera
{
	mat4 cameraView;
	mat4 cameraPerspective;
};

layout(std140) uniform Light
{
	vec3 lightDirection;
	vec3 lightColor;
};

void main()
{   
	UV = texture;
	mat4 mvp = cameraPerspective * cameraView * modelMatrix;
	gl_Position = mvp * vec4(position, 1.0);

	// Calculate the normal in world coordinates
	mat3 normalMatrix = transpose(inverse(mat3(modelMatrix)));
	vec3 normalvec = normalize(normalMatrix * normal);

	// Calculate the vector from this surface to the light
	vec3 surfaceToLight = lightDirection - vec3(gl_Position);
	
	// Calculate the cosine of the angle of incidence.
	float brightness = dot(normalvec, surfaceToLight) / (length(surfaceToLight) * length(normalvec));
	brightness = clamp(brightness, 0, 1);

	color = vec3(brightness * lightColor);
}