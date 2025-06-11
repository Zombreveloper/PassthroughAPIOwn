Shader "Custom/PassthroughDistortion"
{
    Properties
    {
        _MainTex("Passthrough Texture", 2D) = "white" {}
        _TimeScale("Time Scale", Float) = 1.0
        _DistortionStrength("Distortion Strength", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _TimeScale;
            float _DistortionStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y += sin(uv.x * 20 + _Time.y * _TimeScale) * _DistortionStrength;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
