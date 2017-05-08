using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowAboutCardCtr : MonoBehaviour {

	public List<Transform> m_posTrans;
	private List<CardEffectCtr> m_cardList = new List<CardEffectCtr>();
	private GameObject m_itemPrefab;
	private int m_curIndex = 0;

	public bool ShowNext(){
		if(m_curIndex >= GameGlobalData.DeveloperList.Length)
			return false;
		foreach(CardEffectCtr ui in m_cardList){
			GameObject.Destroy(ui.gameObject);
		}
		m_cardList.Clear();
		for(int i=0; i<3; i++){
			if(m_curIndex+i < GameGlobalData.DeveloperList.Length){
				m_cardList.Add(LoadCardItem(m_curIndex+i, m_posTrans[i]));
			}
		}
		m_curIndex += 3;
		return true;
	}
	public bool ShowPre(){
		if(m_curIndex - 3 <= 0)
			return false;
		foreach(CardEffectCtr ui in m_cardList){
			GameObject.Destroy(ui.gameObject);
		}
		m_cardList.Clear();
		m_curIndex -= 6;
		m_curIndex = m_curIndex < 0 ? 0 : m_curIndex;
		for(int i=0; i<3; i++){
			if(m_curIndex+i < GameGlobalData.DeveloperList.Length){
				m_cardList.Add(LoadCardItem(m_curIndex+i, m_posTrans[i]));
			}
		}
		m_curIndex += 3;
		return true;
	}
	CardEffectCtr LoadCardItem(int config_id, Transform parent){
		GameObject prefab = Resources.Load("Cards/developer_"+config_id)as GameObject;
		GameObject go = GameObject.Instantiate(prefab);
		Vector3 scale = go.transform.localScale;
		go.transform.parent = parent;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = scale;
		CardEffectCtr ctr = go.GetComponent<CardEffectCtr>();
		ctr.SetDeveloperCardInfo(0, config_id);
		return ctr;
	}
}
