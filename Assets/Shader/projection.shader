Shader "Unlit/projection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[Enum(Equirectangular,0,Azimuthal,1)]
		_Azimuthal("Projection", float) = 0
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
			float _Azimuthal;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

#define PI 3.141592653589793238462f
#define PI2 6.283185307179586476924f

			float2 uvToEquirectangular(float2 uv) {
				float lat = (uv.x) * PI2;   // from 0 to 2PI
				float lon = (uv.y - .5f) * PI;  // from -PI to PI
				return float2(lat, lon);
			}

			float2 uvAsAzimuthalToEquirectangular(float2 uv) {
				float2 coord = (uv - .5) * 4;

				float radius = length(coord);
				float angle = atan2(coord.y, coord.x) + PI;

				//formula from https://en.wikipedia.org/wiki/Lambert_azimuthal_equal-area_projection
				float lat = angle;
				float lon = 2 * acos(radius / 2.) - PI / 2;
				return float2(lat, lon);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// get equirectangular coordinates
				float2 coord = _Azimuthal ? uvAsAzimuthalToEquirectangular(i.uv) : uvToEquirectangular(i.uv);

				// equirectangular to mercator
				float x = coord.x;
				float y = log(tan(PI / 4. + coord.y / 2.));
				// brin x,y into [0,1] range
				x = x / PI2;
				y = (y + PI) / PI2;

				fixed4 col = tex2D(_MainTex, float2(x,y));

				// just to make it look nicer
				col = _Azimuthal && length(i.uv * 2 - 1) > 1 ? 1 : col;

				return col;
            }
            ENDCG
        }
    }
}