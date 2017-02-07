using UnityEngine;
using System.Collections;

public class BaseGrid : MonoBehaviour {

	protected uint m_ID;
	public uint ID{
		get{return m_ID;}
		set{m_ID = value;}
	}
	[SerializeField]
	protected Transform m_trans;
	[SerializeField]
	protected Transform m_transA;
	[SerializeField]
	protected Transform m_transB;
	[SerializeField]
	protected Transform m_transC;
	[SerializeField]
	protected Transform m_transD;
	public Vector3 CenterPos{
		get{return m_trans.position;}
	}
	public Vector3 PlayerPos(string res_name){
		Vector3 result=m_trans.position;
		switch (res_name) {
		case "PlayerA":
			result=m_transA.position;
			break;
		case "PlayerB":
			result=m_transB.position;
			break;
		case "PlayerC":
			result=m_transC.position;
			break;
		case "PlayerD":
			result=m_transD.position;
			break;
		}
		return result;
	}
}