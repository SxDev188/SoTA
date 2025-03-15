Shader "Unlit/RadialColorMaskURP"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1) // The _ is needed for shaders, I'm not braking our coding conventions :'D
        _MainTex ("Texture", 2D) = "white" {}
        _EffectRadius ("Effect Radius", Float) = 150
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _EffectRadius;
            float4 _PlayerPosition; // This should be the normalized screen position of the player
            float2 _ScreenResolution;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f // v2f = vertex to fragment
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target // frag = fragment
            {
                float2 uv = i.uv;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv); // Samples from texture (the frame buffer in this case)

                // Use the player position to center the effect
                float2 center = _PlayerPosition.xy; // This is already normalized in screen space (0 to 1)

                // Subtract to center the effect around the player
                float2 uvNormalized = uv - center;

                // Calculate distance from player position
                float dist = length(uvNormalized * _ScreenResolution);  

                // Apply a sharp effect transition based on distance and effect radius
                //float mask = step(_EffectRadius, dist); // Step function for a sharp edge, _EffectRadius is the threshold, if dist < _EffectRadius then mask = 0 else mask equals 1
                float mask = smoothstep(_EffectRadius - 10, _EffectRadius + 10, dist); // Creates a smooth transition instead of a sharp edge

                // Convert to greyscale
                half grayscale = dot(col.rgb, half3(0.1, 0.3, 0.05)); // Intensity of the grey
                half4 greyCol = half4(grayscale, grayscale, grayscale, 1); // half4 = half precision vector, instead of 32 bits per coordinate it's 16 bits per coordinate

                // Blend the original color with greyscale based on the mask
                return lerp(col, greyCol, mask); // Mask is either 0 (color) or 1 (greyscale)
            }
            ENDHLSL
        }
    }
}