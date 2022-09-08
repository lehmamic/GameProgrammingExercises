#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector[4];
in vec3 toCameraVector;
in float visibility;

out vec4 out_Color;

uniform sampler2D backgroundTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;
uniform sampler2D blendMap;

uniform vec3 lightColor[4];
uniform float shineDamper; // specular power
uniform float reflectivity;
uniform vec3 skyColor; // for the fog

void main()
{
    vec4 blendMapColor = texture(blendMap, pass_textureCoords);
    
    float backTextureAmount = 1 - (blendMapColor. r + blendMapColor.g + blendMapColor.b);
    vec2 tiledCoords = pass_textureCoords * 40;
    vec4 backgroundTextureColor = texture(backgroundTexture, tiledCoords) * backTextureAmount;
    vec4 rTextureColor = texture(rTexture, tiledCoords) * blendMapColor.r;
    vec4 gTextureColor = texture(gTexture, tiledCoords) * blendMapColor.g;
    vec4 bTextureColor = texture(bTexture, tiledCoords) * blendMapColor.b;
    
    vec4 totalColor = backgroundTextureColor + rTextureColor + gTextureColor + bTextureColor;

    vec3 unitNormal = normalize(surfaceNormal); // N
    vec3 unitVectorToCamera = normalize(toCameraVector); // V

    vec3 totalDiffuse = vec3(0.0);
    vec3 totalSpecular = vec3(0.0);
    
    for (int i = 0; i < 4; i++)
    {
        vec3 unitLightVector = normalize(toLightVector[i]);// L
        float nDotl = dot(unitNormal, unitLightVector);// dot(N,L)
        float brightness = max(nDotl, 0.0);
        vec3 lightDirection = -unitLightVector;
        vec3 reflectedLightDirection = reflect(lightDirection, unitNormal);// R
        float specularFactor = dot(reflectedLightDirection, unitVectorToCamera);// dot(R,V)
        specularFactor = max(specularFactor, 0.0);
        float dampedFactor = pow(specularFactor, shineDamper);// pow(max(0.0, dot(R, V)), uSpecPower)

        totalDiffuse = totalDiffuse +  brightness * lightColor[i];
        totalSpecular = totalSpecular + dampedFactor * reflectivity * lightColor[i];
    }

    totalDiffuse = max(totalDiffuse, 0.2);

    out_Color = vec4(totalDiffuse, 1.0) * totalColor + vec4(totalSpecular, 1.0);

    // color in the fog
    out_Color = mix(vec4(skyColor, 1.0), out_Color, visibility);
}