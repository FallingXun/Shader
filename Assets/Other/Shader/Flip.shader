Shader "Custom/Flip"
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

            o.uv = 1 - i.uv;

            float delta = pow(i.uv.x + i.uv.y , 2) * _Angle;
            float angle = clamp(_Angle * pi + delta, 0, pi);
            float x = (i.vertex.x + _Weight / 2) * cos(angle) * cos(angle) - _Weight / 2;
            float y = (i.vertex.x + _Weight / 2) * sin(angle) * sin(angle);
            float w = sign(i.uv.x - 0.5) * i.vertex.w;
            float4 vertex = float4(x, y, i.vertex.z, w);
            
            o.pos = UnityObjectToClipPos(vertex);

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