Shader "Custom/SDFChange"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _ChangeTex ("Change Texture", 2D) = "white" {}
        _Lerp ("Lerp", Range(0,1)) = 0
        _Thresold("Thresold", Range(0,1)) = 0
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
            sampler2D _ChangeTex;
            float _Lerp;
            float _Thresold;

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
                float a1 = tex2D(_MainTex, i.uv).a - _Thresold;
                float a2 = tex2D(_ChangeTex, i.uv).a - _Thresold;
                float a = a1 * _Lerp + (1 - _Lerp) * a2;
                fixed4 col = fixed4(a,a,a,1);

                return col;
            }
            ENDCG
        }
    }
}
