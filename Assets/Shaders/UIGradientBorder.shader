Shader "UI/GradientBorder"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Gradient Settings)]
        [Toggle] _UseGradient ("Use Gradient", Float) = 1
        _GradientColor1 ("Gradient Color 1", Color) = (1,0.2,0.8,1)
        _GradientColor2 ("Gradient Color 2", Color) = (0.2,0.8,1,1)
        _GradientAngle ("Gradient Angle", Range(0, 360)) = 45
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _GradientSharpness ("Gradient Sharpness", Range(0.1, 5)) = 1

        [Header(Border Settings)]
        _BorderWidth ("Border Width", Range(0, 0.5)) = 0.02
        _BorderSoftness ("Border Softness", Range(0, 0.1)) = 0.01

        [Header(Animation)]
        _AnimationSpeed ("Animation Speed", Float) = 0.5
        _PulseSpeed ("Pulse Speed", Float) = 1.0

        [Header(UI)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            // UI Clipping function
            inline float UnityGet2DClipping(float2 position, float4 clipRect)
            {
                float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
                return inside.x * inside.y;
            }

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _UseGradient;
                half4 _GradientColor1;
                half4 _GradientColor2;
                float _GradientAngle;
                float _GradientOffset;
                float _GradientSharpness;
                float _BorderWidth;
                float _BorderSoftness;
                float _AnimationSpeed;
                float _PulseSpeed;
                float4 _ClipRect;
            CBUFFER_END

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = v.vertex;
                OUT.vertex = TransformObjectToHClip(v.vertex.xyz);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;

                return OUT;
            }

            // Helper function to rotate UV coordinates
            float2 RotateUV(float2 uv, float angleDegrees)
            {
                float angleRadians = radians(angleDegrees);
                float cosAngle = cos(angleRadians);
                float sinAngle = sin(angleRadians);

                // Center UVs around origin
                float2 centeredUV = uv - 0.5;

                // Apply rotation matrix
                float2 rotatedUV;
                rotatedUV.x = centeredUV.x * cosAngle - centeredUV.y * sinAngle;
                rotatedUV.y = centeredUV.x * sinAngle + centeredUV.y * cosAngle;

                // Return to 0-1 range
                return rotatedUV + 0.5;
            }

            half4 frag(v2f IN) : SV_Target
            {
                // Sample texture (default to white if no texture)
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                if (color.a < 0.01) color = half4(1, 1, 1, 1); // Default to white if transparent texture

                // === DIRECTIONAL GRADIENT CALCULATION ===
                half4 gradientColor;

                if (_UseGradient > 0.5)
                {
                    // Rotate UVs based on gradient angle
                    float2 rotatedUV = RotateUV(IN.texcoord, _GradientAngle);

                    // Calculate gradient based on rotated U coordinate (horizontal gradient in rotated space)
                    float gradientValue = rotatedUV.x;

                    // Apply animation offset that flows along gradient direction
                    float animatedOffset = _Time.y * _AnimationSpeed;
                    gradientValue += animatedOffset + _GradientOffset;
                    gradientValue = frac(gradientValue); // Keep in 0-1 range with wrapping

                    // Apply gradient sharpness to control transition between colors
                    gradientValue = pow(gradientValue, _GradientSharpness);

                    // Lerp between the two gradient colors
                    gradientColor = lerp(_GradientColor1, _GradientColor2, gradientValue);
                }
                else
                {
                    // Use solid color (vertex color only) when gradient is disabled
                    gradientColor = half4(1, 1, 1, 1);
                }

                // === BORDER CALCULATION ===
                // Calculate border effect with aspect ratio correction
                float2 uvDDX = ddx(IN.texcoord);
                float2 uvDDY = ddy(IN.texcoord);
                float2 pixelSize = float2(length(uvDDX), length(uvDDY));
                float aspectRatio = pixelSize.y / pixelSize.x;

                // Adjust UV coordinates to account for aspect ratio
                float2 adjustedUV = IN.texcoord;
                adjustedUV.x *= aspectRatio;

                // Calculate distance from edges with aspect correction
                float2 edgeDist = min(adjustedUV, float2(aspectRatio, 1.0) - adjustedUV);
                float minEdgeDist = min(edgeDist.x, edgeDist.y);

                // Apply border width (adjusted for aspect ratio)
                float adjustedBorderWidth = _BorderWidth * aspectRatio;
                float borderMask = smoothstep(adjustedBorderWidth - _BorderSoftness, adjustedBorderWidth + _BorderSoftness, minEdgeDist);

                // === PULSING ANIMATION ===
                // Calculate normalized distance from edge (0 at edge, 1 at border width)
                float normalizedEdgeDist = minEdgeDist / adjustedBorderWidth;
                normalizedEdgeDist = saturate(normalizedEdgeDist); // Clamp to 0-1

                // Create pulsing wave using sin function
                // The wave travels from edges (0) inward (1)
                float pulsePhase = _Time.y * _PulseSpeed;
                float pulseWave = sin((normalizedEdgeDist + pulsePhase) * 6.28318); // 2*PI for full cycle

                // Map sin wave from [-1, 1] to [0.5, 1.0] for alpha modulation
                float pulseAlpha = 0.75 + pulseWave * 0.25; // Creates breathing effect between 50% and 100%

                // Make center transparent, only show border
                float alpha = (1.0 - borderMask) * pulseAlpha;

                // === FINAL COLOR COMPOSITION ===
                // Multiply gradient color by vertex color (preserves random color variation from DrawRect.cs)
                // Then multiply by texture color
                half4 finalColor = gradientColor * IN.color * color;
                finalColor.a = IN.color.a * alpha;

                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDHLSL
        }
    }
}
