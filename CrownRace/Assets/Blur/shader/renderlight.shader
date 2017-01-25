Shader "Custom/renderlight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base i)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.texcoord;
				return o;
			}
			float4 frag(v2f i):COLOR
			{
				float4 result;
				result = tex2D(_MainTex, i.uv);
				result.rgb = result.rgb*result.a;
				result.a = 1 - result.a;
				return result;
			}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
