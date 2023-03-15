Shader "Custom/Fire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)
		_Color3 ("Color3", Color) = (1,1,1,1)
		_Range("Range", Float) = 1
		_SpeedX("SpeedX", Float) = 1
		_SpeedY ("SpeedY", Float) = 1
		_Pos ("Pos", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

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
			fixed4 _Color1;
			fixed4 _Color2;
			fixed4 _Color3; 
			float _Range;
			float _SpeedX;
			float _SpeedY;
			float4 _Pos;

            v2f vert (appdata v)
            {
                v2f o;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float offsetX = sin(worldPos.x + abs(frac(_Time.y * _SpeedX) - 0.5) * 0.1);
				float offsetY = sin(worldPos.y + abs(frac(_Time.y * _SpeedY) - 0.5) * 0.2);
				worldPos.xy += float2(offsetX, offsetY);
				o.vertex = UnityWorldToClipPos(worldPos);
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

				//fixed4 c = tex2D(_MainTex, float2(frac(_Time.x),i.uv.y));
				fixed4 c = tex2D(_MainTex, i.uv + float2(frac(_Time.x*5),0));

				float2 uv1 = i.uv;
				float2 uv2 = i.uv + float2(0, - 0.183);
				float2 uv3 = lerp(uv1, uv2, c.g);
				float2 uv = uv3 + float2(0, 0.183);

				//uv = uv + float2(frac(_Time.x * 6), 0);

				fixed4 col = tex2D(_MainTex, uv);

				fixed4 color = lerp(_Color1, _Color2, col.b);
				color = lerp(color, _Color3, col.b) * 2;
				color.a = col.r;
				//fixed4 color = fixed4(uv.x, uv.y, 0, 1);
				return color;
            }
            ENDCG
        }
    }
}
