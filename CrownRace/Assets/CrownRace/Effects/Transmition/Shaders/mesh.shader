Shader "ParticleScale/Additive" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	_SpaceTex("Space Texture", 2D) = "white"{}
	_DistortTex("Distort Texture", 2D) = "black"{}
	_DistortParam("Move Direction&Distort Power", vector) = (0.25, 0.25, 0.15, 0)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	//Blend SrcAlpha One
	Blend  SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				float4 screenUV:TEXCOORD3;
			};
			
			float4 _MainTex_ST;
			sampler2D _DistortTex;
			float4 _DistortTex_ST;
			float4 _DistortParam;
			sampler2D _SpaceTex;

			v2f vert (appdata_t v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				
				float4 worldPos = mul(_Object2World, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, worldPos);

				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.screenUV = ComputeScreenPos (o.vertex);
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _DistortTex);
				o.texcoord.zw += _DistortParam.xy * _Time.y;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;

			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				half4 distort = tex2D(_DistortTex, i.texcoord.zw);
				half3 nor = UnpackNormal(distort);

				half4 spaceCol = tex2D(_SpaceTex, i.screenUV.xy/i.screenUV.w);
				fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord.xy + nor.xy*_DistortParam.z)*spaceCol;
				//col.a = col.r+col.g+col.b;
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return col;
			}
			ENDCG 
		}
	}	
}
}

