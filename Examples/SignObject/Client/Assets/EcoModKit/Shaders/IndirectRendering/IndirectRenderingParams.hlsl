#ifndef INDIRECT_RENDERING_PARAMS_INCLUDED
#define INDIRECT_RENDERING_PARAMS_INCLUDED

struct IndirectRenderingParams
{
    float4x4 objectToWorld;
    float4x4 worldToObject;
    float    lodFade;
    float2   extra;
};

#endif