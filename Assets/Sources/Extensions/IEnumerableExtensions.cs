﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions {

	/// Applies an action to each element of the enumerable.
	public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action) {

		foreach (var item in enumerable) action(item);
	}

	///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
	///<param name="items">The enumerable to search.</param>
	///<param name="predicate">The expression to test the items against.</param>
	///<returns>The index of the first matching item, or -1 if no items match.</returns>
	public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
		
		if (items == null) throw new ArgumentNullException("items");
		if (predicate == null) throw new ArgumentNullException("predicate");

		int index = 0;
		foreach (var item in items) {
			
			if (predicate(item)) return index;
			index++;
		}

		return -1;
	}

	///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
	///<param name="items">The enumerable to search.</param>
	///<param name="item">The item to find.</param>
	///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
	public static int IndexOf<T>(this IEnumerable<T> items, T item) { 
		
		return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); 
	}
}
