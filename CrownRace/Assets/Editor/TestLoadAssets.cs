using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestLoadAssets : MonoBehaviour {

	[MenuItem("Tool/LoadAsset")]
	static void LoadAsset(){
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/CrownRace/Resources/Cards/card.prefab");
		if (prefab != null)
			Debug.Log ("success");
		else {
			Debug.Log ("null");
		}
	}
}
