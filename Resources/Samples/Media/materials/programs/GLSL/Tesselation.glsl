#version 400 core

layout(vertices = 3) out;

in vec3 inPosition[];
in vec3 inNormal[];

out vec3 tessPosition;
out vec3 tessNormal;

uniform float tessFactor; // Fator de tesselação

void main()
{
    gl_TessLevelInner[0] = tessFactor;
    gl_TessLevelOuter[0] = tessFactor;
    gl_TessLevelOuter[1] = tessFactor;
    gl_TessLevelOuter[2] = tessFactor;

    tessPosition = inPosition[gl_InvocationID];
    tessNormal = inNormal[gl_InvocationID];
}