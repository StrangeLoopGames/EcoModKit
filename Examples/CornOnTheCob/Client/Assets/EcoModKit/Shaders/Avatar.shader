Shader "Curved/Avatar" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskTex("FPV Mask Texture", 2D) = "white" {}
        _SkinColor("SkinColor", Color) = (0.8014706,0.7572515,0.7012867,1)
        _PartColor("ClothingColor", Color) = (1,1,1,1)
        _BumpMap("Normal Map", 2D) = "bump" {}
        _MetallicGlossMap("Metallic", 2D) = "white" {}
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Glossiness("Glossiness", Range(0, 1)) = 0
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _YAxisClip ("Y axis clip", Range(0, 1)) = 0.0
    }
        
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200


        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert  alphatest:_Cutoff

        #pragma multi_compile ___ NO_CURVE // using multicompile here so we get both curve/nocurve in build, also want default no curve
        
        #pragma multi_compile ___ THIRDPERSON_ON //Used for applying mask to the material based on player's view (first or third)

        #pragma shader_feature _METALLICGLOSSMAP

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "../../EcoModKit/Shaders/CurvedHelper.cginc"

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 localPos;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.localPos = v.vertex.xyz;
            v.vertex = curveVertex(v.vertex, unity_ObjectToWorld, unity_WorldToObject);
        }

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _MaskTex;
        half _Glossiness;
        half _Metallic;
        float4 _SkinColor;
        float4 _PartColor;
        half _YAxisClip;

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

//If the player is in third person ignore mask otherwise apply mask Texture
#if defined(THIRDPERSON_ON)
            o.Alpha = 1;
#else
            o.Alpha  = tex2D(_MaskTex, IN.uv_MainTex).r; //note: mask texture is R8 thus R is the only value used
#endif


        }
        ENDCG

        //Render avatar shadows in seperate pass
        Pass
		{		
			Tags{ "LightMode" = "ShadowCaster" }		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
            #pragma multi_compile ___ NO_CURVE // using multicompile here so we get both curve/nocurve in build, also want default no curve
            #include "../../EcoModKit/Shaders/CurvedHelper.cginc"
					
			float4 vert (float4 vertex:POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(curveVertex(vertex, unity_ObjectToWorld, unity_WorldToObject));//curve vertex positions for shadows
			}

			float4 frag (void) : COLOR
			{
				return 0; //we need to draw only shadows in this pass
			}
			ENDCG
		}
    }

    FallBack "Diffuse"
    CustomEditor "Eco.Editor.Shaders.CurvedAvatarShaderGUI"
}
