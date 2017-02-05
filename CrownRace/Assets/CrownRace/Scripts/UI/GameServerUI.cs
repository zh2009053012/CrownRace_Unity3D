using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameServerUI : MonoBehaviour {
	public GridLayoutGroup gridGroupCtr;
	[SerializeField]
	private Text serverIPText;
	public string ServerIP{
		set{
			serverIPText.text = value;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
