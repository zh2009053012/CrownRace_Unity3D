using UnityEngine;
using System.Collections;

public class CMath  {

	public static Vector3 Lerp(Vector3 a, Vector3 b, float t){
		return new Vector3 (Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
	}
}
