using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform FollowTarget;
	public Vector3 Offset = Vector3.zero;
	public float MoveSpeed = 3;
	public float RotateSpeed = 10;
	public Transform GlobalView;

	private Vector3 targetPos;
	private Vector3 selfPos;
	private Vector3 result;

	private Vector2 m_preMousePos;
	private bool m_isGlobalView = false;
	// Use this for initialization
	void Start () {
	
	}
	void Update()
	{
		if(!m_isGlobalView && Input.GetMouseButtonDown(0))
		{
			m_preMousePos = Input.mousePosition;
		}else if(!m_isGlobalView && Input.GetMouseButton(0))
		{
			float dirX = Input.mousePosition.x - m_preMousePos.x;
			float dirY = Input.mousePosition.y - m_preMousePos.y;
			m_preMousePos = Input.mousePosition;
			this.transform.RotateAround(FollowTarget.position, Vector3.up, 
				RotateSpeed*dirX*Time.deltaTime);
			if((dirY > 0 && transform.forward.y > -0.15f) ||
				(dirY < 0 && transform.forward.y < -0.85f))
			{}
			else
			{
				transform.RotateAround(FollowTarget.position, transform.right,
					-RotateSpeed*dirY*Time.deltaTime);
			}
			Offset = transform.position - FollowTarget.position;
		}else if(Input.GetKeyDown(KeyCode.P))
		{
			m_isGlobalView = !m_isGlobalView;
			if(m_isGlobalView)
			{
				transform.position = GlobalView.position;
				transform.rotation = GlobalView.rotation;
			}
		}

	}
	// Update is called once per frame
	void LateUpdate () {
		//Debug.Log ("camera move before:"+transform.position);
		if(!m_isGlobalView)
		{
			targetPos = FollowTarget.position + Offset;
			transform.position = Vector3.Lerp(transform.position, targetPos, MoveSpeed*Time.deltaTime);

			transform.LookAt(FollowTarget);
			//Debug.Log ("camera move after:"+transform.position);
		}
	}

}
