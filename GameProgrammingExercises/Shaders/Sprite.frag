// Request GLSL 3.3
#version 330

// Text coord input from vertex shader
in vec2 fragTextCoord;

// This corresponds to the output color to the color buffer
out vec4 outColor;

// For texture sampling
uniform sampler2D uTexture;

void main()
{
    // Sample color from texture
    outColor = texture(uTexture, fragTextCoord);
}