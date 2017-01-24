using UnityEngine;
using System.Collections;

public class ctr : MonoBehaviour {
	[SerializeField]
	private NavMeshAgent m_agent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (null != m_agent && Input.GetMouseButtonUp (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				m_agent.SetDestination (hit.point);
			}
		}
	}
}
