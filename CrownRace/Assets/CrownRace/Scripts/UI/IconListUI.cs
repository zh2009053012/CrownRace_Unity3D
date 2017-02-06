using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IconListUI : MonoBehaviour {

	private Dictionary<string, GameObject> m_iconList = new Dictionary<string, GameObject>();
	private string m_curIconName;
	public string CurIcon
	{
		get{return m_curIconName;}
	}
	private GameObject m_curIconObject;
	private bool m_isInit = false;
	void Awake()
	{
		if(!m_isInit)
			Init();
	}
	void Init()
	{
		for(int i=0; i < transform.GetChildCount(); i++)
		{
			Transform trans = transform.GetChild(i);
			if(!m_iconList.ContainsKey(trans.gameObject.name))
			{
				m_iconList.Add(trans.gameObject.name, trans.gameObject);
				trans.gameObject.SetActive(false);
			}
			else
			{
				Debug.LogError("IconListUI::Awake:Dictionary m_iconList is already contain key "+trans.gameObject.name);
			}
		}
		m_isInit = true;
	}
	// Use this for initialization
	void Start () {
		
		//Debug.Log("IconListUI::Start:Add "+m_iconList.Count+" icon to list.");
	}
	public void HideCurIcon()
	{
		if (m_curIconObject != null) {
			m_curIconObject.SetActive (false);
			m_curIconName = "";
			m_curIconObject = null;
		}
	}
	public bool HideIcon(string iconName)
	{
		GameObject go;
		if(m_iconList.TryGetValue(iconName, out go))
		{
			go.SetActive(false);
			if (iconName.Equals (m_curIconName)) {
				m_curIconName = "";
				m_curIconObject = null;
			}
			return true;
		}
		return false;
	}
	public bool ShowIcon(Sprite icon)
	{
		if(null == icon)
			return false;
		if(ShowIcon("none"))
		{
			Image image = m_curIconObject.GetComponent<Image>();
			if(image != null)
			{
				image.sprite = icon;
				return true;
			}
		}
		return false;
	}
	public bool ShowIcon(string iconName)
	{
		GameObject go;
		if (!m_isInit) {
			Init ();
		}
		if(m_iconList.TryGetValue(iconName, out go))
		{
			m_curIconName = iconName;
			if(m_curIconObject)
			{
				m_curIconObject.SetActive(false);
			}
			m_curIconObject = go;
			m_curIconObject.SetActive(true);
		}
		else
		{
			if(m_curIconObject)
			{
				m_curIconObject.SetActive(false);
				m_curIconObject = null;
				m_curIconName = string.Empty;
			}
			Debug.LogWarning("IconListUI::ShowIcon:Can not find icon "+iconName);
			return false;
		}
		return true;
	}

}
