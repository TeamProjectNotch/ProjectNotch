using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Tests the functionality of a ScreenView attached to the same GameObject.
[RequireComponent(typeof(ScreenView))]
public class ScreenViewTester : MonoBehaviour {

	ScreenView screenView;

	void Start () {

		screenView = GetComponent<ScreenView>();
	}

	void Update () {

		var resolution = screenView.resolution;
		
		for (int i = 0; i < 128; i++) {

			int x = Random.Range(0, resolution.x);
			int y = Random.Range(0, resolution.y);

			screenView.SetPixel(x, y, (byte)Random.Range(0, 15));
		}
	}
}
