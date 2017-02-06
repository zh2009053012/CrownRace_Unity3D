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
	public string ResName{
		get{ return resNameText.text;}
	}

	private int playerID;
	public int PlayerID{
		get{return playerID;}
	}

	public void SetInfo(int player_id, string resName, string ip)
	{
		playerID = player_id;
		if (iconListUI == null) {
			iconListUI = iconListUITrans.GetComponentInChildren<IconListUI> ();
		}
		iconListUI.ShowIcon (resName);
		ipText.text = ip;
		resNameText.text = resName;
	}

}
