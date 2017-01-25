Shader "Unlit/WaterWave16"
{
	Properties
	{
		_Skybox("Skybox", Cube)=""{}
		_BumpTex ("Bump Texture", 2D) = "white"{}
		_BumpStrength ("Bump strength", Range(0.0, 10.0)) = 1.0
		_BumpDirection ("Bump direction(2 wave)", Vector)=(1,1,1,-1)
		_BumpTiling ("Bump tiling", Vector)=(0.0625,0.0625,0.0625,0.0625)
		_Params("Reflect&Refract/Specular tweek&power", Vector)=(1,0.05,0.55,256)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
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
			#pragma multi_compile_fwdbase
			#pragma multi_compile  BLUR_OFF BLUR_ON
			#include "UnityCG.cginc"
			#include "WaterTools.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 screenPos:TEXCOORD0;
				float4 bumpCoords:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				//float3 worldNormal:TEXCOORD3;
				UNITY_FOG_COORDS(3)
				float4 vertex : SV_POSITION;
			};
			samplerCUBE _Skybox;
			float4 _QA0,_QA1,_QA2,_QA3;
			float4 _A0,_A1,_A2,_A3;
			float4 _S0,_S1,_S2,_S3;
			float4 _Dx0,_Dx1,_Dx2,_Dx3;
			float4 _Dz0,_Dz1,_Dz2,_Dz3;
			float4 _W0,_W1,_W2,_W3;
			float4 _Params;//x:reflect  y:refract  z:spec tweek  w:spec power
			float3 _SunRadiance;
			//
			sampler2D _BumpTex;
			float _BumpStrength;
			float4 _BumpDirection;
			float4 _BumpTiling;
			//
			sampler2D _ReflTexture;
			//sampler2D _AtmosphereScatteringSource;
			float4 _SourceTex_TexelSize;
			sampler2D _RefrTexture;
			sampler2D _BlurTexH;
			sampler2D _BlurTexV;
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
				float4 phase0 = _Dx0*vert.x+_Dz0*vert.z+_S0*_Time.y;
				float4 phase1 = _Dx1*vert.x+_Dz1*vert.z+_S1*_Time.y;
				float4 phase2 = _Dx2*vert.x+_Dz2*vert.z+_S2*_Time.y;
				float4 phase3 = _Dx3*vert.x+_Dz3*vert.z+_S3*_Time.y;
				
				float4 sinp0=float4(0,0,0,0), cosp0=float4(0,0,0,0);
				float4 sinp1=float4(0,0,0,0), cosp1=float4(0,0,0,0);
				float4 sinp2=float4(0,0,0,0), cosp2=float4(0,0,0,0);
				float4 sinp3=float4(0,0,0,0), cosp3=float4(0,0,0,0);
				
				sincos(_W0*phase0, sinp0, cosp0);
				sincos(_W1*phase1, sinp1, cosp1);
				sincos(_W2*phase2, sinp2, cosp2);
				sincos(_W3*phase3, sinp3, cosp3);

				pos.x = dot(_QA0*_Dx0, cosp0)+ dot(_QA1*_Dx1, cosp1)+ dot(_QA2*_Dx2, cosp2)+ dot(_QA3*_Dx3, cosp3);
				pos.z = dot(_QA0*_Dz0, cosp0)+dot(_QA1*_Dz1, cosp1)+dot(_QA2*_Dz2, cosp2)+dot(_QA3*_Dz3, cosp3);
				pos.y = dot(_A0, sinp0)+dot(_A1, sinp1)+dot(_A2, sinp2)+dot(_A3, sinp3);

				return pos;
			}
			
			float3 CalculateWavesNormal(float3 vert)
			{
				float3 nor = float3(0,0,0);
				float4 phase0 = _Dx0*vert.x+_Dz0*vert.z+_S0*_Time.y;
				float4 phase1 = _Dx1*vert.x+_Dz1*vert.z+_S1*_Time.y;
				float4 phase2 = _Dx2*vert.x+_Dz2*vert.z+_S2*_Time.y;
				float4 phase3 = _Dx3*vert.x+_Dz3*vert.z+_S3*_Time.y;
				
				float4 sinp0=float4(0,0,0,0), cosp0=float4(0,0,0,0);
				float4 sinp1=float4(0,0,0,0), cosp1=float4(0,0,0,0);
				float4 sinp2=float4(0,0,0,0), cosp2=float4(0,0,0,0);
				float4 sinp3=float4(0,0,0,0), cosp3=float4(0,0,0,0);
				
				sincos(_W0*phase0, sinp0, cosp0);
				sincos(_W1*phase1, sinp1, cosp1);
				sincos(_W2*phase2, sinp2, cosp2);
				sincos(_W3*phase3, sinp3, cosp3);

				nor.x = -dot(_A0*_Dx0, cosp0)-dot(_A1*_Dx1, cosp1)-dot(_A2*_Dx2, cosp2)-dot(_A3*_Dx3, cosp3);
				nor.z = -dot(_A0*_Dz0, cosp0)-dot(_A1*_Dz1, cosp1)-dot(_A2*_Dz2, cosp2)-dot(_A3*_Dz3, cosp3);
				nor.y = 1-dot(_QA0*_W0, sinp0)-dot(_QA1*_W1, sinp1)-dot(_QA2*_W2, sinp2)-dot(_QA3*_W3, sinp3);

				nor = normalize(nor);

				return nor;
			}
			/*
			float3 CalculateWavesDisplacementNormal(float3 vert, out float3 nor)
			{
				float3 pos = float3(0,0,0);
				float4 phase0 = _Dx0*vert.x+_Dz0*vert.z+_S0*_Time.y;
				float4 phase1 = _Dx1*vert.x+_Dz1*vert.z+_S1*_Time.y;
				float4 phase2 = _Dx2*vert.x+_Dz2*vert.z+_S2*_Time.y;
				float4 phase3 = _Dx3*vert.x+_Dz3*vert.z+_S3*_Time.y;
				
				float4 sinp0=float4(0,0,0,0), cosp0=float4(0,0,0,0);
				float4 sinp1=float4(0,0,0,0), cosp1=float4(0,0,0,0);
				float4 sinp2=float4(0,0,0,0), cosp2=float4(0,0,0,0);
				float4 sinp3=float4(0,0,0,0), cosp3=float4(0,0,0,0);
				
				sincos(_W0*phase0, sinp0, cosp0);
				sincos(_W1*phase1, sinp1, cosp1);
				sincos(_W2*phase2, sinp2, cosp2);
				sincos(_W3*phase3, sinp3, cosp3);

				pos.x = dot(_QA0*_Dx0, cosp0)+ dot(_QA1*_Dx1, cosp1)+ dot(_QA2*_Dx2, cosp2)+ dot(_QA3*_Dx3, cosp3);
				pos.z = dot(_QA0*_Dz0, cosp0)+dot(_QA1*_Dz1, cosp1)+dot(_QA2*_Dz2, cosp2)+dot(_QA3*_Dz3, cosp3);
				pos.y = dot(_A0, sinp0)+dot(_A1, sinp1)+dot(_A2, sinp2)+dot(_A3, sinp3);

				nor.x = -dot(_W0*_A0*_Dx0, cosp0)-dot(_W1*_A1*_Dx1, cosp1)-dot(_W2*_A2*_Dx2, cosp2)-dot(_W3*_A3*_Dx3, cosp3);
				nor.z = -dot(_W0*_A0*_Dz0, cosp0)-dot(_W1*_A1*_Dz1, cosp1)-dot(_W2*_A2*_Dz2, cosp2)-dot(_W3*_A3*_Dz3, cosp3);
				nor.y = 1-dot(_QA0*_W0, sinp0)-dot(_QA1*_W1, sinp1)-dot(_QA2*_W2, sinp2)-dot(_QA3*_W3, sinp3);

				nor = normalize(nor);

				return pos;
			}
			
			float3 CalculateWavesDisplacementBinormalTangent(float3 vert, out float3 binormal, out float3 tangent)
			{
				float3 pos = float3(0,0,0);
				
				float4 phase0 = _Dx0*vert.x+_Dz0*vert.z+_S0*_Time.y;
				float4 phase1 = _Dx1*vert.x+_Dz1*vert.z+_S1*_Time.y;
				float4 phase2 = _Dx2*vert.x+_Dz2*vert.z+_S2*_Time.y;
				float4 phase3 = _Dx3*vert.x+_Dz3*vert.z+_S3*_Time.y;
				
				float4 sinp0=float4(0,0,0,0), cosp0=float4(0,0,0,0);
				float4 sinp1=float4(0,0,0,0), cosp1=float4(0,0,0,0);
				float4 sinp2=float4(0,0,0,0), cosp2=float4(0,0,0,0);
				float4 sinp3=float4(0,0,0,0), cosp3=float4(0,0,0,0);
				
				sincos(_W0*phase0, sinp0, cosp0);
				sincos(_W1*phase1, sinp1, cosp1);
				sincos(_W2*phase2, sinp2, cosp2);
				sincos(_W3*phase3, sinp3, cosp3);

				pos.x = dot(_QA0*_Dx0, cosp0)+ dot(_QA1*_Dx1, cosp1)+ dot(_QA2*_Dx2, cosp2)+ dot(_QA3*_Dx3, cosp3);
				pos.z = dot(_QA0*_Dz0, cosp0)+dot(_QA1*_Dz1, cosp1)+dot(_QA2*_Dz2, cosp2)+dot(_QA3*_Dz3, cosp3);
				pos.y = dot(_A0, sinp0)+dot(_A1, sinp1)+dot(_A2, sinp2)+dot(_A3, sinp3);

				binormal = float3(0,0,0);
				binormal.x = 1-dot(_QA0, _Dx0*sinp0*_Dx0*_W0)-dot(_QA1, _Dx1*sinp1*_Dx1*_W1)-dot(_QA2, _Dx2*sinp2*_Dx2*_W2)-dot(_QA3, _Dx3*sinp3*_Dx3*_W3);
				binormal.z = -dot(_QA0, _Dz0*sinp0*_Dz0*_W0)-dot(_QA1, _Dz1*sinp1*_Dz1*_W1)-dot(_QA2, _Dz2*sinp2*_Dz2*_W2)-dot(_QA3, _Dz3*sinp3*_Dz3*_W3);
				binormal.y = dot(_A0, _Dx0*cosp0*_W0)+dot(_A1, _Dx1*cosp1*_W1)+dot(_A2, _Dx2*cosp2*_W2)+dot(_A3, _Dx3*cosp3*_W3);
				
				tangent = float3(0,0,0);
				tangent.x = -dot(_QA0, _Dx0*sinp0*_Dz0*_W0)-dot(_QA1, _Dx1*sinp1*_Dz1*_W1)-dot(_QA2, _Dx2*sinp2*_Dz2*_W2)-dot(_QA3, _Dx3*sinp3*_Dz3*_W3);
				tangent.z = 1-dot(_QA0, _Dz0*sinp0*_Dz0*_W0)-dot(_QA1, _Dz1*sinp1*_Dz1*_W1)-dot(_QA2, _Dz2*sinp2*_Dz2*_W2)-dot(_QA3, _Dz3*sinp3*_Dz3*_W3);
				tangent.y = dot(_A0, _Dz0*cosp0*_W0)+dot(_A1, _Dz1*cosp1*_W1)+dot(_A2, _Dz2*cosp2*_W2)+dot(_A3, _Dz3*cosp3*_W3);

				binormal = normalize(binormal);
				tangent = normalize(tangent);

				return pos;
			}
			*/
			void CalculateWavesBinormalTangent(float3 vert, out float3 binormal, out float3 tangent)
			{
				float4 phase0 = _Dx0*vert.x+_Dz0*vert.z+_S0*_Time.y;
				float4 phase1 = _Dx1*vert.x+_Dz1*vert.z+_S1*_Time.y;
				float4 phase2 = _Dx2*vert.x+_Dz2*vert.z+_S2*_Time.y;
				float4 phase3 = _Dx3*vert.x+_Dz3*vert.z+_S3*_Time.y;
				
				float4 sinp0=float4(0,0,0,0), cosp0=float4(0,0,0,0);
				float4 sinp1=float4(0,0,0,0), cosp1=float4(0,0,0,0);
				float4 sinp2=float4(0,0,0,0), cosp2=float4(0,0,0,0);
				float4 sinp3=float4(0,0,0,0), cosp3=float4(0,0,0,0);
				
				sincos(_W0*phase0, sinp0, cosp0);
				sincos(_W0*phase1, sinp1, cosp1);
				sincos(_W0*phase2, sinp2, cosp2);
				sincos(_W0*phase3, sinp3, cosp3);

				binormal = float3(0,0,0);
				binormal.x = 1-dot(_QA0, _Dx0*sinp0*_Dx0*_W0)-dot(_QA1, _Dx1*sinp1*_Dx1*_W1)-dot(_QA2, _Dx2*sinp2*_Dx2*_W2)-dot(_QA3, _Dx3*sinp3*_Dx3*_W3);
				binormal.z = -dot(_QA0, _Dz0*sinp0*_Dz0*_W0)-dot(_QA1, _Dz1*sinp1*_Dz1*_W1)-dot(_QA2, _Dz2*sinp2*_Dz2*_W2)-dot(_QA3, _Dz3*sinp3*_Dz3*_W3);
				binormal.y = dot(_A0, _Dx0*cosp0*_W0)+dot(_A1, _Dx1*cosp1*_W1)+dot(_A2, _Dx2*cosp2*_W2)+dot(_A3, _Dx3*cosp3*_W3);
				
				tangent = float3(0,0,0);
				tangent.x = -dot(_QA0, _Dx0*sinp0*_Dz0*_W0)-dot(_QA1, _Dx1*sinp1*_Dz1*_W1)-dot(_QA2, _Dx2*sinp2*_Dz2*_W2)-dot(_QA3, _Dx3*sinp3*_Dz3*_W3);
				tangent.z = 1-dot(_QA0, _Dz0*sinp0*_Dz0*_W0)-dot(_QA1, _Dz1*sinp1*_Dz1*_W1)-dot(_QA2, _Dz2*sinp2*_Dz2*_W2)-dot(_QA3, _Dz3*sinp3*_Dz3*_W3);
				tangent.y = dot(_A0, _Dz0*cosp0*_W0)+dot(_A1, _Dz1*cosp1*_W1)+dot(_A2, _Dz2*cosp2*_W2)+dot(_A3, _Dz3*cosp3*_W3);

				binormal = normalize(binormal);
				tangent = normalize(tangent);
			}
			
			inline float4 MyComputeScreenPos (float4 pos) {
				float4 o = pos * 0.5f;
				#if defined(UNITY_HALF_TEXEL_OFFSET)
				o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w * _ScreenParams.zw;
				#else
				o.xy = float2(o.x, o.y*_ProjectionParams.x) + o.w;
				#endif

				#if defined(SHADER_API_FLASH)
				o.xy *= unity_NPOTScale.xy;
				#endif
				
				o.zw = pos.zw;
				return o;
			}

			v2f vert (appdata v)
			{
				v2f o;
				float3 worldPos = mul(_Object2World, v.vertex);
				float3 worldNormal = float3(0,0,0);
				float3 disPos = CalculateWavesDisplacement(worldPos);
				worldPos = worldPos+disPos;
				v.vertex.xyz = mul(_World2Object, float4(worldPos, 1));
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.screenPos = ComputeScreenPos(mul(_RefractCameraVP, float4(worldPos, 1))).xyw;
				//o.screenPos = MyComputeScreenPos(o.vertex).xyw;
				o.worldPos = worldPos;
				o.bumpCoords.xyzw = (worldPos.xzxz + _Time.yyyy * _BumpDirection.xyzw) * _BumpTiling.xyzw;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 result = float4(0,0,0,1);
				float3 viewVector = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
				//calculate normal
				float3 binormal = float3(0,0,0);
				float3 tangent = float3(0,0,0);
				CalculateWavesBinormalTangent(i.worldPos, binormal, tangent);
				float3 worldNormal = normalize(cross(tangent, binormal));
				float3x3 M = {binormal, worldNormal, tangent};
				M= transpose(M);
				float3 bumpNormal = PerPixelNormal(_BumpTex, i.bumpCoords, _BumpStrength);
				worldNormal = normalize( mul(M, bumpNormal));
				//worldNormal = normalize(CalculateWavesNormal(i.worldPos));
				
				float fresnel = FastFresnel(-viewVector, worldNormal, 0.0204f);
				float2 offsets = worldNormal.xz*viewVector.y;
				float2 screenUV = i.screenPos.xy/i.screenPos.z;
				#if UNITY_UV_STARTS_AT_TOP && BLUR_ON
				if (_SourceTex_TexelSize.y > 0)
					screenUV.y = 1-screenUV.y;
				#endif

		
				float4 reflectionColor = tex2D(_ReflTexture, screenUV+offsets*_Params.x);
				float4 refractionColor = tex2D(_RefrTexture, screenUV+offsets*_Params.y);
				float3 reflUV = reflect( viewVector, worldNormal);
				float3 skyColor = texCUBE(_Skybox, reflUV);
				float3 blurH = tex2D(_BlurTexH, screenUV);
				float3 blurV = tex2D(_BlurTexV, screenUV);
				
				reflectionColor.xyz = lerp(skyColor, reflectionColor.xyz, reflectionColor.a);//reflectionColor.xyz+(1-reflectionColor.a)*skyColor;
				
				result = lerp(refractionColor, reflectionColor, fresnel);
				//specular
				worldNormal.y *= _Params.z;
				worldNormal = normalize(worldNormal);
				reflUV = reflect( viewVector, worldNormal);
				float dotRS = dot(reflUV, normalize(_WorldSpaceLightPos0.xyz));
				float spec = pow( saturate(dotRS), _Params.w);
				result.xyz += spec*_SunRadiance ;//+ blurH + blurV;
				//result.xyz = reflectionColor.xyz;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, result);
				return result;
			}
			ENDCG
		}
	}
}
