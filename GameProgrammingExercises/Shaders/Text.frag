// Request GLSL 3.3
#version 330

// Text coord input from vertex shader
in vec2 fragTextCoord;

// This corresponds to the output color to the color buffer
out vec4 outColor;

// For texture sampling
uniform sampler2D uTexture;
uniform vec3 textColor;

void main()
{
    // Sample color from texture
    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(uTexture, fragTextCoord).r);
    outColor = vec4(textColor, 1.0) * sampled;
}