Shader "Game/Outline Fill"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 4
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+110" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "OutlineFill"
            Cull Off
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            float4 Vertex(Attributes input) : SV_POSITION
            {
                float3 positionVS = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
                float3 normalVS = TransformWorldToViewDir(TransformObjectToWorldNormal(input.normalOS), true);
                positionVS += normalVS * -positionVS.z * _OutlineWidth * 0.001;
                return TransformWViewToHClip(positionVS);
            }

            half4 Fragment() : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
