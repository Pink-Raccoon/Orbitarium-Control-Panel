﻿Shader "Co2Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Texture2("Texture2", 2D) = "white" {}
        _Alpha("Alpha", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            sampler2D _Texture2;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_Texture2, i.uv);
                col *= 1 - _Alpha;
                col2 *= _Alpha;

				return col + col2;
            }
            ENDCG
        }
    }
}