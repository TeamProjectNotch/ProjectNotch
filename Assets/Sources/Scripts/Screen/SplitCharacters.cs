using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AUTHOR: LUDEREGAMES
// READS CHARACTER SHEET

public class SplitCharacters : MonoBehaviour {

	public Sprite[] array;

	private string[] charCache;

	void Start () {
		

		/*// Calculate number of cells
		int cellsX = (int)Mathf.Floor(fontSheet.width / 4);
		int cellsY = (int)Mathf.Floor(fontSheet.height / 8);
		// Loop through each cell
		for(int i = 0; i < cellsY; i++) {
			for(int i2 = 0; i2 < cellsX; i2++) {
				// Loop through each pixel in each cell
				for(int y = 0; y < 8; y++) {
					for(int x = 0; x < 4; x++) {
						if(fontSheet.GetPixel(x , y) != new Color(1, 1, 1, 1)) {
							Debug.Log(x + " : " + y);
						}
					}
				}
			}
		}*/
	}

}
