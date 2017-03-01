using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardPositionCtr : MonoBehaviour {
	public Transform m_left;
	public Transform m_right;
	public Transform m_cardSpacePos;
	public Transform m_cardNearPos;
	public Transform m_cardShowPos;
	private List<GameObject> m_list = new List<GameObject>();
	private GameObject m_curSelect;
	private Vector3 m_prePos;
	private Quaternion m_preRotate;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A)){
			StartCoroutine(WaitSecondsAddCard(1));
		}
		if (Input.GetMouseButtonDown (0)) {
			GameObject go = TryHitCard ();
			if (go != null) {
				if (m_curSelect == null) {
					SelectCard (go);
				} else {
					if (m_curSelect.Equals (go)) {
				
					} else {
						BackCurSelectPos ();
						SelectCard (go);
					}
				}
			} else {
				BackCurSelectPos ();
			}
		}
	}
	void SelectCard(GameObject go){
		m_curSelect = go;
		m_prePos = m_curSelect.transform.localPosition;
		m_preRotate = m_curSelect.transform.localRotation;
		//
		MoveTo(m_curSelect, m_cardShowPos.localPosition, 0.1f);
		RotateTo (m_curSelect, m_cardShowPos.localRotation.eulerAngles, 0.1f);
	}
	void BackCurSelectPos()
	{
		if (m_curSelect != null) {
			MoveTo(m_curSelect, m_prePos, 0.1f);
			RotateTo(m_curSelect, m_preRotate.eulerAngles, 0.1f);
		
			m_curSelect = null;
		}
	}
	GameObject TryHitCard(){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 50, 1<<LayerMask.NameToLayer("Card"))) {
			Debug.Log ("hit :"+hit.collider.gameObject.name);
			return hit.collider.gameObject;
		}
		return null;
	}

	IEnumerator WaitSecondsAddCard(float second){
		yield return new WaitForSeconds(second);
		GameObject prefab = Resources.Load("Cards/card")as GameObject;
		GameObject go = GameObject.Instantiate(prefab)as GameObject;
		AddCard(go);
	}
	void CardMoveToNear(GameObject go){
		Vector3 dst = Vector3.zero;
		float t=0.5f;
		if(m_list.Count > 0)
			t=(m_list.Count+1)/(float)(m_list.Count+2);
		
		dst = CMath.Lerp (m_left.localPosition, m_right.localPosition, t);
		MoveTo(go, dst, 1);
		RotateTo(go, Quaternion.Lerp(m_left.localRotation, m_right.localRotation, t).eulerAngles, 1);

		for(int i=0; i<m_list.Count; i++)
		{
			t = (i+1)/(float)(m_list.Count+2);
			Vector3 pos = CMath.Lerp (m_left.localPosition, m_right.localPosition, t);
			pos = pos - go.transform.forward*Mathf.Sin(Mathf.PI*i/(float)(m_list.Count))*0.1f;
			MoveTo(m_list[i], pos, 1);
			Quaternion q = Quaternion.Lerp(m_left.localRotation, m_right.localRotation, t);
			RotateTo(m_list[i], q.eulerAngles, 1);
		}

		m_list.Add(go);
	}
	void CardMoveOver(GameObject go){
		go.GetComponentInChildren<BoxCollider> ().enabled = true;
	}
	public void AddCard(GameObject go){
		if (go == null)
			return;
		BackCurSelectPos ();

		go.GetComponentInChildren<BoxCollider> ().enabled = false;
		go.transform.parent = this.transform;
		go.transform.position = m_cardSpacePos.position;
		go.transform.rotation = m_cardSpacePos.rotation;

		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", 1);
		args.Add("loopType", "none");
		args.Add("delay", 0);
		args.Add("islocal", true);
		args.Add("position", m_cardNearPos.localPosition);
		args.Add("oncomplete", "CardMoveToNear");
		args.Add("oncompleteparams", go);
		args.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(go, args);
		//
		Hashtable args2 = new Hashtable();
		args2.Add("easeType", iTween.EaseType.linear);
		args2.Add("time", 1);
		args2.Add("loopType", "none");
		args2.Add("delay", 0);
		args2.Add("islocal", true);
		args2.Add("rotation", m_cardNearPos.localRotation.eulerAngles);

		iTween.RotateTo(go, args2);
	}

	void MoveTo(GameObject go, Vector3 dst, float time){
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", time);
		args.Add("loopType", "none");
		args.Add("position", dst);
		args.Add("oncomplete", "CardMoveOver");
		args.Add("oncompleteparams", go);
		args.Add("oncompletetarget", this.gameObject);
		args.Add("islocal", true);
		iTween.MoveTo(go, args);
	}
	void RotateTo(GameObject go, Vector3 dst, float time){
		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", time);
		args.Add("loopType", "none");
		args.Add("rotation", dst);
		args.Add("islocal", true);
		iTween.RotateTo(go, args);
	}
}
