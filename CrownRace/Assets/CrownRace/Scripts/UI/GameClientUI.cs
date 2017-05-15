using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameClientUI : MonoBehaviour {
	[SerializeField]
	private InputField serverIPInput;
	[SerializeField]
	private InputField serverPortInput;
	[SerializeField]
	private Button connectBtn;
	[SerializeField]
	private Button disconnectBtn;
	[SerializeField]
	private Text notifyText;

	public void SetNotifyText(string msg){
		notifyText.text = msg;
	}

	public void SetInputReadOnly(bool readOnly){
		serverIPInput.readOnly = readOnly;
		serverPortInput.readOnly = readOnly;
	}

	public bool ConnectBtnInteractable{
		get{ return connectBtn.interactable;}
		set{ connectBtn.interactable = value;}
	}

	public bool DisConnectBtnInteractable{
		get{ return disconnectBtn.interactable;}
		set{ disconnectBtn.interactable = value;}
	}

	public void OnBackBtnClick()
	{
		GameStateManager.Instance ().FSM.CurrentState.Message ("BackToPrevious", null);
		AudioManager.Instance.PlayAudio ("click_btn", false);
	}
	public void OnConnectBtnClick()
	{
		object[] p = new object[2];
		p [0] = (object)serverIPInput.text;
		p [1] = (object)serverPortInput.text;
		GameStateManager.Instance ().FSM.CurrentState.Message ("ConnectServer", p);
		AudioManager.Instance.PlayAudio ("click_btn", false);
	}
	public void OnDisconnectBtnClick()
	{
		AudioManager.Instance.PlayAudio ("click_btn", false);
		GameStateManager.Instance ().FSM.CurrentState.Message ("DisconnectServer", null);
	}
}
