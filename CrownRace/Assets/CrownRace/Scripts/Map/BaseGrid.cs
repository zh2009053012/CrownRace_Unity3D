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
	public Vector3 CenterPos{
		get{return m_trans.position;}
	}
}