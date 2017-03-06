using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardEffectCtr : MonoBehaviour {
	protected int m_card_id;
	public int CardID{
		get{return m_card_id;}
		set{ m_card_id = value;}
	}
	[SerializeField]
	protected Text m_cardNameText;
	[SerializeField]
	protected Text m_cardDescText;

	public void SetCardInfo(int id){
		m_card_id = id;
		m_cardNameText.text = GameGlobalData.CardList[id].name;
		m_cardDescText.text = GameGlobalData.CardList[id].desc;
	}
}
