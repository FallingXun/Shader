Shader "Custom/StencilShaderMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_Stencil ("_Stencil", Int) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue"="Geometry-1"}
		ColorMask 0
		ZWrite Off
		Stencil{
			Ref [_Stencil]
			Comp Always
			Pass Replace
		}

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
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = fixed4(0,0,0,0);
                return col;
            }
            ENDCG
        }
    }
}
