using UnityEngine;
using System.Collections.Generic;

public class CardPositionCtr : MonoBehaviour {
	public Transform m_left;
	public Transform m_right;
	private List<Transform> m_list = new List<Transform>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void AddCard(GameObject go){
		Vector3 pos = Vector3.zero;
		pos = CMath.Lerp (m_left.position, m_right.position, m_list.Count + 1);

	}
}
