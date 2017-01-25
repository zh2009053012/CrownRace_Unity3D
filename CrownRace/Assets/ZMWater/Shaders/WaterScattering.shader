Shader "Unlit/WaterScattering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
//		_kd("water scattering kd", Vector) = ( .3867, .1055, .0469, 0)
//		_attenuation_c("water scattering attenuation", Vector) = ( .45, .1718, .1133, 0)
//		_diffuseRadiance("water diffuse radiance", Color) = ( .0338, .1015, .2109, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "WaterTools.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 screenPos:TEXCOORD1;
				float3 outScattering:TEXCOORD2;
				float3 inScattering:TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _UnScatteringRefrTex;
			float3 _WaterScatteringKd;
			float3 _WaterScatteringAttenuation;
			float3 _WaterScatteringDiffuseRadiance;
			float _WaterAltitude;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex).xyw;
				
				float3 worldPos = mul(_Object2World, v.vertex);
				float3 outScattering = float3(1,0,0);
				float3 inScattering = float3(1,0,0);
				float3 viewVector = worldPos - _WorldSpaceCameraPos.xyz;
				float depth = max(0, _WaterAltitude - worldPos.y);
			
				SimpleWaterScattering(length(viewVector), worldPos.xyz, depth, _WaterScatteringDiffuseRadiance, 
								_WaterScatteringAttenuation, _WaterScatteringKd, outScattering, inScattering);
				
				o.outScattering = outScattering;
				o.inScattering = inScattering;
				
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half4 result = half4(0,0,0,1);
				float2 srcUV = i.screenPos.xy/i.screenPos.z;
				
				half4 refractionColor = tex2D(_UnScatteringRefrTex, srcUV);
				result.xyz = refractionColor.xyz*i.outScattering+i.inScattering;
				result.w = refractionColor.w;
			
				return result;
			}
			ENDCG
		}
	}
}
