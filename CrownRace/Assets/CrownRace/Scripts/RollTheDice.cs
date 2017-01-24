using UnityEngine;
using System.Collections;

public delegate void RollOverEvent(uint result);
public class RollTheDice : MonoBehaviour {

	[SerializeField]
	private Rigidbody m_rigidbody;
	private RollOverEvent m_rollOverCallback;
	private bool m_isRollOver = true;
	public bool RegisterRollOverNotify(RollOverEvent callback)
	{
		if (null != callback) {
			m_rollOverCallback += callback;
			return true;
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		if (null == m_rigidbody) {
			m_rigidbody = GetComponent<Rigidbody> ();
		}
		Random.seed = GetTimeStamp ();
	}

	int GetTimeStamp()
	{
		System.TimeSpan ts = System.DateTime.UtcNow - new System.DateTime (1970, 1, 1, 0, 0, 0, 0);
		return (int) ts.TotalSeconds;
	}

	public void Roll(float force)
	{
		if (null != m_rigidbody) {
			m_isRollOver = false;
			m_rigidbody.AddForce (RandomVector3()*m_rigidbody.mass*force);
			m_rigidbody.AddTorque (RandomVector3 () * 5000);

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.R) && null != m_rigidbody ) {
			Roll(Random.Range(10000, 20000));
		}
		if (m_rigidbody.velocity.sqrMagnitude < 0.01f) {
			
			if (null != m_rollOverCallback) {
				m_rollOverCallback.Invoke (GetNumber());
			}
			m_isRollOver = true;
		}
	}
	uint GetNumber()
	{
		if (Vector3.Dot (transform.up, Vector3.up) > 0.9f) {
			return 5;
		}
		if (Vector3.Dot (-transform.up, Vector3.up) > 0.9f) {
			return 2;
		}
		if (Vector3.Dot (transform.right, Vector3.up) > 0.9f) {
			return 4;
		}
		if (Vector3.Dot (-transform.right, Vector3.up) > 0.9f) {
			return 3;
		}
		if (Vector3.Dot (transform.forward, Vector3.up) > 0.9f) {
			return 1;
		}
		if (Vector3.Dot (-transform.forward, Vector3.up) > 0.9f) {
			return 6;
		}
		return 0;
	}

	Vector3 RandomVector3()
	{
		Vector3 dir = new Vector3 (Random.Range(0.0f, 1.0f), 1, Random.Range(0.0f, 1.0f));
		return dir.normalized;
	}

}
