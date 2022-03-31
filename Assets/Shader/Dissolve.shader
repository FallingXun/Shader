Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "black" {}
        _Threshold("Threshold", Range(0.0, 1.0)) = 0.0
        _DissoveColor("DissoveColor", Color) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { 
            "Queue" = "Transparent"
            "RendetType" = "Transparent"
        }

        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass{

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            sampler2D _NoiseTex;
            float _Threshold;
            fixed4 _DissoveColor;

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 noiseColor = tex2D(_NoiseTex, i.uv);
                float alpha = step(_Threshold, noiseColor.a - 0.0001);
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                fixed3 finalColor = lerp(_DissoveColor.rgb, mainColor.rgb , alpha);
                return fixed4(finalColor, alpha);
            }

            ENDCG
        }
    }
}