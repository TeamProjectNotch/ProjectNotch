using System;
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

	public static void Serialize(this IUnifiedSerializer s, ref Quaternion value) {

		s.Serialize(ref value.x);
		s.Serialize(ref value.y);
		s.Serialize(ref value.z);
		s.Serialize(ref value.w);
	}

	/// Serialization function for an array of IUnifiedSerializable. 
	/// T must have an public parameterless constructor.
	public static void Serialize<T>(this IUnifiedSerializer s, ref T[] array) where T : IUnifiedSerializable, new() {

		int numElements = s.isWriting ? array.Length : 0;
		s.Serialize(ref numElements);

		if (s.isWriting) {

			for (int i = 0; i < numElements; ++i) {

				array[i].Serialize(s);
			}

		} else {

			array = new T[numElements];
			for (int i = 0; i < numElements; ++i) {

				array[i] = new T();
				array[i].Serialize(s);
			}
		}
	}
}
