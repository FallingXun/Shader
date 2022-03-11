Shader "XC/XC_Opaque_All"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle]_ZWriteOff("ZWriteOff",int) = 1
        _SrcBlend("SrcBlend" , int) = 1
        _DstBlend("DstBlend" , int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        Lighting Off
        blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWriteOff]
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
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                return col;
            }
            ENDCG
        }
    }
}
