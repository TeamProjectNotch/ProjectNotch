using System;
using UnityEngine;

public struct RangeFloat {

	public float min;
	public float max;

	public RangeFloat(float min, float max) {
		
		this.min = min;
		this.max = max;
	}

	public float Clamp(float value) {

		return Mathf.Clamp(value, min, max);
	}
}

