Shader "Unlit/projection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

#define PI 3.141592653589793238462f

			float2 azimuthalToEquirectangular(float2 uv) {
				// to center the circle properly
				float2 coord = 4 * uv - 2;

				float radius = length(coord);
				float angle = atan2(coord.y, coord.x) + PI;

				float lat = angle;
				float lon = 2 * acos(radius / 2.) -PI / 2;
				return float2(lat, lon);
			}

            fixed4 frag (v2f i) : SV_Target	
            {
				float2 coord = azimuthalToEquirectangular(i.uv);

				// equirectangular to mercator
				float x = coord.x;
				float y = log(tan(PI / 4. + coord.y / 2.));

				x = x / (PI * 2);
				y = (y + PI) / (PI * 2.1);

				fixed4 col = tex2D(_MainTex, float2(x,y));

				// everything outside our projections sould be white
				col = length(i.uv * 2 - 1) > 1 ? 1 : col;

				return col;
            }
            ENDCG
        }
    }
}