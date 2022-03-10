Shader "Custom/Edge"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Edge("Edge", Float) = 1.0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Edge;

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv[9] : TEXCOORD0;
            };

            v2f vert(a2v i)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(i.vertex);
                float2 uv = i.uv;
                o.uv[0] = uv + _MainTex_TexelSize.xy * float2(-1, -1);
                o.uv[1] = uv + _MainTex_TexelSize.xy * float2(0, -1);
                o.uv[2] = uv + _MainTex_TexelSize.xy * float2(1, -1);
                o.uv[3] = uv + _MainTex_TexelSize.xy * float2(-1, 0);
                o.uv[4] = uv ;
                o.uv[5] = uv + _MainTex_TexelSize.xy * float2(1, 0);
                o.uv[6] = uv + _MainTex_TexelSize.xy * float2(1, -1);
                o.uv[7] = uv + _MainTex_TexelSize.xy * float2(1, 0);
                o.uv[8] = uv + _MainTex_TexelSize.xy * float2(1, 1);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float Gx[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1};
                float Gy[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
                float Edge_X = 0;
                float Edge_Y = 0;
                for(int j=0; j<9; ++j)
                {
                    fixed4 col = tex2D(_MainTex, i.uv[j]);
                    Edge_X += (0.2125 * col.r + 0.7154 * col.g + 0.0721 * col.b) * Gx[j];
                    Edge_Y += (0.2125 * col.r + 0.7154 * col.g + 0.0721 * col.b) * Gy[j];
                }

                float Edge = abs(Edge_X) + abs(Edge_Y);
                float weight = sign(Edge - _Edge);
                fixed4 color = fixed4(0,0,0,1) * weight + fixed4(1,1,1,1) * (1 - weight);
                return color;
            }

            ENDCG
        }
    }
}