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
	[SerializeField]
	private Image m_backImage;
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
	public void ShowHighlight(bool isHighlight){
		if(isHighlight)
		{
			m_backImage.color = Color.yellow;
		}else{
			m_backImage.color = Color.white;
		}
	}
	public void OnHeadClick(){
		Debug.Log("click "+m_id);
		object[] p = new object[1];
		p[0] = (object)m_id;
		GameStateManager.Instance ().FSM.CurrentState.Message ("HeadBarClick", p);
	}
	public void OnPointerEnter(){
		object[] p = new object[1];
		p[0] = (object)m_id;
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnPointerEnter", p);
	}
	public void OnPointerExit(){
		object[] p = new object[1];
		p[0] = (object)m_id;
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnPointerExit", p);
	}
	public void OnSelect(){

		Debug.Log("select");
		object[] p = new object[1];
		p[0] = (object)m_id;
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnSelectHeadBar", p);
		//

	}
}
