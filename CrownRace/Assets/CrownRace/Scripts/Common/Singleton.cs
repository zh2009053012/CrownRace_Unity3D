using UnityEngine;
using System.Collections;

public abstract class Singleton<T> where T : new() {
	private static T instance;
	private static readonly object lockHelper = new object();
	public static T Instance{
		get{ 
			if (null == instance) {
				lock (lockHelper) {
					if (null == instance) {
						instance = new T ();
					}
				}
			}
			return instance;
		}
	}
}
