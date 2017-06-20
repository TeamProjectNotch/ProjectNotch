using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions {

	/// Applies an action to each element of the enumerable.
	public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action) {

		foreach (var item in enumerable) action(item);
	}
}
