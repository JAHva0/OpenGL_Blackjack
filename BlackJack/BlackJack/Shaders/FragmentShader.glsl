#version 330

in vec2 UV;
in vec3 Position_worldspace;
in vec3 Normal_cameraspace;
in vec3 EyeDirection_cameraspace;
in vec3 LightDirection_cameraspace;

uniform sampler2D textureSampler;

out vec3 finalColor;

layout(std140) uniform Light
{
	vec3 lightPosition;
	vec3 lightColor;
};

void main()
{	
	// Light emission properties
	float lightPower = 10.0f;

	// Material Properties
	vec3 MaterialDiffuseColor = texture2D(textureSampler, UV).rgb;
	vec3 MaterialAmbientColor = vec3(0.1f, 0.1f, 0.1f) * MaterialDiffuseColor;
	vec3 MaterialSpecularColor = vec3(0.3f, 0.3f, 0.3f);

	// Distance to the light source
	float distance = length(lightPosition - Position_worldspace);

	// Normal of the computed fragment, in camera space
	vec3 n = normalize(Normal_cameraspace);
	// Direction of the light (from the fragment to the light)
	vec3 l = normalize(LightDirection_cameraspace);

	// Cosine of the angle between the normal and the light direction, clamped 0-1
	// - light is vertical to the triangle -> 1
	// - light is perpendicular to the triangle -> 0
	// - light is behind the triangle -> 0
	float cosTheta = clamp(dot(n,l), 0,1);

	// Eye Vector (towards the camera)
	vec3 E = normalize(EyeDirection_cameraspace);
	// Direction the triangle reflects the light
	vec3 R = reflect(-l, n);
	
	// Cosine of the angle between the Eye Vector and the Reflect Vector, clampled 0-1
	// - Looking into the reflection -> 1
	// - Looking elsewhere -> <1
	float CosAlpha = clamp(dot(E, R), 0, 1);

	finalColor = 
		// Ambient: simulates indirect lighting
		MaterialAmbientColor +
		// Diffuse: the "color" of the fragment
		MaterialDiffuseColor * lightColor * lightPower * cosTheta / (distance*distance) +
		// Specular: reflective highlight
		MaterialSpecularColor * lightColor * lightPower * pow(CosAlpha, 5) / (distance*distance);
}