using System;

public static class ArrayExtensions {
	
	public static int Count<T>(this T[] array, T value) {

		int count = 0;
		int length = array.Length;
		for (int i = 0; i < length; ++i) {

			if (array[i].Equals(value)) ++count;
		}

		return count;
	}

	public static void Fill<T>(this T[] array, T value) {

		int length = array.Length;
		for (int i = 0; i < length; ++i) array[i] = value;
	}
} 
