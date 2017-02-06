using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour {
	[SerializeField]
	private Transform iconListUITrans;
	private IconListUI iconListUI;
	[SerializeField]
	private Text ipText;
	[SerializeField]
	private Text resNameText;

	public void SetInfo(string resName, string ip)
	{
		if (iconListUI == null) {
			iconListUI = iconListUITrans.GetComponentInChildren<IconListUI> ();
		}
		iconListUI.ShowIcon (resName);
		ipText.text = ip;
		resNameText.text = resName;
	}

}
