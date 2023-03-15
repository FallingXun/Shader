Shader "Custom/Flip2"
{
 Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Angle("AngleX", Range(0, 1)) = 0
        _Weight("_Weight", Float) = 10
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        ZWrite Off
        ZTest Always

        CGINCLUDE

        #define pi 3.1415926
            
        sampler2D _MainTex;
        half4 _MainTex_ST;
        float _Angle;
        float _Weight;

        struct a2v{
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f{
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };


        v2f vert_flip(a2v i)
        {
            v2f o;

            float2 uv = 1 - i.uv;

            float delta = _Angle;
            float angle = clamp(_Angle * pi + delta, 0, pi);
            float c = cos(angle) * saturate(uv.y - _Angle);
            float s = sin(angle) * saturate(uv.y - _Angle);
            
            o.uv = float2(uv.x * c + uv.y * s, uv.x * s + uv.y * c); 
            o.pos = UnityObjectToClipPos(i.vertex);

            return o;
        }

        fixed4 frag_flip(v2f i) : SV_Target
        {
            fixed4 color = tex2D(_MainTex, i.uv);

            return color;
        }

        ENDCG

        Pass
        {
            Cull Off

            CGPROGRAM

            #pragma vertex vert_flip
            #pragma fragment frag_flip

            #include "UnityCG.cginc"


            ENDCG

        }

    }
}