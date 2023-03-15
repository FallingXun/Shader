Shader "Custom/OutlineShader_3"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _OutlineTex ("OutlineTex", 2D) = "black" {}
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry"}

        ZTest Always
        Cull Off
        ZWrite Off

        Pass
        {
            NAME "Outline"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv[9] : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;

            sampler2D _OutlineTex;


            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                half2 uv = v.uv;
                o.uv[0] = uv + _MainTex_TexelSize * half2(-1,-1);
                o.uv[1] = uv + _MainTex_TexelSize * half2(0,-1);
                o.uv[2] = uv + _MainTex_TexelSize * half2(1,-1);
                o.uv[3] = uv + _MainTex_TexelSize * half2(-1,0);
                o.uv[4] = uv ;
                o.uv[5] = uv + _MainTex_TexelSize * half2(1,0);
                o.uv[6] = uv + _MainTex_TexelSize * half2(-1,1);
                o.uv[7] = uv + _MainTex_TexelSize * half2(0,1);
                o.uv[8] = uv + _MainTex_TexelSize * half2(-1,1);
                return o;
            }

            half luminance(half4 color)
            {
                return color.r * 0.2125 + color.g * 0.7154 + color.g * 0.0721 + color.b;
            }

            half Sobel(v2f v)
            {
                const half Gx[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1};
                const half Gy[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
                
                half x = 0;
                half y = 0;
                half texColor;
                for(int i=0; i<9; ++i)
                {
                    texColor = luminance(tex2D(_MainTex, v.uv[i]));
                    x += texColor * Gx[i];
                    y += texColor * Gy[i];
                }

                half outline = 1 - abs(x) - abs(y);

                return outline;
            } 

            half4 frag(v2f v) : SV_Target
            {
                half outline = Sobel(v);
                half4 outlineCol = half4(0,0,0,1);
                // half4 mainCol = tex2D(_MainTex, v.uv[4]);
                return lerp(outlineCol, half4(0,0,0,0), outline);
            }

            ENDCG
        }

        Pass
        {
            NAME "Combine"



            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
            };


            sampler2D _MainTex;
            half4 _MainTex_TexelSize;

            sampler2D _OutlineTex;

            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            half4 frag(v2f v) : SV_Target
            {
                half4 outlineCol = tex2D(_OutlineTex, v.uv);
                half4 mainCol = tex2D(_MainTex, v.uv);
                half alpha = mainCol.a;
                half l = outlineCol.a;
                return half4(lerp(mainCol, outlineCol, l).rgb,alpha);
            }

            ENDCG
        }
    }
}