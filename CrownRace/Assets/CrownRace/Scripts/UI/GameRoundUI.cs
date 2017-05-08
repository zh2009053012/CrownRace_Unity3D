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
	private GameObject m_settingPanel;
	[SerializeField]
	private Button m_fullScreenBtn;
	[SerializeField]
	private Button m_noFullScreenBtn;
	[SerializeField]
	private RectTransform m_rootRectTransform;
	public Button EndRoundBtn;
	public RectTransform RootRectTransform{
		get{return m_rootRectTransform;}
	}
	// Use this for initialization
	public void Init (string head) {
		m_head.sprite = Resources.Load ("Heads/" + head+"_Alpha", typeof(Sprite))as Sprite;
		UpdateFullScreenBtn ();
		HideSettingPanel ();
		HideCrown ();
	}
	public void ShowCrown(){
		m_crow.enabled = true;
	}
	public void HideCrown(){
		m_crow.enabled = false;
	}
	public void ShowSettingPanel(){
		m_settingPanel.SetActive (true);
		UpdateFullScreenBtn ();
	}
	public void HideSettingPanel(){
		m_settingPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 60 == 0) {
			UpdateFullScreenBtn ();
		}
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
	public void OnEndRoundBtnClick(){
		GameStateManager.Instance ().FSM.CurrentState.Message ("ClickEndRoundBtn", null);
	}
	public void OnPauseBtnClick(){
		
	}
	public void OnCardBtnClick(){
		GameStateManager.Instance ().FSM.CurrentState.Message ("ClickCardBtn", null);
	}
	public void OnQuitBtnClick(){
		GameStateManager.Instance ().FSM.CurrentState.Message ("ClickQuitBtn", null);
	}
	public void OnSettingBtnClick(){
		//GameStateManager.Instance ().FSM.CurrentState.Message ("ClickSettingBtn", null);
		ShowSettingPanel();
	}
	public void OnCloseSettingBtnClick(){
		HideSettingPanel ();
	}
	public void OnChangeMusicVolume(){
		
	}
	public void OnFullScreenBtnClick(){
		Screen.fullScreen = true;
		UpdateFullScreenBtn ();
	}
	public void OnNoFullScreenBtnClick(){
		Screen.fullScreen = false;
		UpdateFullScreenBtn ();
	}
	void UpdateFullScreenBtn(){
		m_fullScreenBtn.gameObject.SetActive(!Screen.fullScreen);
		m_noFullScreenBtn.gameObject.SetActive(Screen.fullScreen);
	}
}
