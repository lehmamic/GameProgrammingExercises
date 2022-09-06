#version 400 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 textureCoords;

out vec2 pass_textureCoords;
out vec3 surfaceNormal;
out vec3 toLightVector;
out vec3 toCameraVector;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 lightPosition;

uniform float useFakeLighting;

void main()
{
    vec4 worldPosition = transformationMatrix * vec4(position, 1.0);
    gl_Position = projectionMatrix * viewMatrix * worldPosition;
    pass_textureCoords = textureCoords;
    
    vec3 actualNormal = normal;
    // Fake the grass lighting since it consists only of vertical planes and are not lightned correctly
    // we set fake normals for that
    if (useFakeLighting > 0.5)
    {
        actualNormal = vec3(0.0, 1.0, 0.0); // directly up
    }

    surfaceNormal = (transformationMatrix * vec4(actualNormal, 0.0)).xyz;
    toLightVector = lightPosition - worldPosition.xyz;
    toCameraVector = (inverse(viewMatrix) * vec4(0.0, 0.0, 0.0, 1.0)).xyz - worldPosition.xyz;
}