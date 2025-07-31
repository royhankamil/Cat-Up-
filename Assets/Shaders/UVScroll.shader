Shader "Unlit/URP_Scroll"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeedX ("Scroll Speed X", Range(-5, 5)) = 1.0
        _ScrollSpeedY ("Scroll Speed Y", Range(-5, 5)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _ScrollSpeedX;
                float _ScrollSpeedY;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Create a vector for the scroll speed
                float2 speed = float2(_ScrollSpeedX, _ScrollSpeedY);

                // Calculate the offset using built-in time
                float2 offset = _Time.y * speed;

                // Apply offset and use frac() to loop the UVs
                float2 scrolledUV = IN.uv + offset;
                float2 loopedUV = frac(scrolledUV);
                
                // Sample the texture with the new UVs
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, loopedUV);
                return color;
            }
            ENDHLSL
        }
    }
}