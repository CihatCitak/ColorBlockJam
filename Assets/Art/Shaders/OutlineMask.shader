Shader "Game/Outline Mask"
{
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "OutlineMask"
            Cull Off
            ZTest Always
            ZWrite Off
            ColorMask 0

            Stencil
            {
                Ref 1
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 Vertex(float4 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS.xyz);
            }

            half4 Fragment() : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
