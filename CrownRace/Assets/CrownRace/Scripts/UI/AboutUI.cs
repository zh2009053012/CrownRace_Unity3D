﻿using UnityEngine;
using System.Collections;

public class AboutUI : MonoBehaviour {

	private ShowAboutCardCtr m_ctr;

	void Start(){
		GameObject prefab = Resources.Load("ShowDeveloperCamera")as GameObject;
		GameObject go = GameObject.Instantiate(prefab);
		m_ctr = go.GetComponent<ShowAboutCardCtr>();
		m_ctr.ShowNext();
	}


	public void OnLeftBtnClick(){
		m_ctr.ShowPre();
		AudioManager.Instance.PlayAudio ("click_btn", false);
	}
	public void OnRightBtnClick(){
		m_ctr.ShowNext();
		AudioManager.Instance.PlayAudio ("click_btn", false);
	}
	public void OnCloseBtnClick(){
		AudioManager.Instance.PlayAudio ("click_btn", false);
		SceneLoading.LoadSceneName = GameGlobalData.LoginSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);

	}
}
