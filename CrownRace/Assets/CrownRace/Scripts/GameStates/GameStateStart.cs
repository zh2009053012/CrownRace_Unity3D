using UnityEngine;
using System.Collections;

public class GameStateStart : IStateBase {

	private GameStateStart()
	{}

	private static GameStateStart m_instance;
	private static object m_lockHelper = new object();
	public static GameStateStart Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateStart ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameStartUI ctr;
	public void Enter(GameStateBase owner)
	{
		Debug.Log ("enter GameStateStart");
		GameObject prefab = Resources.Load ("UI/GameStartUICanvas")as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ctr = go.GetComponent<GameStartUI> ();
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log ("exit GameStateStart");
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
