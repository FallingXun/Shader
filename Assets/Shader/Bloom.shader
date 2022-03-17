Shader "Custom/Bloom"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _BloomTex("Bloom Tex", 2D) = "black" {}
        _Threshold("Threshold", Float) = 0.0
        _Intensity("Intensity", Float) = 1.0
        _BlurSize("BlurSize", Float) = 1.0
    }

    SubShader
    {

        CGINCLUDE

        sampler2D _MainTex;
        sampler2D _BloomTex;
        float4 _MainTex_ST;
        float _Threshold;
        float _Intensity;
        float _BlurSize;

        ENDCG

        UsePass "Custom/Gaussian/Horizontal"

        UsePass "Custom/Gaussian/Vertical"

        Pass
        {
            CGPROGRAM

            #pragma vertex vert;
            #pragma fragment frag;
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            v2f vert(appdata data)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(data.vertex);
                o.uv = data.uv.xy;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_BloomTex, i.uv);
                float4 mainColor = tex2D(_MainTex, i.uv);
                float brightness = 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
                color *= saturate(brightness - _Threshold);
                return mainColor + color;
            }

            ENDCG
        }
    }
}