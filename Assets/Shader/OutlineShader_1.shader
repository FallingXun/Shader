Shader "Custom/OutlineShader_1"
{
    Properties
    {
        _Color ("MainColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque"  "Queue" = "Geometry"}

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            half4 _Color;

            v2f vert(a2v a)
            {
                v2f v;

                v.pos = UnityObjectToClipPos(a.vertex);
                v.worldNormal = UnityObjectToWorldNormal(a.normal);
                v.worldPos = mul(UNITY_MATRIX_M, a.vertex);

                return v;
            }

            half4 frag(v2f v) : SV_Target
            {
                half4 outlineCol = half4(0,0,0,1);
                float3 worldViewDir = _WorldSpaceCameraPos.xyz - v.worldPos.xyz;
                float percent = step(0.1, dot(v.worldNormal,worldViewDir));
                half4 color = _Color * percent + outlineCol * (1 - percent);
                return color;   
            }

            ENDCG
        }
    }
}
