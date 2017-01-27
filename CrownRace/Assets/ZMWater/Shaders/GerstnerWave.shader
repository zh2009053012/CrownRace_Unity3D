Shader "Unlit/GerstnerWave"
{
	Properties
	{
		_Skybox("Skybox", Cube)=""{}
		_QA("Q(Q1,Q2,Q3,Q4)", Vector)=(1,1,1,1)
		_A("A(A1,A2,A3,A4)", Vector)=(1,1,1,1)
		_Dx("Direction x component (Dx1,Dx2,Dx3,Dx4)", Vector)=(1,1,1,1)
		_Dz("Direction z component (Dz1,Dz2,Dz3,Dz4)", Vector)=(1,1,1,1)
		_S("Speed(S1,S2,S3,S4)", Vector)=(1,1,1,1)
		_L("Length(L1,L2,L3,L4)", Vector)=(1,1,1,1)
		_ReflectPerturb("Reflect perturb", float)=1
		_RefractPerturb("Refract perturb", float)=0.15
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			cull off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 screenPos:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};
			samplerCUBE _Skybox;
			float4 _QA;
			float4 _A;
			float4 _S;
			float4 _Dx;
			float4 _Dz;
			float4 _L;
			float _ReflectPerturb;
			float _RefractPerturb;
			//
			sampler2D _ReflTexture;
			sampler2D _RefrTexture;
			//
			float4x4 _RefractCameraVP;
			/*
			float3 CalculateWavesDisplacement(float3 vert)
			{
				float PI = 3.141592f;
				float3 pos = float3(0,0,0);
				float4 w = 2*PI/_L;
				float4 psi = _S*2*PI/_L;
				float4 phase = w*_Dx*vert.x+w*_Dz*vert.z+psi*_Time.y;
				float4 sinp=float4(0,0,0,0), cosp=float4(0,0,0,0);
				sincos(phase, sinp, cosp);

				pos.x = dot(_Q*_A*_Dx, cosp);
				pos.z = dot(_Q*_A*_Dz, cosp);
				pos.y = dot(_A, sinp);

				return pos;
			}
			*/
			float3 CalculateWavesDisplacement(float3 vert)
			{
				float3 pos = float3(0,0,0);
				float4 phase = _Dx*vert.x+_Dz*vert.z+_S*_Time.y;
				float4 sinp=float4(0,0,0,0), cosp=float4(0,0,0,0);
				sincos(_L*phase, sinp, cosp);

				pos.x = dot(_QA*_Dx, cosp);
				pos.z = dot(_QA*_Dz, cosp);
				pos.y = dot(_A, sinp);

				return pos;
			}
			float3 CalculateWavesNormal(float3 vert)
			{
				float3 nor = float3(0,0,0);
				float4 phase = _Dx*vert.x+_Dz*vert.z+_S*_Time.y;
				float4 sinp=float4(0,0,0,0), cosp=float4(0,0,0,0);
				sincos(_L*phase, sinp, cosp);

				nor.x = -dot(_L*_A*_Dx, cosp);
				nor.z = -dot(_L*_A*_Dz, cosp);
				nor.y = 1-dot(_QA*_L, sinp);

				nor = normalize(nor);

				return nor;
			}
			float3 CalculateWavesDisplacementNormal(float3 vert, out float3 nor)
			{
				float3 pos = float3(0,0,0);
				float4 phase = _Dx*vert.x+_Dz*vert.z+_S*_Time.y;
				float4 sinp=float4(0,0,0,0), cosp=float4(0,0,0,0);
				sincos(_L*phase, sinp, cosp);

				pos.x = dot(_QA*_Dx, cosp);
				pos.z = dot(_QA*_Dz, cosp);
				pos.y = dot(_A, sinp);

				nor.x = -dot(_L*_A*_Dx, cosp);
				nor.z = -dot(_L*_A*_Dz, cosp);
				nor.y = 1-dot(_QA*_L, sinp);

				nor = normalize(nor);

				return pos;
			}
			// Fresnel approximation, power = 5
			inline half FastFresnel(float3 I, float3 N, float R0) 
			{
			  float icosIN = saturate(1-dot(I, N));
			  float i2 = icosIN*icosIN, i4 = i2*i2;
			  return R0 + (1-R0)*(i4*icosIN);
			}

			v2f vert (appdata v)
			{
				v2f o;
				float3 worldPos = mul(_Object2World, v.vertex);
				//float3 worldNormal = float3(0,0,0);
				float3 disPos = CalculateWavesDisplacement(worldPos);
				worldPos = worldPos+disPos;
				v.vertex.xyz = mul(_World2Object, float4(worldPos, 1));
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.screenPos = ComputeScreenPos(mul(_RefractCameraVP, float4(worldPos, 1))).xyw;
				//o.screenPos = ComputeScreenPos(o.vertex).xyw;
				o.worldPos = worldPos;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 result = float4(0,0,0,1);
				
				float3 viewVector = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
				float3 worldNormal = CalculateWavesNormal(i.worldPos);
				float fresnel = FastFresnel(-viewVector, worldNormal, 0.02f);
				float2 offsets = worldNormal.xz*viewVector.y;
				float2 screenUV = i.screenPos.xy/i.screenPos.z;
				float4 reflectionColor = tex2D(_ReflTexture, screenUV+offsets*_ReflectPerturb);
				float4 refractionColor = tex2D(_RefrTexture, screenUV+offsets*_RefractPerturb);
				float3 reflUV = reflect( normalize(i.worldPos-_WorldSpaceCameraPos.xyz), worldNormal);
				float3 skyColor = texCUBE(_Skybox, reflUV);
				
				reflectionColor.xyz = lerp(skyColor, reflectionColor.xyz, reflectionColor.a);
				
				result.xyz = lerp(refractionColor.xyz, reflectionColor.xyz, fresnel);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, result);
				return result;
			}
			ENDCG
		}
	}
}
