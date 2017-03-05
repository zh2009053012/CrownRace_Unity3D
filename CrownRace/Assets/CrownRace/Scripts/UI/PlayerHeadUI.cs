using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHeadUI : MonoBehaviour {
	[SerializeField]
	private Text m_cardNumText;
	[SerializeField]
	private Text m_nameText;
	[SerializeField]
	private Image m_headImage;
	[SerializeField]
	private Image m_diceImage;
	protected int m_id;
	public int ID{
		get{return m_id;}
	}

	public void SetInfo(int id, string playerName, int cardNum=0)
	{
		m_id = id;
		m_nameText.text = playerName;
		Sprite head = Resources.Load<Sprite>("Heads/"+playerName)as Sprite;
		m_headImage.sprite = head;
		SetCardNum(cardNum);
	}
	public void SetCardNum(int num){
		m_cardNumText.text = num.ToString();
	}
	public void ShowDiceImage(bool isShow){
		m_diceImage.enabled = isShow;
	}
	public void OnHeadClick(){
		Debug.Log("click "+m_id);
		object[] p = new object[1];
		p[0] = (object)m_id;
		GameStateManager.Instance ().FSM.CurrentState.Message ("HeadBarClick", p);
	}
}
