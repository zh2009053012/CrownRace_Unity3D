using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour {
	public static string LoadSceneName;
	public Slider m_slider;
	public Text m_text;
    AsyncOperation async;
  
    int progress = 0;  
  
    void Start()  
    {  
		if (null == m_slider || null == m_text) {
			Debug.LogError ("SceneLoading/Start: Slider or Text is null");
		}
		StartCoroutine(LoadScene());  
    }  
  

    IEnumerator LoadScene()  
    {  
        yield return new WaitForEndOfFrame();

		if (string.IsNullOrEmpty (LoadSceneName)) {
			Debug.LogWarning ("SceneLoading::loadScene: scene name is null.");
			LoadSceneName = GameGlobalData.LoginSceneName;
		}
		async = SceneManager.LoadSceneAsync(LoadSceneName);  
  
        yield return async;  
    }  
  
    void Update()  
    {  
		if (null != async) {
			progress = (int)(async.progress * 100);  
			m_slider.value = async.progress;
			m_text.text = (progress).ToString () + "%";
		}
    }   
}
