// Request GLSL 3.3
#version 330

// Any vertex attributes go here
// For now, just a position
in vec3 inPosition;

void main()
{
    // The vertex shader needs to output a 4D coordinate.
    // For now set the 4th coordinate to 1.0
    gl_Position = vec4(inPosition, 1.0);
}