using UnityEngine;
using System.Collections;

public class Refraction : MonoBehaviour {

	public Camera RefCamera;
	public Material RefMat;
	public Transform Panel;
	public float clipPlaneOffset=0;
	private Shader m_scatteringShader;
	private RenderTexture m_waterScatteringRT, refTexture;
	// Use this for initialization
	void Start () {
		if(null == RefCamera)
		{
			GameObject go = new GameObject();
			go.name = "refrCamera";
			RefCamera = go.AddComponent<Camera>();
			RefCamera.CopyFrom(Camera.main);
			RefCamera.fieldOfView *= 1.1f;
			RefCamera.enabled = false;
			RefCamera.cullingMask =  ~(1 << LayerMask.NameToLayer("Water"));
		}
		if(null == RefMat)
		{
			RefMat = this.GetComponent<Renderer>().sharedMaterial;
		}
		refTexture = new RenderTexture(Mathf.FloorToInt(RefCamera.pixelWidth),
		                                             Mathf.FloorToInt(RefCamera.pixelHeight), 24);
		refTexture.hideFlags = HideFlags.DontSave;
		RefCamera.targetTexture = refTexture;
		m_waterScatteringRT = new RenderTexture(Mathf.FloorToInt(RefCamera.pixelWidth), 
		                                        Mathf.FloorToInt(RefCamera.pixelHeight), 24);
		if(null == Panel)
		{
			Panel = transform;
		}
		m_scatteringShader = Shader.Find("Unlit/WaterScattering");
		if(!m_scatteringShader.isSupported)
		{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}
	public void OnWillRenderObject()
	{
		RefCamera.transform.position = Camera.main.transform.position;
		RefCamera.transform.rotation = Camera.main.transform.rotation;

		Matrix4x4 projM = RefCamera.projectionMatrix;
		Vector4 clipPlane = CameraSpacePlane (RefCamera, Panel.position, Panel.up, -1);
		RefCamera.projectionMatrix = Reflection.CalculateObliqueMatrix(projM, clipPlane);
		RefCamera.targetTexture = refTexture;
		RefCamera.Render();

		//RefMat.SetTexture("_RefrTexture", RefCamera.targetTexture);
		RefMat.SetMatrix("_RefractCameraVP", GL.GetGPUProjectionMatrix(RefCamera.projectionMatrix, false)*RefCamera.worldToCameraMatrix);

		Shader.SetGlobalTexture ("_UnScatteringRefrTex", RefCamera.targetTexture);

		RefCamera.targetTexture = m_waterScatteringRT;
		RefCamera.RenderWithShader(m_scatteringShader, "RenderType");
		RefMat.SetTexture("_RefrTexture", RefCamera.targetTexture);
	}
}
