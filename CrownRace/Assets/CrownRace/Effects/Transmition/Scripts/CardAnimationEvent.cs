using UnityEngine;
using System.Collections;

public class CardAnimationEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (gameObject.layer);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.localPosition.z <= 5 && this.gameObject.layer == 8) {
			
			this.gameObject.layer = 0;
			Debug.Log ("change"+gameObject.layer);

		}
	}
	public void OnMoveOver()
	{
		Debug.Log ("move over");
		this.gameObject.SetActive (false);
	}
}
