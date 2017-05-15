using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStartUI : MonoBehaviour {


	public void OnCreateGameBtnClick()
	{
		AudioManager.Instance.PlayAudio ("click_btn", false);
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnCreateGameBtnClick", null);
	}
	public void OnJoinGameBtnClick()
	{
		AudioManager.Instance.PlayAudio ("click_btn", false);
		GameStateManager.Instance ().FSM.CurrentState.Message ("OnJoinGameBtnClick", null);
	}
	public void OnExitBtnClick()
	{
		AudioManager.Instance.PlayAudio ("click_btn", false);
		Application.Quit ();
	}
	public void OnShowCardBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		SceneLoading.LoadSceneName = GameGlobalData.ShowCardSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
	public void OnAboutBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		SceneLoading.LoadSceneName = GameGlobalData.AboutSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
}
