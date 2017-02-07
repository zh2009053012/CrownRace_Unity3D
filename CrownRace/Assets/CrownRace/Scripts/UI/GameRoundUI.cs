using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameRoundUI : MonoBehaviour {
	public Button RollDiceBtn;
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
}
