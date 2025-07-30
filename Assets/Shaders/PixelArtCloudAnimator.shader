Shader "Unlit/PixelArtCloudAnimatorStepped"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FramesPerSecond ("Frames Per Second", Range(1, 30)) = 8
        _AnimationIntensity ("Animation Intensity", Range(0, 1)) = 0.5
        _AnimationSpeed ("Animation Speed", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 worldPos : TEXCOORD1; // Pass world position to the fragment shader
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float _FramesPerSecond;
            float _AnimationIntensity;
            float _AnimationSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // Calculate world position
                return o;
            }

            // A simple procedural hash function to generate pseudo-random values
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Quantize time to create a stepped animation frame rate.
                float steppedTime = floor(_Time.y * _FramesPerSecond) / _FramesPerSecond;

                // 2. SNAP the UV coordinates to the pixel grid to keep it crisp.
                // This is done first to ensure we are always working with whole pixels.
                float2 snapped_uv = (floor(i.uv * _MainTex_TexelSize.zw) + 0.5) * _MainTex_TexelSize.xy;
                
                // 3. Sample the original texture color at the snapped UV.
                fixed4 col = tex2D(_MainTex, snapped_uv);

                // We only animate pixels that are not fully transparent.
                if (col.a > 0.1)
                {
                    // 4. Generate a pseudo-random value for this pixel.
                    // It's based on the pixel's world position and the current animation "frame".
                    // This creates a unique "boiling" value for each pixel over time.
                    float2 random_input = i.worldPos.xy * 0.1 + steppedTime * _AnimationSpeed;
                    float random_val = hash(random_input);

                    // 5. Use the random value to decide whether to "turn off" this pixel.
                    // If the random value is less than the intensity, we reduce its alpha.
                    // This creates the boiling/flickering effect.
                    if (random_val < _AnimationIntensity)
                    {
                        col.a *= 0.5; // You can change this value to control how dim the flicker is.
                    }
                }
                
                // Multiply by the vertex color from the SpriteRenderer
                col *= i.color;
                
                // Clamp alpha to ensure it stays between 0 and 1.
                col.a = saturate(col.a);

                return col;
            }
            ENDCG
        }
    }
}
