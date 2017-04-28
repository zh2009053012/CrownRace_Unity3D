using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameRoundUI : MonoBehaviour {
	public Button RollDiceBtn;
	public Transform HeadContent;
	[SerializeField]
	private Image m_head;
	[SerializeField]
	private Image m_crow;
	[SerializeField]
	private RectTransform m_rootRectTransform;
	public RectTransform RootRectTransform{
		get{return m_rootRectTransform;}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnRollDiceBtnClick()
	{
		GameStateManager.Instance ().FSM.CurrentState.Message ("RollDiceBtn", null);
	}
	public Vector2 RectTransformToScreenPos(Vector2 pos)
	{
		Vector2 p = pos + new Vector2(-m_rootRectTransform.rect.width, m_rootRectTransform.rect.height)*0.5f;
		p = new Vector2(p.x/m_rootRectTransform.rect.width, p.y/m_rootRectTransform.rect.height);
		p = new Vector2(p.x*Screen.width, p.y*Screen.height);
		return p;
	}
	public void OnEndGroundBtnClick(){
		
	}
	public void OnPauseBtnClick(){
	
	}
	public void OnCardBtnClick(){
	
	}
	public void OnQuitBtnClick(){
	
	}
	public void OnSettingBtnClick(){
	
	}
}
