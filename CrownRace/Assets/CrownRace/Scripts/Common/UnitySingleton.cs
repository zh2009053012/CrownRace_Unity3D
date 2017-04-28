using UnityEngine;
using System.Collections;

public class UnitySingleton<T>: MonoBehaviour where T : Component
{
	private static T instance;
	public static T Instance{
		get{ 
			if (null == instance) {
				instance = GameObject.FindObjectOfType<T> ()as T;
				if (null == instance) {
					GameObject go = new GameObject ();
					go.name = "UnitySingleton";
					instance = go.AddComponent<T> ();
				}
			}
			return instance;
		}
	}
	public virtual void Awake(){
		DontDestroyOnLoad (this.gameObject);
	}
}
