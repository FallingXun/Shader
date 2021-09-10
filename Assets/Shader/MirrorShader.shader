Shader "Custom/MirrorShader"
{
    Properties
    {
        _MainTex("MainTex",2D) = "white" {}
    }

    SubShader{
        Tags {"RenderType"="Opaque"}

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct a2v{
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert(a2v i){
                v2f o;

                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord.xy, _MainTex);
                o.uv.x = 1 - o.uv.x;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex,i.uv);
                return color;
            }

            ENDCG
        }
    }
    
}