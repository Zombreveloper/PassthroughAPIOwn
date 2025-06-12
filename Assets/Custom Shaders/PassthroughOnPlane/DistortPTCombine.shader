Shader "Unlit/DistortPTCombine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise("Noise", 2D) = "white" {}
        _StrengthFilter("Strength Filter", 2D) = "white" {}
        _Strength("Distort Strength", float) = 1.0
        _Speed("Distort Speed", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            BlendOp RevSub
            Blend One Zero, Zero Zero

            CGPROGRAM          
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            //#pragma target 3.0
            
            #include "UnityCG.cginc"

            // Properties
            sampler2D _Noise;
            sampler2D _StrengthFilter;
            sampler2D _BackgroundTexture;
            float     _Strength;
            float     _Speed;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float noise = tex2Dlod(_Noise, float4(v.uv, 0)).rgb;
                float3 filt = tex2Dlod(_StrengthFilter, float4(v.uv, 0)).rgb;
                o.uv.x += cos(noise*_Time.x*_Speed) * filt * _Strength;
                o.uv.y += sin(noise*_Time.x*_Speed) * filt * _Strength;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {/*
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;*/
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}


/*Shader "Unlit/DistortPTCombine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Wave Amplitude", Float) = 0.02
        _Frequency ("Wave Frequency", Float) = 20.0
        _Speed ("Wave Speed", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            BlendOp RevSub
            Blend One Zero, Zero Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Amplitude;
            float _Frequency;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;
                float wave = sin(i.uv.y * _Frequency + time * _Speed) * _Amplitude;
                float2 distortedUV = float2(i.uv.x + wave, i.uv.y);

                //fixed4 col = tex2D(_MainTex, distortedUV);
                //return col;

                fixed4 col = tex2D(_MainTex, distortedUV);
                fixed3 shifted = col.rgb;
                shifted.r += 0.2 * sin(i.uv.y * 30 + _Time.y * 5); // Rote Welle sichtbar machen
                return float4(shifted, 0);

            }
            ENDCG
        }
    }
}*/


/*

//Das ist meine alte Patchworkversion vom Shader. Vllt funktioniert aber auch die oben von GPT. Diese ist jedenfalls noch nicht fertig

Shader "Unlit/DistortPTCombine"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TimeScale("Time Scale", Float) = 1.0
        _DistortionStrength("Distortion Strength", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            BlendOp RevSub
            Blend One Zero, Zero Zero

            CGPROGRAM          
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 3.0
                     
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                float2 uv = i.uv;
                uv.y += sin(uv.x * 20 + _Time.y * _TimeScale) * _DistortionStrength;

                fixed4 col = tex2D(_MainTex, uv);

                return col;

                //Hier irgendwo Kamerabild mit Distortion verbinden

                //return float4(0, 0, 0, 0); //Ursprünglicher Return für reines Passthrough


            }
            ENDCG
        }
    }
}*/