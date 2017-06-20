using System;
using UnityEngine;

public enum ColorCode : byte {

	Black,
	DarkBlue,
	DarkGreen,
	DarkAqua,
	DarkRed,
	DarkPurple,
	Gold,
	Gray,
	DarkGray,
	Blue,
	Green,
	Aqua,
	Red,
	LightPurple,
	Yellow,
	White
}

public static class ColorCodeExtensions {

	static readonly Color[] colors = new Color[] {
		new Color(0f   , 0f   , 0f   , 1f),
		new Color(0f   , 0f   , 0.66f, 1f),
		new Color(0f   , 0.66f, 0f   , 1f),
		new Color(0f   , 0.66f, 0.66f, 1f),
		new Color(0.66f, 0f   , 0f   , 1f), 
		new Color(0.66f, 0f   , 0.66f, 1f),
		new Color(1f   , 0.66f, 0f   , 1f),
		new Color(0.66f, 0.66f, 0.66f, 1f),
		new Color(0.33f, 0.33f, 0.33f, 1f), 
		new Color(0.33f, 0.33f, 1f   , 1f),
		new Color(0.33f, 1f   , 0.33f, 1f),
		new Color(0.33f, 1f   , 1f   , 1f),
		new Color(1f   , 0.33f, 0.33f, 1f), 
		new Color(1f   , 0.33f, 1f   , 1f),
		new Color(1f   , 1f   , 0.33f, 1f),
		new Color(1f   , 1f   , 1f   , 1f)
	};

	public static Color ToColor(this ColorCode colorCode) {

		return colors[(int)colorCode];
	}
}

