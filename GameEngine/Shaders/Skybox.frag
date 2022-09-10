#version 400

in vec3 textureCoords;
out vec4 out_Color;

uniform samplerCube cubeMap;
uniform vec3 fogColor;

// fog fading in limits
const float lowerLimit = 0.0; // completly in fog
const float upperLimit = 30.0; // completle out of fog, inbetween it will be a transition

void main(void){
    vec4 finalColor = texture(cubeMap, textureCoords);
    
    float factor = (textureCoords.y - lowerLimit) / (upperLimit - lowerLimit);
    factor = clamp(factor, 0.0, 1.0);
    out_Color = mix(vec4(fogColor, 1.0), finalColor, factor);
}