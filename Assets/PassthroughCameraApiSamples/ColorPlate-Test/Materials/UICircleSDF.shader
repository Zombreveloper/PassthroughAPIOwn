Shader "UI/Circle SDF"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0,1)) = 0.45
        _Smoothness ("Edge Smoothness", Range(0.001,0.2)) = 0.02
        _Thickness ("Ring Thickness", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanvasModulateColor"="True" }
        Lighting Off
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;
            float _Smoothness;
            float _Thickness;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv * 2.0 - 1.0; // von (0–1) zu (-1–1)
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float dist = length(i.uv);
                float inner = _Radius * (1.0 - _Thickness);
                float edge0 = inner - _Smoothness;
                float edge1 = _Radius + _Smoothness;

                float alpha = smoothstep(edge1, edge0, dist);
                return half4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}
