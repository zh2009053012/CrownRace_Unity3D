using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardEffectCtr : MonoBehaviour {
	protected int m_card_instance_id;
	public int CardInstanceID{
		get{return m_card_instance_id;}
		set{ m_card_instance_id = value;}
	}
	protected int m_card_config_id;
	public int CardConfigID{
		get{return m_card_config_id;}
		set{m_card_config_id = value;}
	}
	[SerializeField]
	protected Text m_cardNameText;
	[SerializeField]
	protected Text m_cardDescText;

	public void SetCardInfo(int instance_id, int config_id){
		m_card_instance_id = instance_id;
		m_card_config_id = config_id;
		m_cardNameText.text = GameGlobalData.CardList[config_id].name;
		m_cardDescText.text = GameGlobalData.CardList[config_id].desc;
	}
}
