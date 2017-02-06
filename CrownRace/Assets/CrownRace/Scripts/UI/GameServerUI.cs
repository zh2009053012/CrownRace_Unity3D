using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameServerUI : MonoBehaviour {
	public GridLayoutGroup gridGroupCtr;
	[SerializeField]
	private InputField serverIPInput;
	[SerializeField]
	private InputField serverPortInput;
	[SerializeField]
	private Button startServerBtn;

	public string ServerIP{
		set{
			serverIPInput.text = value;
		}
		get{ 
			return serverIPInput.text;
		}
	}
	public string ServerPort{
		set{ 
			serverPortInput.text = value;
		}
		get{ 
			return serverPortInput.text;
		}
	}

	public void SetStartBtnEnable(bool isEnable){
		startServerBtn.interactable = isEnable;
	}

	public void SetReadOnlyIPInput(bool isReadOnly){
		serverIPInput.readOnly = isReadOnly;
		serverPortInput.readOnly = isReadOnly;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnStartServerBtnClick()
	{
		object[] p = new object[2];
		p [0] = (string)ServerIP;
		p [1] = (string)ServerPort;
		GameStateManager.Instance().FSM.CurrentState.Message("StartServer", p);
	}
	public void OnBackBtnClick()
	{
		GameStateManager.Instance().FSM.CurrentState.Message("BackToStart", null);
	}
	public void OnStartGameBtnClick()
	{
		GameStateManager.Instance().FSM.CurrentState.Message("StartGame", null);
	}
}
