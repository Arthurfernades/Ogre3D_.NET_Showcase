uniform sampler2D texture;
uniform vec2 texelSize;

void main()
{
    vec2 uv = gl_TexCoord[0].st;
    
    // Pegando a cor original
    vec4 color = texture2D(texture, uv);
    
    // Amostragem para Sharpening
    vec4 up = texture2D(texture, uv + vec2(0.0, texelSize.y));
    vec4 down = texture2D(texture, uv - vec2(0.0, texelSize.y));
    vec4 left = texture2D(texture, uv - vec2(texelSize.x, 0.0));
    vec4 right = texture2D(texture, uv + vec2(texelSize.x, 0.0));

    // Sharpening (Realce de detalhes)
    vec4 sharpened = color * 5.0 - (up + down + left + right);

    // Bloom (Iluminação leve)
    vec4 bloom = color + vec4(0.1, 0.1, 0.1, 0.0);

    // Combinação final
    vec4 finalColor = mix(bloom, sharpened, 0.5);
    
    gl_FragColor = finalColor;
}