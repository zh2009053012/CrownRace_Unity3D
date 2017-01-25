using UnityEngine;
using System.Collections;

public class ShipAnim : MonoBehaviour {

	public float RockRange = 0.5f;
	private Vector3 worldPos=Vector3.zero;
	private Vector3 normal = Vector3.zero;
	private Vector3 waveDis = Vector3.zero;
	// Use this for initialization
	void Start () 
	{
		worldPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		
		waveDis = GerstnerWave16.CalculateShipMovement (worldPos);
		transform.position = new Vector3 (worldPos.x, waveDis.y+worldPos.y, worldPos.z);
		normal = Vector3.Normalize (new Vector3(waveDis.x, 1/RockRange, waveDis.z));
		Vector3 forward = Vector3.Cross (normal, transform.forward);//right
		forward = Vector3.Cross (forward, normal);
		Quaternion q = Quaternion.LookRotation (forward, normal);
		transform.rotation = q;

	}
}
