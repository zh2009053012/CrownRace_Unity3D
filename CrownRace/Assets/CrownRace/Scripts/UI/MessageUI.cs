using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void VoidEvent();
public delegate void ParameterEvent(object[] p);
public class MessageUI : MonoBehaviour {
	public Text m_messageText;
	public Image m_bg;
	public Image m_mask;
	private float m_showTime;
	private bool m_isHitClose = true;
	private ParameterEvent m_callback;
	private object[] m_parameter;

	public void Init()
	{
		m_messageText.text = "";
		m_callback = null;
		this.gameObject.SetActive(false);
	}
	//In order to prevent the current msg shut down by other msg. this will cause the current callback would never invoke.
	void CheckCallback()
	{
		if (null != m_callback) {
			m_callback.Invoke (m_parameter);
		}
	}
	public void ShowNotify(string message, float showTime=3)
	{
		CheckCallback ();
		m_callback = null;
		m_parameter = null;
		m_isHitClose = true;
		m_bg.enabled = false;
		m_mask.enabled = false;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowNotify(string message, ParameterEvent e, object[] p, float showTime=3)
	{
		CheckCallback ();
		m_callback = e;
		m_parameter = p;
		m_isHitClose = true;
		m_bg.enabled = false;
		m_mask.enabled = false;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowMessage(string message, ParameterEvent e, object[] p, float showTime=3)
	{
		CheckCallback ();
		m_callback = e;
		m_parameter = p;
		m_isHitClose = true;
		m_bg.enabled = true;
		m_mask.enabled = true;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowMessage(string message, float showTime=3)
	{
		CheckCallback ();
		m_callback = null;
		m_parameter = null;
		m_isHitClose = true;
		m_bg.enabled = true;
		m_mask.enabled = true;
		SetMessage(message);
		this.gameObject.SetActive(true);
		StartCoroutine(Hide(showTime));
	}
	public void ShowMessage(string message, Color color, float showTime, bool isHitClose, ParameterEvent e, object[] p)
	{
		CheckCallback ();
		m_callback = e;
		m_parameter = p;
		m_isHitClose = isHitClose;
		m_bg.enabled = true;
		m_mask.enabled = true;
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
				m_callback.Invoke(m_parameter);
				m_callback = null;
				m_parameter = null;
				m_messageText.text = "";
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
