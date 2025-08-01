Shader "Complete/URP_Scroll_Transparent_Tinted_Adjustable"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture (PNG)", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _ScrollSpeedX ("Scroll Speed X", Range(-5, 5)) = 1.0
        _ScrollSpeedY ("Scroll Speed Y", Range(-5, 5)) = 0.0
        
        // 👇 New properties for manual control 👇
        _OffsetX ("Offset X", Range(0, 1)) = 0.0
        _OffsetY ("Offset Y", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 p:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct Varyings {
                float4 p:SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _TintColor;
                float _ScrollSpeedX;
                float _ScrollSpeedY;

                // 👇 Declare the new properties for HLSL 👇
                float _OffsetX;
                float _OffsetY;
            CBUFFER_END
            
            Varyings vert(Attributes IN) {
                Varyings o;
                o.p = TransformObjectToHClip(IN.p.xyz);
                o.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return o;
            }

            half4 frag(Varyings IN) : SV_Target {
                // Create a float2 offset from our new properties
                float2 manualOffset = float2(_OffsetX, _OffsetY);
                
                // Add the manual offset to the UV calculation
                float2 loopedUV = frac(IN.uv + _Time.y * float2(_ScrollSpeedX, _ScrollSpeedY) + manualOffset);
                
                half4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, loopedUV);
                return textureColor * _TintColor;
            }
            ENDHLSL
        }
    }
}