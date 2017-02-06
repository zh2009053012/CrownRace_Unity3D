using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void VoidEvent();
public class MessageUI : MonoBehaviour {
	public Text m_messageText;
	public Image m_bg;
	private float m_showTime;
	private bool m_isHitClose = true;
	private VoidEvent m_callback;

	public void Init()
	{
		m_messageText.text = "";
		m_callback = null;
		this.gameObject.SetActive(false);
	}
	public void ShowMessage(string message, VoidEvent e, float showTime=3)
	{
		m_callback = e;
		m_isHitClose = true;
		//m_messageText.color = Color.white;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowMessage(string message, float showTime=3)
	{
		m_callback = null;
		m_isHitClose = true;
		//m_messageText.color = Color.white;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowMessage(string message, Color color, float showTime, bool isHitClose, VoidEvent e)
	{
		m_callback = e;
		m_isHitClose = isHitClose;
		m_messageText.color = color;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}

	private void SetMessage(string message)
	{
		m_messageText.text = message;
		float width = m_messageText.preferredWidth;
		m_messageText.rectTransform.sizeDelta = new Vector2(width, m_messageText.rectTransform.sizeDelta.y);
		m_bg.rectTransform.sizeDelta = new Vector2(width+40, m_bg.rectTransform.sizeDelta.y);
	}

	private IEnumerator Hide(float time)
	{
		yield return new WaitForSeconds(time);
		Hide();
	}
	private void Hide()
	{
		if(this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(false);
			if(null != m_callback)
			{
				m_callback.Invoke();
				m_callback = null;
			}
		}
	}

	public void OnCloseBtnClick()
	{
		if(m_isHitClose)
		{
			Hide();
		}
	}
}
