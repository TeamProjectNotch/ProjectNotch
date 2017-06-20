using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A helper class for the GameObjects' transform.
public static class GameObjects {
    
	static Transform _root;
	public static Transform root {
		get {
			return _root ?? (_root = GetRoot());
		}
	}

	static Transform GetRoot() {

		var go = GameObject.Find("GameObjects");

		if (go) return go.transform;

		return new GameObject("GameObjects").transform;
	}

}
