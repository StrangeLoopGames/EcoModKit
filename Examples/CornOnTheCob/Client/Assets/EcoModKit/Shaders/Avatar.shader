Shader "Curved/Avatar" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SkinColor("SkinColor", Color) = (0.8014706,0.7572515,0.7012867,1)
        _PartColor("ClothingColor", Color) = (1,1,1,1)
        _BumpMap("Normal Map", 2D) = "bump" {}
        _MetallicGlossMap("Metallic", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Glossiness("Glossiness", Range(0, 1)) = 0
    }
        
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200


        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert addshadow

        #pragma multi_compile ___ NO_CURVE // using multicompile here so we get both curve/nocurve in build, also want default no curve

        #pragma shader_feature _METALLICGLOSSMAP

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "../../EcoModKit/Shaders/CurvedHelper.cginc"

        void vert(inout appdata_full v) {
            v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
        }

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        half _Glossiness;
        half _Metallic;
        float4 _SkinColor;
        float4 _PartColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        float3 BlendSkin(float3 DetailColor, float DetailAlpha, float3 SkinColor, float3 ClothingColor)
        {
            return lerp(SkinColor, DetailColor * ClothingColor, DetailAlpha);
        }

        half3 AlbedoSkin(float2 texcoords)
        {
            half4 detail = tex2D(_MainTex, texcoords.xy).rgba;
            return BlendSkin(detail.rgb, detail.a, _SkinColor.rgb, _PartColor.rgb);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = AlbedoSkin(IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

#if defined(_METALLICGLOSSMAP)
            fixed2 metallicGloss = tex2D(_MetallicGlossMap, IN.uv_MainTex).ra;
#else
            // use sliders when no texture
            fixed2 metallicGloss = fixed2(_Metallic, _Glossiness);
#endif
            o.Metallic = metallicGloss.r;
            o.Smoothness = metallicGloss.g;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    FallBack "Diffuse"
    CustomEditor "Eco.Editor.Shaders.CurvedAvatarShaderGUI"
}
