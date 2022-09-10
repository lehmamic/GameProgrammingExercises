#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector[4];
in vec3 toCameraVector;
in float visibility;

out vec4 out_Color;

uniform sampler2D textuerSampler;
uniform vec3 lightColor[4];
uniform vec3 attenuation[4];
uniform float shineDamper; // specular power
uniform float reflectivity;
uniform vec3 skyColor; // for the fog

const float levels = 3.0;

void main()
{
    vec3 unitNormal = normalize(surfaceNormal); // N
    vec3 unitVectorToCamera = normalize(toCameraVector); // V
    
    vec3 totalDiffuse = vec3(0.0);
    vec3 totalSpecular = vec3(0.0);

    for (int i = 0; i < 4; i++)
    {
        float distance = length(toLightVector[i]);
        float attFactor = attenuation[i].x + (attenuation[i].y * distance) + (attenuation[i].z * distance * distance);
        vec3 unitLightVector = normalize(toLightVector[i]); // L
        float nDotl = dot(unitNormal, unitLightVector); // dot(N,L)
        float brightness = max(nDotl, 0.0);
        float level = floor(brightness * levels);
        brightness = level / levels;
        vec3 lightDirection = -unitLightVector;
        vec3 reflectedLightDirection = reflect(lightDirection, unitNormal); // R
        float specularFactor = dot(reflectedLightDirection, unitVectorToCamera); // dot(R,V)
        specularFactor = max(specularFactor, 0.0);
        float dampedFactor = pow(specularFactor, shineDamper); // pow(max(0.0, dot(R, V)), uSpecPower)
        level = floor(dampedFactor * levels);
        dampedFactor = level / levels;

        totalDiffuse = totalDiffuse + (brightness * lightColor[i]) / attFactor;
        totalSpecular = totalSpecular + (dampedFactor * reflectivity * lightColor[i]) / attFactor;
    }

    totalDiffuse = max(totalDiffuse, 0.2);

    vec4 textureColor = texture(textuerSampler, pass_textureCoords);
    // don't render transparent pixels
    if (textureColor.a < 0.5) 
    {
        discard;
    }

    out_Color = vec4(totalDiffuse, 1.0) * textureColor + vec4(totalSpecular, 1.0);

    // color in the fog
    out_Color = mix(vec4(skyColor, 1.0), out_Color, visibility);
}