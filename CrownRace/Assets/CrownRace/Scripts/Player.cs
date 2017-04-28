using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	[SerializeField]
	private NavMeshAgent m_agentCtr;
	public MapGrid CurMapGrid;
	private bool m_isMoving=false;
	private int m_player_id;
	public int PlayerID{
		get{ return m_player_id;}
		set{ m_player_id = value;}
	}
	private string m_res_name;
	public string ResName{
		get{ return m_res_name;}
		set{ m_res_name = value;}
	}
	//
	private List<MapGrid> m_targetList = new List<MapGrid>();

	void Awake(){
		if(null == m_agentCtr)
		{
			m_agentCtr = GetComponent<NavMeshAgent>();
		}
	}

	public bool GotoMapGrid(List<MapGrid> target)
	{
		if(null == target || target.Count <= 0)
			return false;
		m_targetList.Clear ();
		m_targetList = target;
//		if (m_targetList[0] == CurMapGrid) {
//			SendMoveOverMessage ();
//			return true;
//		}
		Debug.Log ("GotoMapGrid:"+m_targetList[0].ID);
		m_agentCtr.SetDestination (m_targetList [0].PlayerPos (ResName));

		#if UNITY_EDITOR
		NavMeshPath path = new NavMeshPath();
		m_agentCtr.CalculatePath (m_targetList [0].PlayerPos (ResName), path);
		points.Clear();
		points.AddRange(path.corners);
		#endif
		m_targetList.RemoveAt (0);
		return true;
	}
	private List<Vector3> points = new List<Vector3>();
	void OnDrawGizmos(){
		#if UNITY_EDITOR
		Gizmos.color = Color.red;
		for (int i = 0; i < points.Count - 1; i++) {
			Gizmos.DrawLine (points [i], points [i + 1]);
		}
		#endif
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (m_agentCtr.hasPath) {
			m_isMoving = true;
		}
		if(m_isMoving && Mathf.Abs( m_agentCtr.remainingDistance ) <= 0.01f)
		{
			if (m_targetList.Count > 0) {
				//CurMapGrid = m_targetList [0];
				Debug.Log ("GotoMapGrid:Update:"+m_targetList[0].ID);
				m_agentCtr.SetDestination (m_targetList [0].PlayerPos (ResName));
				m_targetList.RemoveAt (0);
			} else {
				Debug.Log ("move over");
				m_isMoving = false;
				SendMoveOverMessage ();
			}
		}
	}
	void SendMoveOverMessage()
	{
		object[] p = new object[1];
		p[0] = (object)PlayerID;
		GameStateManager.Instance().FSM.CurrentState.Message("MovingOver", p);
	}
}
