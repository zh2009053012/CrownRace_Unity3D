Shader "CardEffect/CardEffect_ZM"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Color("Color", Color) = (1,1,1,1)
		_MaskTex("Mask Texture", 2D) = "white" {}

		_BlendMode1("blend mode 1", float) = 0.0
		_IsMove1("is open move 1", int) = 0
		_IsRotate1("is open rotate 1", int) = 0

		_MotionTex1("Motion Texture 1", 2D) = "black" {}
		_MotionColor1("Motion Color 1", Color) = (1,1,1,0)
		_ColorParam1("Color change",vector) = (1,0,1,1)
		_DistortParam1("Distort1 power", float) = 0.015
		_MoveParam1("Move dir", vector) = (0,0,0,0)
		_RotateParam1("Rotate Center&Speed", vector)=(0,0,0,0)

		_BlendMode2("blend mode 2", float) = 0.0
		_IsMove2("is open move 2", int) = 0
		_IsRotate2("is open rotate 2", int) = 0

		_MotionTex2("Motion Texture 2", 2D) = "black" {}
		_MotionColor2("Motion Color 2", Color) = (1,1,1,0)
		_ColorParam2("Color change",vector) = (1,0,1,1)
		_DistortParam2("Distort power", float) = 0.015
		_MoveParam2("Move dir", vector) = (0,0,0,0)
		_RotateParam2("Rotate Center&Speed", vector)=(0,0,0,0)

		_BlendMode3("blend mode 3", float) = 0.0
		_IsMove3("is open move 3", int) = 0
		_IsRotate3("is open rotate 3", int) = 0

		_MotionTex3("Motion Texture 3", 2D) = "black" {}
		_MotionColor3("Motion Color 3", Color) = (1,1,1,0)
		_ColorParam3("Color change",vector) = (1,0,1,1)
		_DistortParam3("Distort power", float) = 0.015
		_MoveParam3("Move dir", vector) = (0,0,0,0)
		_RotateParam3("Rotate Center&Speed", vector)=(0,0,0,0)

		_BlendMode4("blend mode 4", float) = 0.0
		_IsMove4("is open move 4", int) = 0
		_IsRotate4("is open rotate 4", int) = 0

		_MotionTex4("Motion Texture 4", 2D) = "black" {}
		_MotionColor4("Motion Color 4", Color) = (1,1,1,0)
		_ColorParam4("Color change",vector) = (1,0,1,1)
		_DistortParam4("Distort power", float) = 0.015
		_MoveParam4("Move dir", vector) = (0,0,0,0)
		_RotateParam4("Rotate Center&Speed", vector)=(0,0,0,0)
	}
	SubShader
	{
		offset -1,-1
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _MOTION1_ON
			#pragma shader_feature _MOTION1_BLEND_ADD_ON _MOTION1_BLEND_MINUS_ON _MOTION1_BLEND_MULTIPLY_ON _MOTION1_BLEND_DISTORT_ON
			#pragma shader_feature _MOTION1_MOVE_ON
			#pragma shader_feature _MOTION1_ROTATE_ON

			#pragma shader_feature _MOTION2_ON
			#pragma shader_feature _MOTION2_BLEND_ADD_ON _MOTION2_BLEND_MINUS_ON _MOTION2_BLEND_MULTIPLY_ON _MOTION2_BLEND_DISTORT_ON
			#pragma shader_feature _MOTION2_MOVE_ON
			#pragma shader_feature _MOTION2_ROTATE_ON

			#pragma shader_feature _MOTION3_ON
			#pragma shader_feature _MOTION3_BLEND_ADD_ON _MOTION3_BLEND_MINUS_ON _MOTION3_BLEND_MULTIPLY_ON _MOTION3_BLEND_DISTORT_ON
			#pragma shader_feature _MOTION3_MOVE_ON
			#pragma shader_feature _MOTION3_ROTATE_ON

			#pragma shader_feature _MOTION4_ON
			#pragma shader_feature _MOTION4_BLEND_ADD_ON _MOTION4_BLEND_MINUS_ON _MOTION4_BLEND_MULTIPLY_ON _MOTION4_BLEND_DISTORT_ON
			#pragma shader_feature _MOTION4_MOVE_ON
			#pragma shader_feature _MOTION4_ROTATE_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				#if _MOTION2_ON || _MOTION3_ON
				float4 uv2 : TEXCOORD1;
				#endif
				#if _MOTION4_ON 
				float2 uv3 : TEXCOORD2;
				#endif
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			half4 _Color;
			sampler2D _MaskTex;

			half _BlendMode1;
			bool _IsMove1, _IsRotate1;
			#if _MOTION1_ON
			sampler2D _MotionTex1;
			half4 _MotionTex1_ST;
			half4 _MotionColor1;
			half4 _ColorParam1;
			half _DistortParam1;
			half4 _MoveParam1;
			half4 _RotateParam1;
			#endif

			half _BlendMode2;
			int _IsMove2, _IsRotate2;
			#if _MOTION2_ON
			sampler2D _MotionTex2;
			half4 _MotionTex2_ST;
			half4 _MotionColor2;
			half4 _ColorParam2;
			half _DistortParam2;
			half4 _MoveParam2;
			half4 _RotateParam2;
			#endif

			half _BlendMode3;
			int _IsMove3, _IsRotate3;
			#if _MOTION3_ON
			sampler2D _MotionTex3;
			half4 _MotionTex3_ST;
			half4 _MotionColor3;
			half4 _ColorParam3;
			half _DistortParam3;
			half4 _MoveParam3;
			half4 _RotateParam3;
			#endif

			half _BlendMode4;
			int _IsMove4, _IsRotate4;
			#if _MOTION4_ON
			sampler2D _MotionTex4;
			half4 _MotionTex4_ST;
			half4 _MotionColor4;
			half4 _ColorParam4;
			half _DistortParam4;
			half4 _MoveParam4;
			half4 _RotateParam4;
			#endif

			half4 MyTex2D_4(sampler2D src, half2 uv)
			{
				half4 result = tex2D(src, uv);
				if(IsGammaSpace())
				{
					result.xyzw = pow(result.xyzw, 2.2f);
				}
				return result;
			}
			half3 MyTex2D_3(sampler2D src, half2 uv)
			{
				half3 result = tex2D(src, uv).xyz;
				if(IsGammaSpace())
				{
					result.xyz = pow(result.xyz, 2.2f);
				}
				return result;
			}
			half2 MyTex2D_2(sampler2D src, half2 uv)
			{
				half2 result = tex2D(src, uv).xy;
				if(IsGammaSpace())
				{
					result = pow(result, 2.2f);
				}
				return result;
			}
			half2 DoDistort(half mask, sampler2D distTex, half2 distUV, half distPower)
			{
				half4 distColor =  tex2D(distTex, distUV);
				half2 offsetUV = mask*UnpackNormal(distColor)*distPower;
				return offsetUV;
			}
			half2 DoMove(half2 uv, half4 param){
				return uv+frac(param.xy*half2(_Time.y, _Time.y));
			}
			half2 DoRotate(half2 uv, half4 param){
				float cosx = cos(param.z+param.w*_Time.y);
				float sinx = sin(param.z+param.w*_Time.y);
				float2x2 m2 = float2x2(cosx, sinx, -sinx, cosx);
				//
				return mul(m2, uv.xy-param.xy)+param.xy;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				//do move or rotate uv
				#if _MOTION1_ON
					o.uv.zw = TRANSFORM_TEX(v.uv, _MotionTex1);
					#if _MOTION1_ROTATE_ON
						o.uv.zw = DoRotate(o.uv.zw, _RotateParam1);
					#endif
					#if _MOTION1_MOVE_ON
						o.uv.zw = DoMove(o.uv.zw, _MoveParam1);
					#endif
				#else
					o.uv.zw = half2(0,0);
				#endif
				//init
				#if _MOTION2_ON || _MOTION3_ON
					o.uv2.xyzw = half4(0,0,0,0);
				#endif

				#if _MOTION2_ON
					o.uv2.xy = TRANSFORM_TEX(v.uv, _MotionTex2);
					#if _MOTION2_ROTATE_ON
						o.uv2.xy = DoRotate(o.uv2.xy, _RotateParam2);
					#endif
					#if _MOTION2_MOVE_ON
						o.uv2.xy = DoMove(o.uv2.xy, _MoveParam2);
					#endif
				#endif
				#if _MOTION3_ON
					o.uv2.zw = TRANSFORM_TEX(v.uv, _MotionTex3);
					#if _MOTION3_ROTATE_ON
						o.uv2.zw = DoRotate(o.uv2.zw, _RotateParam3);
					#endif
					#if _MOTION3_MOVE_ON
						o.uv2.zw = DoMove(o.uv2.zw, _MoveParam3);
					#endif
				#endif
				#if _MOTION4_ON
					o.uv3.xy = TRANSFORM_TEX(v.uv, _MotionTex4);
					#if _MOTION4_ROTATE_ON
						o.uv3.xy = DoRotate(o.uv3.xy, _RotateParam4);
					#endif
					#if _MOTION4_MOVE_ON
						o.uv3.xy = DoMove(o.uv3.xy, _MoveParam4);
					#endif
				#endif
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 mask = MyTex2D_4(_MaskTex, i.uv.xy);
				half2 uv = i.uv.xy;
				half2 uv1 = i.uv.zw;
				#if _MOTION2_ON || _MOTION3_ON
				half2 uv2 = i.uv2.xy;
				half2 uv3 = i.uv2.zw;
				#endif
				#if _MOTION4_ON
				half2 uv4 = i.uv3.xy;
				#endif

				//do distort uv
				#if _MOTION4_ON && _MOTION3_ON && _MOTION4_BLEND_DISTORT_ON
					uv3 += DoDistort(mask.a, _MotionTex4, uv4, _DistortParam4);
				#endif
				#if _MOTION3_ON && _MOTION2_ON && _MOTION3_BLEND_DISTORT_ON
					uv2 += DoDistort(mask.b, _MotionTex3, uv3, _DistortParam3);
				#endif
				#if _MOTION2_ON && _MOTION1_ON && _MOTION2_BLEND_DISTORT_ON
					uv1 += DoDistort(mask.g, _MotionTex2, uv2, _DistortParam2);
				#endif
				#if _MOTION1_ON && _MOTION1_BLEND_DISTORT_ON
					uv += DoDistort(mask.r, _MotionTex1, uv1, _DistortParam1);
				#endif

				if(IsGammaSpace()){
					_Color = pow(_Color, 2.2);
				}

				half4 result = MyTex2D_4(_MainTex, uv)*_Color;

				//if not distort, blend src and dest
				#if _MOTION1_ON
					if(IsGammaSpace()){
						_MotionColor1 = pow(_MotionColor1, 2.2);
					}
					half3 mulColor1 = _MotionColor1.xyz*(_ColorParam1.x+_ColorParam1.y*max(0,sin(_ColorParam1.w+_Time.y*_ColorParam1.z)));
					half4 motion1 = MyTex2D_4(_MotionTex1, uv1);

					#if _MOTION1_BLEND_ADD_ON
						result.xyz += motion1.xyz*mask.r*mulColor1;
					#elif _MOTION1_BLEND_MINUS_ON
						result.xyz -= motion1.xyz*mask.r*mulColor1;
					#elif _MOTION1_BLEND_MULTIPLY_ON
						result.xyz = lerp(result.xyz, result.xyz*motion1.xyz*mulColor1, mask.r*motion1.a);
					#endif
				#endif

				#if _MOTION2_ON && !_MOTION2_DISTORT_ON
					if(IsGammaSpace()){
						_MotionColor2 = pow(_MotionColor2, 2.2);
					}
					half3 mulColor2 = _MotionColor2.xyz*(_ColorParam2.x+_ColorParam2.y*max(0,sin(_ColorParam2.w+_Time.y*_ColorParam2.z)));
					half4 motion2 = MyTex2D_4(_MotionTex2, uv2);

					#if _MOTION2_BLEND_ADD_ON
						result.xyz += motion2.xyz*mask.g*mulColor2;
					#elif _MOTION2_BLEND_MINUS_ON
						result.xyz -= motion2.xyz*mask.g*mulColor2;
					#elif _MOTION2_BLEND_MULTIPLY_ON
						result.xyz = lerp(result.xyz, result.xyz*motion2.xyz*mulColor2, mask.r*motion2.a);
					#endif
				#endif

				#if _MOTION3_ON && !_MOTION3_DISTORT_ON
					if(IsGammaSpace()){
						_MotionColor3 = pow(_MotionColor3, 2.2);
					}
					half3 mulColor3 = _MotionColor3.xyz*(_ColorParam3.x+_ColorParam3.y*max(0,sin(_ColorParam3.w+_Time.y*_ColorParam3.z)));
					half4 motion3 = MyTex2D_4(_MotionTex3, uv3);
					#if _MOTION3_BLEND_ADD_ON
						result.xyz += motion3.xyz*mask.b*mulColor3;
					#elif _MOTION3_BLEND_MINUS_ON
						result.xyz -= motion3.xyz*mask.b*mulColor3;
					#elif _MOTION3_BLEND_MULTIPLY_ON
						result.xyz = lerp(result.xyz, result.xyz*motion3.xyz*mulColor3, mask.r*motion3.a);
					#endif
				#endif

				#if _MOTION4_ON && !_MOTION4_DISTORT_ON
					if(IsGammaSpace()){
						_MotionColor4 = pow(_MotionColor4, 2.2);
					}
					half3 mulColor4 = _MotionColor4.xyz*(_ColorParam4.x+_ColorParam4.y*max(0,sin(_ColorParam4.w+_Time.y*_ColorParam4.z)));
					half4 motion4 = MyTex2D_4(_MotionTex4, uv4);
					#if _MOTION4_BLEND_ADD_ON
						result.xyz += motion4.xyz*mask.a*mulColor4;
					#elif _MOTION4_BLEND_MINUS_ON
						result.xyz -= motion4.xyz*mask.a*mulColor4;
					#elif _MOTION4_BLEND_MULTIPLY_ON
						result.xyz = lerp(result.xyz, result.xyz*motion4.xyz*mulColor4, mask.r*motion4.a);
					#endif
				#endif

				if(IsGammaSpace())
				{
					result.xyz = pow(result.xyz, 1/2.2f);
				}

				return result;
			}

			ENDCG
		}
	}
	CustomEditor "CardEffectShaderGUI"
}
