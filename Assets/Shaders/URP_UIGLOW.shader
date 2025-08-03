// Unity Shader for a UI element with a Fresnel-based edge glow.
// This shader is intended for use with the Universal Render Pipeline (URP).
// For the glow to be visible, you MUST have a Bloom Post-Processing effect
// enabled on your camera or in a Global Volume.
Shader "UI/URP_UIGlow"
{
    // Properties block defines the inputs that will be visible in the Material Inspector.
    Properties
    {
        // The main texture for the UI element (e.g., a sprite).
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // The color of the glow. It's crucial to set this to HDR (High Dynamic Range)
        // so its intensity can go above 1, which is what triggers the Bloom effect.
        [HDR] _GlowColor ("Glow Color", Color) = (0, 1, 0, 1)

        // Controls the "tightness" or falloff of the glow from the edge.
        // Higher values make the glow band thinner and closer to the edge.
        _GlowPower ("Glow Power", Range(0.1, 10)) = 2.5

        // A multiplier for the overall brightness of the glow.
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2

        // Standard properties required for UI masking and interaction.
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        // Tags are essential for the render pipeline to know how and when to render this shader.
        Tags
        {
            "Queue"="Transparent"           // Rendered with other transparent objects.
            "RenderType"="Transparent"      // Treated as a transparent shader.
            "RenderPipeline"="UniversalPipeline" // Specifies this is a URP shader.
            "IgnoreProjector"="True"
        }

        // Stencil block for UI masking (e.g., for Scroll Rects).
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            // Standard blend mode for transparent UI.
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off        // UI elements are typically two-sided.
            ZWrite Off      // Don't write to the depth buffer.
            ZTest [unity_GUIZTestMode] // Use standard UI depth testing.
            ColorMask [_ColorMask]

            // Begin HLSL code block
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Include necessary URP and Unity UI shader library files.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            // Struct for the data passed from the application to the vertex shader.
            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float3 normal   : NORMAL;
            };

            // Struct for the data passed from the vertex shader to the fragment shader.
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float3 viewDir  : TEXCOORD1; // To store the view direction.
                float3 normal   : TEXCOORD2; // To store the world normal.
            };

            // Define the properties from the Properties block so we can use them in HLSL.
            CBUFFER_START(UnityPerMaterial)
                sampler2D _MainTex;
                float4 _MainTex_ST;
                half4 _GlowColor;
                half _GlowPower;
                half _GlowIntensity;
            CBUFFER_END

            // Vertex Shader
            v2f vert(appdata v)
            {
                v2f o;
                // Transform vertex position from object space to clip space.
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                
                // Pass through the texture coordinates and vertex color.
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                // Calculate world space normal and view direction for the Fresnel effect.
                // The view direction is the vector from the vertex position to the camera.
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                // Sample the main texture color.
                half4 texColor = tex2D(_MainTex, i.uv);
                
                // Combine texture color with the vertex color from the UI system (for tinting).
                half4 finalColor = texColor * i.color;

                // Calculate the Fresnel effect.
                // It's based on the dot product between the surface normal and the view direction.
                // The result is close to 0 when viewing the surface head-on, and 1 at grazing angles.
                // We use pow() with _GlowPower to control the falloff.
                half fresnel = pow(1.0 - saturate(dot(i.normal, i.viewDir)), _GlowPower);
                
                // Calculate the final emission color.
                // We multiply the glow color by the fresnel term and the overall intensity.
                half3 emission = _GlowColor.rgb * fresnel * _GlowIntensity;
                
                // Add the emission to the final color's RGB channels.
                // The final alpha is determined by the texture and vertex color alpha.
                return half4(finalColor.rgb + emission, finalColor.a);
            }
            ENDHLSL
        }
    }
    // Fallback for older systems.
    FallBack "UI/Default"
}
