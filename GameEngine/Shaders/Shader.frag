#version 400

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightvector;

out vec4 out_Color;

uniform sampler2D textuerSampler;
uniform vec3 lightColor;

void main()
{
    vec3 unitNormal = normalize(surfaceNormal);
    vec3 unitLightVector = normalize(toLightvector);

    float nDotl = dot(unitNormal, unitLightVector);
    float brightness = max(nDotl, 0.0);

    vec3 diffuse = brightness * lightColor;

    out_Color = vec4(diffuse, 1.0) * texture(textuerSampler, pass_textureCoords);
}