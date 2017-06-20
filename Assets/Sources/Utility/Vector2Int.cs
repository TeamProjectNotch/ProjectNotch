using System;
using UnityEngine;

/// A 2-dimensional vector of integers.
[Serializable]
public struct Vector2Int {

	public static readonly Vector2Int zero  = new Vector2Int( 0,  0);
	public static readonly Vector2Int one   = new Vector2Int( 1,  1);

	public static readonly Vector2Int right = new Vector2Int( 1,  0);
	public static readonly Vector2Int up    = new Vector2Int( 0,  1);
	public static readonly Vector2Int left  = new Vector2Int(-1,  0);
	public static readonly Vector2Int down  = new Vector2Int( 0, -1);

	public static readonly Vector2Int[] directions = {right, up, left, down};


    public int x;
    public int y;

    public Vector2Int(int x, int y) {
        
        this.x = x;
        this.y = y;
    }

	public static implicit operator Vector2(Vector2Int vec) {
		return new Vector2(vec.x, vec.y);
	}

	public static implicit operator Vector3(Vector2Int vec) {
		return new Vector3(vec.x, vec.y);
	}

	public static Vector2Int operator +(Vector2Int a, Vector2Int b) {
		return new Vector2Int(a.x + b.x, a.y + b.y);
	}

	public static Vector2Int operator -(Vector2Int a, Vector2Int b) {
		return new Vector2Int(a.x - b.x, a.y - b.y);
	}

	public static Vector2Int operator *(Vector2Int vector, int factor) {
		return new Vector2Int(vector.x * factor, vector.y * factor);
	}

	public static Vector2Int operator /(Vector2Int vector, int factor) {
		return new Vector2Int(vector.x / factor, vector.y / factor);
	}
}
