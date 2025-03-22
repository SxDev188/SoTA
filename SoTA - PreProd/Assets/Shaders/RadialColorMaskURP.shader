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

            #define MAX_LIGHT_SOURCE_NUM 4

            float _EffectRadius;
            float4 _StarPosition; // This should be the normalized screen position of the star
            float4 _LightPositions[MAX_LIGHT_SOURCE_NUM]; 
            
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

            float GetMask(float4 lightPosition, float2 uv)
            {
                // Use the star position to center the effect
                float2 center = lightPosition.xy; // This is already normalized in screen space (0 to 1)

                // Subtract to center the effect around the star
                float2 uvNormalized = uv - center;

                // Calculate distance from star position
                float dist = length(uvNormalized * _ScreenResolution);  

                // Apply a sharp effect transition based on distance and effect radius
                //float mask = step(_EffectRadius, dist); // Step function for a sharp edge, _EffectRadius is the threshold, if dist < _EffectRadius then mask = 0 else mask equals 1
                return smoothstep(_EffectRadius - 10, _EffectRadius + 10, dist); // Creates a smooth transition instead of a sharp edge
            }

            half4 frag (v2f i) : SV_Target // frag = fragment
            {
                float2 uv = i.uv;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv); // Samples from texture (the frame buffer in this case)

                // Use the star position to center the effect
                float2 center = _StarPosition.xy; // This is already normalized in screen space (0 to 1)

                // Subtract to center the effect around the star
                float2 uvNormalized = uv - center;

                // Calculate distance from star position
                float dist = length(uvNormalized * _ScreenResolution);  

                // Apply a sharp effect transition based on distance and effect radius
                //float mask = step(_EffectRadius, dist); // Step function for a sharp edge, _EffectRadius is the threshold, if dist < _EffectRadius then mask = 0 else mask equals 1
                float mask = smoothstep(_EffectRadius - 10, _EffectRadius + 10, dist); // Creates a smooth transition instead of a sharp edge

                float mask_final = mask;
                for (int i = 0; i < MAX_LIGHT_SOURCE_NUM; ++i)
                {
                    mask_final *= GetMask(_LightPositions[i], uv);
                }

                // Convert to greyscale
                half grayscale = dot(col.rgb, half3(0.1, 0.3, 0.05)); // Intensity of the grey
                half4 greyCol = half4(grayscale, grayscale, grayscale, 1); // half4 = half precision vector, instead of 32 bits per coordinate it's 16 bits per coordinate

                // Blend the original color with greyscale based on the mask
                return lerp(col, greyCol, mask_final); // Mask is either 0 (color) or 1 (greyscale)
                //return half(mask_final);
            }
            ENDHLSL
        }
    }
}