using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardPositionCtr : MonoBehaviour {
	public Transform m_left;
	public Transform m_right;
	public Transform m_cardSpacePos;
	public Transform m_cardNearPos;
	private List<GameObject> m_list = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A)){
			StartCoroutine(WaitSecondsAddCard(1));
		}
	}
	IEnumerator WaitSecondsAddCard(float second){
		yield return new WaitForSeconds(second);
		GameObject prefab = Resources.Load("Cards/card")as GameObject;
		GameObject go = GameObject.Instantiate(prefab)as GameObject;
		AddCard(go);
	}
	void CardMoveToNear(GameObject go){
		Vector3 dst = Vector3.zero;
		float t=0;
		if(m_list.Count > 0)
			t=1;
		
		dst = CMath.Lerp (m_left.position, m_right.position, t);
		MoveTo(go, dst, 1);
		RotateTo(go, Quaternion.Lerp(m_left.rotation, m_right.rotation, t).eulerAngles, 1);

		for(int i=0; i<m_list.Count; i++)
		{
			t = i/(float)(m_list.Count);
			MoveTo(m_list[i], CMath.Lerp(m_left.position, m_right.position, t), 1);
			Quaternion q = Quaternion.Lerp(m_left.rotation, m_right.rotation, t);
			RotateTo(m_list[i], q.eulerAngles, 1);
		}

		m_list.Add(go);
	}
	public void AddCard(GameObject go){
		go.transform.parent = this.transform;
		go.transform.position = m_cardSpacePos.position;
		go.transform.rotation = m_cardSpacePos.rotation;

		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", 1);
		args.Add("loopType", "none");
		args.Add("delay", 0);
		args.Add("position", m_cardNearPos.position);
		args.Add("oncomplete", "CardMoveToNear");
		args.Add("oncompleteparams", go);
		args.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(go, args);
	}

	void MoveTo(GameObject go, Vector3 dst, float time){
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", time);
		args.Add("loopType", "none");
		args.Add("position", dst);
		iTween.MoveTo(go, args);
	}
	void RotateTo(GameObject go, Vector3 dst, float time){
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", time);
		args.Add("loopType", "none");
		args.Add("rotation", dst);
		iTween.RotateTo(go, args);
	}
}
