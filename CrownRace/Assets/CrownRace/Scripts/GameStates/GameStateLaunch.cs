using UnityEngine;
using System.Collections;

public class GameStateLaunch : IStateBase {

	private GameStateLaunch()
	{}

	private static GameStateLaunch m_instance;
	private static object m_lockHelper = new object();
	public static GameStateLaunch Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateLaunch ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameStartUI ctr;
	public void Enter(GameStateBase owner)
	{
		Debug.Log ("enter GameStateLaunch");
		GameObject prefab = Resources.Load ("UI/GameStartUICanvas")as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ctr = go.GetComponent<GameStartUI> ();
		//
		TcpClientHelper.Instance.Close();
		TcpListenerHelper.Instance.Close ();
		GameGlobalData.PlayerID = -1;
		GameGlobalData.PlayerResName = "";
		GameGlobalData.ClearClientPlayerData ();
		GameGlobalData.ClearServerPlayerData ();
		GameGlobalData.InitResName ();
		//

		if (PlayerPrefs.HasKey ("volume")) {
			AudioManager.Instance.Volume = PlayerPrefs.GetFloat ("volume");
		} else {
			AudioManager.Instance.Volume = 1;
			PlayerPrefs.SetFloat ("volume", 1);
		}
		AudioManager.Instance.StopAudio ("game_scene_bg");
		AudioManager.Instance.PlayAudio ("start_scene_bg", true);
		AudioManager.Instance.StopAudio ("else_scene_bg");
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log ("exit GameStateLaunch");
		if (null != ctr && null != ctr.gameObject) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("OnCreateGameBtnClick")) {
			DoCreateGameBtnClick ();
		} else if (message.Equals ("OnJoinGameBtnClick")) {
			DoJoinGameBtnClick ();
		}
	}

	void DoCreateGameBtnClick()
	{
		GameStateManager.Instance ().FSM.ChangeState (GameStateServerWait.Instance ());
	}
	void DoJoinGameBtnClick()
	{
		GameStateManager.Instance ().FSM.ChangeState (GameStateClientConnect.Instance ());
	}
}
