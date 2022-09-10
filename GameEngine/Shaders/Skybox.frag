#version 400

in vec3 textureCoords;
out vec4 out_Color;

uniform samplerCube cubeMap;
uniform samplerCube cubeMap2;
uniform float blendFactor;
uniform vec3 fogColor;

// fog fading in limits
const float lowerLimit = 0.0; // completly in fog
const float upperLimit = 30.0; // completle out of fog, inbetween it will be a transition

const float levels = 3.0;

void main(void)
{
    vec4 texture1 = texture(cubeMap, textureCoords);
    vec4 texture2 = texture(cubeMap2, textureCoords);
    vec4 finalColor = mix(texture1, texture2, blendFactor);
    
    float amount = (finalColor.r + finalColor.g + finalColor.b) / 3.0;
    amount = floor(amount * levels) / levels;
    finalColor.rgb = amount * fogColor;
    
    float factor = (textureCoords.y - lowerLimit) / (upperLimit - lowerLimit);
    factor = clamp(factor, 0.0, 1.0);
    out_Color = mix(vec4(fogColor, 1.0), finalColor, factor);
}