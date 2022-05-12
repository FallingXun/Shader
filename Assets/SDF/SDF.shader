
Shader "Custom/SDF"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Distance("Distance", Float) = 0
		_Smooth("Smooth", Float) = 0
	}

	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Distance;
			float _Smooth;

			struct a2v{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color =  tex2D(_MainTex, i.uv);
				float distance = color.a;
				color.a = smoothstep(_Distance - _Smooth, _Distance + _Smooth, distance);
				return float4(1, 1, 1, color.a);
			}

			ENDCG
		}
	}
}
