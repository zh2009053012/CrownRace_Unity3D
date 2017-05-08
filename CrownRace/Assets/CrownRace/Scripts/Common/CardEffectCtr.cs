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
	public Text CardNameText{
		get{ 
			if (null == m_cardNameText) {
				m_cardNameText = GameObject.Find ("Canvas/NameText").GetComponent<Text>();
			}
			return m_cardNameText;
		}
	}
	[SerializeField]
	protected Text m_cardDescText;
	public Text CardDescText{
		get{ 
			if (null == m_cardDescText) {
				m_cardDescText = GameObject.Find ("Canvas/DescText").GetComponent<Text>();
			}
			return m_cardDescText;
		}
	}

	public void SetCardInfo(int instance_id, int config_id){
		m_card_instance_id = instance_id;
		m_card_config_id = config_id;
		CardNameText.text = GameGlobalData.CardList[config_id].name;
		CardDescText.text = GameGlobalData.CardList[config_id].desc;
		Debug.Log ("CardEffectCtr::SetCardInfo:"+config_id+","+GameGlobalData.CardList[config_id].name+","+GameGlobalData.CardList[config_id].desc);
	}
	public void SetDeveloperCardInfo(int instance_id, int config_id){
		m_card_instance_id = instance_id;
		m_card_config_id = config_id;
		CardNameText.text = GameGlobalData.DeveloperList[config_id].name;
		CardDescText.text = GameGlobalData.DeveloperList[config_id].desc;
		Debug.Log ("CardEffectCtr::SetCardInfo:"+config_id+","+GameGlobalData.DeveloperList[config_id].name+","+GameGlobalData.DeveloperList[config_id].desc);
	}
}
