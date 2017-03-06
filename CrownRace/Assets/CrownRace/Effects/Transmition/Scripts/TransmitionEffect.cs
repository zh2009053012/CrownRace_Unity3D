using UnityEngine;
using System.Collections;

public class TransmitionEffect : MonoBehaviour {
	public Material m_skybox;
	public float m_rotateSpeed = 0.5f;
	public Material m_spaceMat;
	private ParticleSystem m_ps;
	private Camera m_camera;
	private RenderTexture m_rt;

	// Use this for initialization
	void Start () {
		m_ps = GetComponent<ParticleSystem> ();
		m_camera = GetComponentInChildren<Camera> ();
		CopyCamera ();

		m_rt = new RenderTexture (m_camera.pixelWidth, m_camera.pixelHeight, 24);
		m_camera.targetTexture = m_rt;
		m_spaceMat.SetTexture ("_SpaceTex", m_rt);
	}
	void CopyCamera()
	{
		m_camera.transform.position = Camera.main.transform.position;
		m_camera.transform.rotation = Camera.main.transform.rotation;
		m_camera.aspect = Camera.main.aspect;
		m_camera.farClipPlane = Camera.main.farClipPlane;
		m_camera.nearClipPlane = Camera.main.nearClipPlane;
		m_camera.orthographic = Camera.main.orthographic;
		m_camera.fieldOfView = Camera.main.fieldOfView;
	}
	IEnumerator StopParticleSystem(float second){
		yield return new WaitForSeconds (second);
		Debug.Log("stop");
		m_ps.Stop ();
		m_ps.Clear ();
	}

	public void Play(){
		if (!m_ps.isPlaying) {
			m_ps.Play ();
			StartCoroutine (StopParticleSystem(m_ps.duration));
	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_ps.isPlaying) {
			//m_camera.transform.RotateAround (new Vector3 (0, 1, 0), Time.deltaTime*m_rotateSpeed);
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			Play ();
		}
	}
}
