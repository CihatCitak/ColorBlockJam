Shader "Game/Toon"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.8, 0.82, 0.96, 1)
        _RampThreshold ("Ramp Threshold", Range(0, 1)) = 0.4
        _RampSmoothness ("Ramp Smoothness", Range(0.001, 0.5)) = 0.25
        _EdgeColor ("Edge Color", Color) = (0.52, 0.54, 0.67, 1)
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.58
        _EdgeSmoothness ("Edge Smoothness", Range(0.001, 0.5)) = 0.12
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _ShadowColor;
            half _RampThreshold;
            half _RampSmoothness;
            half4 _EdgeColor;
            half _EdgeThreshold;
            half _EdgeSmoothness;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = position.positionCS;
                output.positionWS = position.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 viewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
                Light light = GetMainLight();

                half halfLambert = dot(normal, light.direction) * 0.5 + 0.5;
                half ramp = smoothstep(_RampThreshold - _RampSmoothness, _RampThreshold + _RampSmoothness, halfLambert);
                half3 color = _BaseColor.rgb * lerp(_ShadowColor.rgb, 1, ramp) * light.color;

                half fresnel = 1 - saturate(dot(normal, viewDirection));
                half edge = smoothstep(_EdgeThreshold - _EdgeSmoothness, _EdgeThreshold + _EdgeSmoothness, fresnel);

                return half4(color * lerp(1, _EdgeColor.rgb, edge), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Vert(float4 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS.xyz);
            }

            half4 Frag() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
