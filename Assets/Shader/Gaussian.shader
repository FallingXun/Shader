Shader "Custom/Gaussian"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 1.0
    }

    SubShader
    {
        
        CGINCLUDE

        struct a2v
        {
            float4 position : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv[5] : TEXCOORD0;
        };

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;    
        float _BlurSize;


        v2f vert_H(a2v i) 
        {
            v2f o;
            o.pos = UnityObjectToClipPos(i.position);
            float2 uv = i.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(_MainTex_TexelSize.x * -1.0, 0.0) * _BlurSize;
            o.uv[2] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[3] = uv + float2(_MainTex_TexelSize.x * -2.0, 0.0) * _BlurSize;
            o.uv[4] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            return o;
        }

        float4 frag(v2f i) : SV_Target
        {
            float weight[3] = {0.4026, 0.2442, 0.0545};
            float3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
            for(int j=1; j<3; ++j)
            {
                sum += tex2D(_MainTex, i.uv[j]).rgb * weight[j];
                sum += tex2D(_MainTex, i.uv[j+2]).rgb * weight[j];
            }
            return float4(sum, 1.0);
        }

        v2f vert_V(a2v i) 
        {
            v2f o;
            o.pos = UnityObjectToClipPos(i.position);
            float2 uv = i.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * -1.0) * _BlurSize;
            o.uv[2] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * -2.0) * _BlurSize;
            o.uv[4] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            return o;
        }

        ENDCG
      
        Pass
        {
            Name "Horizontal"

            CGPROGRAM

            #pragma vertex vert_H
            #pragma fragment frag

            ENDCG
        }

        Pass
        {
            Name "Vertical"

            
            CGPROGRAM

            #pragma vertex vert_V
            #pragma fragment frag

            ENDCG
        }
    } 
}
