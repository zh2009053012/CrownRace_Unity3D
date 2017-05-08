using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
	public void OnShowCardBtnClick(){
		SceneLoading.LoadSceneName = GameGlobalData.ShowCardSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
	public void OnAboutBtnClick(){
		SceneLoading.LoadSceneName = GameGlobalData.AboutSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
}
