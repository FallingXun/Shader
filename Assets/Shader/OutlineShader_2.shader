Shader "Custom/OutlineShader_2"
{
    Properties
    {
        _Color ("MainColor", Color) = (1,1,1,1)
        _Outline ("Outline", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }


		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
		
			struct v2f
			{
				float4 pos : SV_POSITION;
			};

            float _Outline;

			v2f vert(appdata v)
			{
				v2f o;

				float3 viewNormal = mul(UNITY_MATRIX_MV, v.normal);
                viewNormal.z = -0.5;
                viewNormal = normalize(viewNormal);
				float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex) + float4(viewNormal, 0) * _Outline;

                o.pos = mul(UNITY_MATRIX_P, viewPos);
 
                return o;
			}

			half4 frag(v2f v) : SV_Target
			{
				half4 color = half4(0,0,0,1);
				return color;
			}

			ENDCG
		}


        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
