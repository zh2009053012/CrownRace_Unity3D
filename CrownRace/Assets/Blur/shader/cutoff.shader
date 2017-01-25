Shader "Custom/cutoff" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
		CGPROGRAM
			//#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _LumCutoff;
			float _Lum;
			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base i)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, i.texcoord.xy);
				return o;
			}
			float4 frag(v2f i):COLOR
			{
				float4 result;
				result = tex2D(_MainTex, i.uv.xy);
				float lum = dot(result.rgb, float3(0.33,0.33,0.33))+result.a*0.05;
				result.rgb *= max(0, lum-_LumCutoff)*_Lum;
				
				return result;
			}
		ENDCG
		}
	} 
	//FallBack "Diffuse"
}
