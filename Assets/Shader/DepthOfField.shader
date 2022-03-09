Shader "Custom/DepthOfField"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Near("Near", Float) = 1
		_Far("Far", Float) = 1000
		_BlurSize("Blur Size", Float) = 1
	}

	SubShader
	{
		Pass
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct a2v 
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv[9] : TEXCOORD0;
			};

			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;
			float _Near;
			float _Far;
			float _BlurSize;

			v2f vert(a2v i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.position);
				o.uv[0] = i.uv;
				o.uv[1] = i.uv + _MainTex_TexelSize.xy * float2(-1, -1) * _BlurSize;
				o.uv[2] = i.uv + _MainTex_TexelSize.xy * float2(0, -1) * _BlurSize;
				o.uv[3] = i.uv + _MainTex_TexelSize.xy * float2(1, -1) * _BlurSize;
				o.uv[4] = i.uv + _MainTex_TexelSize.xy * float2(0, -1) * _BlurSize;
				o.uv[5] = i.uv + _MainTex_TexelSize.xy * float2(0, 1) * _BlurSize;
				o.uv[6] = i.uv + _MainTex_TexelSize.xy * float2(1, -1) * _BlurSize;
				o.uv[7] = i.uv + _MainTex_TexelSize.xy * float2(1, 0) * _BlurSize;
				o.uv[8] = i.uv + _MainTex_TexelSize.xy * float2(1, 1) * _BlurSize;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[0]);
				float z = LinearEyeDepth(d);
				float near = sign(z - _Near);
				float far = sign(_Far - z);
				float blur = (1 - near * far) * 8 ;
				float count = blur + 1;
				float4 sum = tex2D(_MainTex, i.uv[0]) / count;
				// 中心像素占0.3，周围像素各占0.0875，避免均值模糊导致变暗
				sum *= 1 + 2 * blur / 8; 
				for(int j = 1; j < count; j++)
				{
					sum += tex2D(_MainTex, i.uv[j]) * 0.0875;
				}
				float4 color = sum;

				return color;
			}

			ENDCG
		}
	}
}