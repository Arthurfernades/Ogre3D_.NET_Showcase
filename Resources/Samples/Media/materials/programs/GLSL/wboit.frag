#include <OgreUnifiedShader.h>

SAMPLER2D(accumTexture, 0);
SAMPLER2D(revealageTexture, 1);
SAMPLER2D(reflectionTexture, 2); // Nova textura para reflexão ou refração

#ifdef OGRE_HLSL
void main(vec4 pos : POSITION, vec2 oUv0 : TEXCOORD0, out vec4 gl_FragColor : COLOR)
#else
varying vec2 oUv0;
void main()
#endif
{
    vec4 accum = texture2D(accumTexture, oUv0);
    float r = accum.a;
    accum.a = texture2D(revealageTexture, oUv0).r;

    // Suaviza a transparência
    vec4 smoothAccum = (accum + texture2D(accumTexture, oUv0 + vec2(1.0 / 1024.0, 0))) * 0.5;
    smoothAccum += (accum + texture2D(accumTexture, oUv0 + vec2(0, 1.0 / 1024.0))) * 0.5;

    // Aplicando reflexão (ou refração)
    vec4 reflection = texture2D(reflectionTexture, oUv0);
    smoothAccum.rgb += reflection.rgb * 0.5;

    // Aplicando profundidade para melhorar a transparência
    float depthFactor = clamp(accum.a, 1e-4, 5e4);
    gl_FragColor = vec4(smoothAccum.rgb / depthFactor, r);
}