using UnityEngine;
using System.Collections;

public class GameStartUI : MonoBehaviour {


	public void OnCreateGameBtnClick()
	{
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnCreateGameBtnClick", null);
	}
	public void OnJoinGameBtnClick()
	{
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnJoinGameBtnClick", null);
	}
	public void OnExitBtnClick()
	{
		Application.Quit ();
	}
}
