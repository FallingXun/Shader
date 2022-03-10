Shader "Custom/Shake"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Strength("Strength", Float) = 1.0
        _Pow("Pow" , Float) = 1.0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert;
            #pragma fragment frag;

            #include "UnityCG.cginc"
            
            struct a2v
            {
                float4 position : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Strength;
            float _Pow;

            v2f vert(a2v i)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, i.position);
                worldPos.x += _Strength * sin(_Time.y ) * pow(2, worldPos.y * _Pow);
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                return color;
            }

            ENDCG
        }
    }
}