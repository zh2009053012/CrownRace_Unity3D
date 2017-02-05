using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void VoidEvent();
public class MessageUI : MonoBehaviour {
	[SerializeField]
	private Text m_message;
	private VoidEvent m_callback;

	void Awake()
	{
		gameObject.SetActive(false);
	}

	public void ShowMessage(string message, float seconds, VoidEvent callback=null)
	{
		gameObject.SetActive(true);
		m_message.text = message;
		m_callback = callback;
		StartCoroutine(HideSelf(seconds));
	}
	IEnumerator HideSelf(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		gameObject.SetActive(false);
		if(null != m_callback)
		{
			m_callback.Invoke();
		}
	}
}
