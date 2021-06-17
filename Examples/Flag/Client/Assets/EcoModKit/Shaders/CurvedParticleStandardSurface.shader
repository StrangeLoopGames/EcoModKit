// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Modified to add curving

Shader "Curved/Particles/Standard Surface"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _MetallicGlossMap("Metallic", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DistortionStrength("Strength", Float) = 1.0
        _DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

        // Hidden properties
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _FlipbookMode ("__flipbookmode", Float) = 0.0
        [HideInInspector] _LightingEnabled ("__lightingenabled", Float) = 1.0
        [HideInInspector] _DistortionEnabled ("__distortionenabled", Float) = 0.0
        [HideInInspector] _EmissionEnabled ("__emissionenabled", Float) = 0.0
        [HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector] _Cull ("__cull", Float) = 2.0
        [HideInInspector] _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams ("__camerafadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _DistortionStrengthScaled ("__distortionstrengthscaled", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "IgnoreProjector"="True" "PreviewType"="Plane" "PerformanceChecks"="False" }

        BlendOp [_BlendOp]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]

        GrabPass
        {
            Tags { "LightMode" = "Always" }
            "_GrabTexture"
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma target 3.0

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma shader_feature NO_CURVE
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:vertInstancingSetup

            #pragma vertex vertParticleShadowCasterCurve
            #pragma fragment fragParticleShadowCaster

            #include "UnityStandardParticleShadow.cginc"
            #include "CurvedHelper.cginc"

            void vertParticleShadowCasterCurve(VertexInput v,
#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
                out VertexOutputShadowCaster o,
#define OUTO o, 
#else
#define OUTO
#endif
#ifdef UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
                out VertexOutputStereoShadowCaster os,
#define OUTOS os, 
#else
#define OUTOS
#endif
                out float4 opos : SV_POSITION)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
                vertParticleShadowCaster(v, OUTO OUTOS opos);
            }
            ENDCG
        }

        Pass
        {
            Name "SceneSelectionPass"
            Tags { "LightMode" = "SceneSelectionPass" }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma target 3.0

            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:vertInstancingSetup

            #pragma vertex vertEditorPass
            #pragma fragment fragSceneHighlightPass

            #include "UnityStandardParticleEditor.cginc"
            ENDCG
        }

        Pass
        {
            Name "ScenePickingPass"
            Tags{ "LightMode" = "Picking" }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma target 3.0

            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:vertInstancingSetup

            #pragma vertex vertEditorPass
            #pragma fragment fragScenePickingPass

            #include "UnityStandardParticleEditor.cginc"
            ENDCG
        }

        CGPROGRAM
        #pragma surface surf Standard nolightmap nometa noforwardadd keepalpha vertex:vertCurve
        #pragma multi_compile __ SOFTPARTICLES_ON
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:vertInstancingSetup
        #pragma target 3.0
        #pragma shader_feature NO_CURVE

        #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
        #pragma shader_feature_local _METALLICGLOSSMAP
        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _EMISSION
        #pragma shader_feature_local _FADING_ON
        #pragma shader_feature_local _REQUIRE_UV2
        #pragma shader_feature_local EFFECT_BUMP

        #include "UnityStandardParticles.cginc"
        #include "CurvedHelper.cginc"

        void vertCurve(inout appdata_particles v, out Input o)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
            vert(v, o);
        }

        ENDCG
    }

    Fallback "VertexLit"
    CustomEditor "StandardParticlesShaderGUI"
}
