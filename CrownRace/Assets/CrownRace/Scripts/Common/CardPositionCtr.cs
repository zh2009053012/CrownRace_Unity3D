using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardPositionCtr : MonoBehaviour {
	public Transform m_left;
	public Transform m_right;
	public Transform m_cardSpacePos;
	public Transform m_cardBackSpacePos;
	public Transform m_cardNearPos;
	public Transform m_cardShowPos;
	private List<CardEffectCtr> m_list = new List<CardEffectCtr>();
	private static CardEffectCtr m_curSelect;
	public static CardEffectCtr CurSelect{
		get{return m_curSelect;}
	}
	private Vector3 m_prePos;
	private Vector3 m_preScale;
	private Quaternion m_preRotate;
	private bool m_isReadyUse = false;
	public bool IsReadyUse{
		get{return m_isReadyUse;}
	}
	public static bool IsSelectCard{
		get{return m_curSelect!=null;}
	}
	// Use this for initialization
	void Start () {
		
	}
	public void DestroyCurSelectCard(){
		if(null != m_curSelect){
			m_list.Remove(m_curSelect);
			GameObject.Destroy(m_curSelect.gameObject);
			m_curSelect = null;
			ResortCardPos();
		}
	}
	// Update is called once per frame
	public void MyUpdate (bool canUseCard) {
//		if(Input.GetKeyDown(KeyCode.A)){
//			AddCard(Random.Range(0, GameGlobalData.CardList.Length), null);
//		}
		if (Input.GetMouseButtonDown (0)) {
			GameObject go = TryHitCard ();
			if (go != null) {
				if (m_curSelect == null) {
					SelectCard (go);
				} else {
					if (m_curSelect.gameObject.Equals (go)) {
						m_isReadyUse = true;
					} else {
						BackCurSelectPos ();
						SelectCard (go);
					}
				}
			} else {
				BackCurSelectPos ();
			}
		}else if(Input.GetMouseButtonUp(0)){
			if(m_isReadyUse){

				m_isReadyUse = false;
				BackCurSelectPos();
			}
		}
		if(canUseCard && m_isReadyUse){
			if(m_curSelect != null)
				FollowCursor(m_curSelect.gameObject);
		}
	}
	void FollowCursor(GameObject go){
		float x = Input.mousePosition.x;
		float y = Input.mousePosition.y;
		Vector3 screenPos = new Vector3(x, y, 3);
		go.transform.position = Camera.main.ScreenToWorldPoint(screenPos);
		go.transform.localScale = m_cardScale*0.25f;
	}
	void SelectCard(GameObject go){
		m_curSelect = go.GetComponent<CardEffectCtr>();
		m_prePos = go.transform.localPosition;
		m_preRotate = go.transform.localRotation;
		m_preScale = go.transform.localScale;
		//
		MoveTo(go, m_cardShowPos.localPosition, 0.1f);
		RotateTo (go, m_cardShowPos.localRotation.eulerAngles, 0.1f);
	}
	void BackCurSelectPos()
	{
		if (m_curSelect != null) {
			m_curSelect.transform.localScale = m_preScale;
			MoveTo(m_curSelect.gameObject, m_prePos, 0.1f);
			RotateTo(m_curSelect.gameObject, m_preRotate.eulerAngles, 0.1f);
		
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
	void ResortCardPos(){
		float t;
		for(int i=0; i<m_list.Count; i++)
		{
			t = (i+2)/(float)(m_list.Count+2);
			Vector3 pos = CMath.Lerp (m_left.localPosition, m_right.localPosition, t);
			pos = pos - m_list[i].transform.forward*Mathf.Sin(Mathf.PI*i/(float)(m_list.Count))*0.1f;
			MoveTo(m_list[i].gameObject, pos, 1);
			Quaternion q = Quaternion.Lerp(m_left.localRotation, m_right.localRotation, t);
			RotateTo(m_list[i].gameObject, q.eulerAngles, 1);
		}
	}
	void CardMoveToNear(CardEffectCtr ctr){
		Vector3 dst = Vector3.zero;
		float t=0.5f;
		if(m_list.Count > 0)
			t=(m_list.Count+2)/(float)(m_list.Count+3);
		
		dst = CMath.Lerp (m_left.localPosition, m_right.localPosition, t);
		MoveTo(ctr.gameObject, dst, 1);
		RotateTo(ctr.gameObject, Quaternion.Lerp(m_left.localRotation, m_right.localRotation, t).eulerAngles, 1);

		for(int i=0; i<m_list.Count; i++)
		{
			t = (i+2)/(float)(m_list.Count+3);
			Vector3 pos = CMath.Lerp (m_left.localPosition, m_right.localPosition, t);
			pos = pos - m_list[i].transform.forward*Mathf.Sin(Mathf.PI*i/(float)(m_list.Count))*0.1f;
			MoveTo(m_list[i].gameObject, pos, 1);
			Quaternion q = Quaternion.Lerp(m_left.localRotation, m_right.localRotation, t);
			RotateTo(m_list[i].gameObject, q.eulerAngles, 1);
		}

		m_list.Add(ctr);
	}
	private VoidEvent m_cardMoveOverEvent = null;
	private Vector3 m_cardScale = Vector3.one;
	void CardMoveOver(GameObject go){
		go.GetComponentInChildren<Collider> ().enabled = true;
		if(null != m_cardMoveOverEvent){
			m_cardMoveOverEvent.Invoke();
			m_cardMoveOverEvent = null;
		}
	}
	private VoidEvent m_backCardMoveOverEvent=null;
	public void AddCardTo(int instance_id, int config_id, Vector3 screenPos, VoidEvent callback){
		m_backCardMoveOverEvent = callback;

		GameObject prefab = Resources.Load("Cards/card_"+config_id)as GameObject;
		GameObject go = GameObject.Instantiate(prefab)as GameObject;

		go.GetComponentInChildren<Collider> ().enabled = false;
		go.transform.parent = this.transform;
		go.transform.position = m_cardBackSpacePos.position;
		go.transform.rotation = m_cardBackSpacePos.rotation;
		m_cardScale = go.transform.localScale;
		Debug.Log("record scale:"+go.transform.localScale);

		Vector3 targetPos = Camera.main.ScreenToWorldPoint(screenPos);
		Debug.Log("AddCardTo:"+targetPos);

		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.easeOutExpo);
		args.Add("time", 3);
		args.Add("loopType", "none");
		args.Add("delay", 0);
		args.Add("position", targetPos);
		args.Add("oncomplete", "BackCardMoveOver");
		args.Add("oncompleteparams", go);
		args.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(go, args);

		Hashtable args2 = new Hashtable();
		args2.Add("easeType", iTween.EaseType.easeOutExpo);
		args2.Add("time", 3);
		args2.Add("loopType", "none");
		args2.Add("delay", 0);
		args2.Add("scale", go.transform.localScale*0.05f);
		
		iTween.ScaleTo(go, args2);
	}
	void BackCardMoveOver(GameObject go){
		Debug.Log("BackCardMoveOver");
		if(null != m_backCardMoveOverEvent){
			m_backCardMoveOverEvent.Invoke();
			m_backCardMoveOverEvent = null;
		}
		GameObject.Destroy(go);
	}
	public void AddCard(int instance_id, int config_id, VoidEvent callback){
		m_cardMoveOverEvent = callback;
		m_isReadyUse = false;
		BackCurSelectPos ();

		GameObject prefab = Resources.Load("Cards/card_"+config_id)as GameObject;
		GameObject go = GameObject.Instantiate(prefab)as GameObject;

		go.GetComponentInChildren<Collider> ().enabled = false;
		go.transform.parent = this.transform;
		go.transform.position = m_cardSpacePos.position;
		go.transform.rotation = m_cardSpacePos.rotation;
		m_cardScale = go.transform.localScale;
		CardEffectCtr ctr = go.GetComponent<CardEffectCtr> ();
		ctr.SetCardInfo (instance_id, config_id);

		Hashtable args = new Hashtable();
		args.Add("easeType", iTween.EaseType.linear);
		args.Add("time", 1);
		args.Add("loopType", "none");
		args.Add("delay", 1);
		args.Add("islocal", true);
		args.Add("position", m_cardNearPos.localPosition);
		args.Add("oncomplete", "CardMoveToNear");
		args.Add("oncompleteparams", ctr);
		args.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(go, args);
		//
		Hashtable args2 = new Hashtable();
		args2.Add("easeType", iTween.EaseType.linear);
		args2.Add("time", 1);
		args2.Add("loopType", "none");
		args2.Add("delay", 1);
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
