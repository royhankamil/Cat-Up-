Shader "Complete/URP_Scroll_Transparent_Tinted"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture (PNG)", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1) // New color property
        _ScrollSpeedX ("Scroll Speed X", Range(-5, 5)) = 1.0
        _ScrollSpeedY ("Scroll Speed Y", Range(-5, 5)) = 0.0
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

            struct Attributes { float4 p:POSITION; float2 uv:TEXCOORD0; };
            struct Varyings { float4 p:SV_POSITION; float2 uv:TEXCOORD0; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _TintColor; // New color variable
                float _ScrollSpeedX;
                float _ScrollSpeedY;
            CBUFFER_END

            Varyings vert(Attributes IN) { /* ... same as before ... */ Varyings o; o.p = TransformObjectToHClip(IN.p.xyz); o.uv = TRANSFORM_TEX(IN.uv, _MainTex); return o; }

            half4 frag(Varyings IN) : SV_Target {
                float2 loopedUV = frac(IN.uv + _Time.y * float2(_ScrollSpeedX, _ScrollSpeedY));
                half4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, loopedUV);
                // Multiply texture by tint color
                // The alpha of the tint color can affect the final transparency
                return textureColor * _TintColor;
            }
            ENDHLSL
        }
    }
}