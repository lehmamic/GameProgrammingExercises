#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;
in float visibility;

out vec4 out_Color;

uniform sampler2D textuerSampler;
uniform vec3 lightColor;
uniform float shineDamper; // specular power
uniform float reflectivity;
uniform vec3 skyColor; // for the fog

void main()
{
    vec3 unitNormal = normalize(surfaceNormal); // N
    vec3 unitLightVector = normalize(toLightVector); // L

    float nDotl = dot(unitNormal, unitLightVector); // dot(N,L)
    float brightness = max(nDotl, 0.2);

    vec3 diffuse = brightness * lightColor;
    
    vec3 unitVectorToCamera = normalize(toCameraVector); // V
    vec3 lightDirection = -unitLightVector;
    vec3 reflectedLightDirection = reflect(lightDirection, unitNormal); // R

    float specularFactor = dot(reflectedLightDirection, unitVectorToCamera); // dot(R,V)
    specularFactor = max(specularFactor, 0.0);
    float dampedFactor = pow(specularFactor, shineDamper); // pow(max(0.0, dot(R, V)), uSpecPower)
    vec3 finalSpecular = dampedFactor * reflectivity * lightColor;

    out_Color = vec4(diffuse, 1.0) * texture(textuerSampler, pass_textureCoords) + vec4(finalSpecular, 1.0);

    // color in the fog
    out_Color = mix(vec4(skyColor, 1.0), out_Color, visibility);
}