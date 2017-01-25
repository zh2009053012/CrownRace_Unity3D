using UnityEngine;
using System.Collections;

//attach with water plane
public class WaterManager : MonoBehaviour {

	private float WaterAltitude=0;
	public Vector4 WaterScatteringKd = new Vector4(.3867f, .1055f, .0469f, 0f);
	public Vector4 WaterScatteringAttenuation = new Vector4( .45f, .1718f, .1133f, 0);
	public Vector4 WaterScatteringDiffuseRadiance = new Vector4( .0338f, .1015f, .2109f, 0);
	// Use this for initialization
	void Start () {
		WaterAltitude = this.transform.position.y;
		SetShaderGlobalParams();
	}
	void SetShaderGlobalParams()
	{
		Shader.SetGlobalFloat("_WaterAltitude", WaterAltitude);
		Shader.SetGlobalVector("_WaterScatteringKd", WaterScatteringKd);
		Shader.SetGlobalVector("_WaterScatteringAttenuation", WaterScatteringAttenuation);
		Shader.SetGlobalVector("_WaterScatteringDiffuseRadiance", WaterScatteringDiffuseRadiance);
	}
	
	// Update is called once per frame
	void Update () {
		//SetShaderGlobalParams();

	}
}
