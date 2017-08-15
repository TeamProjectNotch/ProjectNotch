using System;
using System.Collections.Generic;
using UnityEngine;

public static class IUnifiedSerializerExtensions {

	public static void Serialize(this IUnifiedSerializer s, ref Vector3 value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
		s.Serialize(ref value.z);
	}

	public static void Serialize(this IUnifiedSerializer s, ref Vector3D value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
		s.Serialize(ref value.z);
	}

	public static void Serialize(this IUnifiedSerializer s, ref Vector2 value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
	}

	public static void Serialize(this IUnifiedSerializer s, ref Vector2D value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
	}

	public static void Serialize(this IUnifiedSerializer s, ref Quaternion value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
		s.Serialize(ref value.z);
		s.Serialize(ref value.w);
	}

	/// A helper function. Serializes/deserializes objects of non-primitive types.
	public static void Serialize<T>(this IUnifiedSerializer s, ref T serializable) 
		where T : IUnifiedSerializable, new() {

		if (s.isReading) {
			serializable = new T();
		}

		serializable.Serialize(s);
	}

	/// Serialization function for an array of IUnifiedSerializable|s. 
	/// T must have an public parameterless constructor.
	public static void Serialize<T>(this IUnifiedSerializer s, ref T[] array) 
		where T : IUnifiedSerializable, new() {

		int numElements = s.isWriting ? array.Length : 0;
		s.Serialize(ref numElements);

		if (s.isReading) {
			array = new T[numElements];
		}

		for (int i = 0; i < numElements; ++i) {

			s.Serialize(ref array[i]);
		}
	}

	/// Serialization function for a list of IUnifiedSerializable. 
	/// T must have an public parameterless constructor.
	public static void Serialize<T>(this IUnifiedSerializer s, ref List<T> list) 
		where T : IUnifiedSerializable, new() {

		int numElements = s.isWriting ? list.Count : 0;
		s.Serialize(ref numElements);

		if (s.isWriting) {

			for (int i = 0; i < numElements; ++i) {

				list[i].Serialize(s);
			}

		} else {

			list = new List<T>(numElements);
			for (int i = 0; i < numElements; ++i) {

				var element = new T();
				element.Serialize(s);
                list.Add(element);
			}
		}
	}
}
