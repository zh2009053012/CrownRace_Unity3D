#ifndef WATER_TOOLS
#define WATER_TOOLS

void SimpleWaterScattering(half viewDist, half3 worldPos, half depth, half3 diffuseRadiance,
					half3 attenuation_c, half3 kd, out half3 outScattering, out half3 inScattering)
{
	float t = depth / (_WorldSpaceCameraPos.y - worldPos.y);

	// Water scattering
	float d = viewDist*t;  // one way!
	outScattering = exp(-attenuation_c*d);
	inScattering = diffuseRadiance* (1 - outScattering*exp(-depth*kd));
}

float3 PerPixelNormal(sampler2D bumpMap, float4 coords, float bumpStrength) 
{
	float2 bump = (UnpackNormal(tex2D(bumpMap, coords.xy)) + UnpackNormal(tex2D(bumpMap, coords.zw))) * 0.5;
	bump += (UnpackNormal(tex2D(bumpMap, coords.xy*2))*0.5 + UnpackNormal(tex2D(bumpMap, coords.zw*2))*0.5) * 0.5;
	bump += (UnpackNormal(tex2D(bumpMap, coords.xy*8))*0.5 + UnpackNormal(tex2D(bumpMap, coords.zw*8))*0.5) * 0.5;
	float3 worldNormal = float3(0,0,0);
	worldNormal.xz = -bump.xy * bumpStrength;
	worldNormal.y = 1;
	return worldNormal;
}
// Fresnel approximation, power = 5
float FastFresnel(float3 I, float3 N, float R0) 
{
  float icosIN = saturate(1-dot(I, N));
  float i2 = icosIN*icosIN, i4 = i2*i2;
  return R0 + (1-R0)*(i4*icosIN);
}
#endif