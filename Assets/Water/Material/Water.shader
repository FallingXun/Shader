Shader "Custom/Water"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		_SColor("SpecColor", Color) = (1,1,1,1)
		_BumpMap("NormalMap",2D) = "bump"{}
		_BumpScale("Scale", Float) = 1
		_Gloss("Gloss", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 lightDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
            };

			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			float4 _Color;
			float _Gloss;
			float4 _SColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv * _BumpMap_ST.xy + _BumpMap_ST.zw;
				float3 normal = normalize(v.normal);
				float3 tangent = normalize(v.tangent.xyz);
				float3 biNormal = cross(normal, tangent) * v.tangent.w;
				float3x3 objectToTangent = float3x3(tangent, biNormal, normal);
				o.lightDir = mul(objectToTangent, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(objectToTangent, ObjSpaceViewDir(v.vertex)).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float3 tangentViewDir = normalize(i.viewDir);
				float3 tangentLightDir = normalize(i.lightDir);
				float4 packNormal = tex2D(_BumpMap, i.uv + float2(frac(_Time.y*0.1), frac(_Time.y*0.2)));
				fixed3 tangentNormal = UnpackNormal(packNormal);
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				fixed3 albedo = _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));
				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				fixed3 specular = _LightColor0.rgb * _SColor.rgb * pow(max(0, dot(tangentNormal, halfDir)), _Gloss);
				fixed4 color = fixed4(ambient + diffuse + specular, 1);
				return color;
            }
            ENDCG
        }
    }
}
