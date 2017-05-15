using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class ShowCardUI : MonoBehaviour {
	private ShowCardCtr m_ctr;

	void Start(){
		GameObject prefab = Resources.Load("ShowCardCamera")as GameObject;
		GameObject go = GameObject.Instantiate(prefab);
		m_ctr = go.GetComponent<ShowCardCtr>();
		m_ctr.ShowNext();
	}


	public void OnLeftBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		m_ctr.ShowPre();
	}
	public void OnRightBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		m_ctr.ShowNext();
	}
	public void OnCloseBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		SceneLoading.LoadSceneName = GameGlobalData.LoginSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
}
