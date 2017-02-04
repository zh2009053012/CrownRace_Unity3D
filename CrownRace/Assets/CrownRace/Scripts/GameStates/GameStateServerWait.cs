using UnityEngine;
using System.Collections;

public class GameStateServerWait : IStateBase {

	private GameStateServerWait()
	{}

	private static GameStateServerWait m_instance;
	private static object m_lockHelper = new object();
	public static GameStateServerWait Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateServerWait ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameServerUI ctr;
	public void Enter(GameStateBase owner)
	{
		GameObject prefab = Resources.Load ("UI/GameServerUICanvas")as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ctr = go.GetComponent<GameServerUI> ();
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		if (null != ctr && null != ctr.gameObject) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{

	}
}
