﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// A helper class for reading and writing INetworkMessage to and from byte[].
public static class NetworkMessageSerializer {

	public static readonly Type[] messageTypes;
	public static readonly string[] messageTypeNames;
	public static readonly Dictionary<Type, byte> messageTypeIndices = new Dictionary<Type, byte>();

	public static byte[] Serialize(INetworkMessage message) {

		var stream = new MemoryStream();
		using (var writer = new MyWriter(stream)) {

			byte typeIndex = GetTypeIndexOf(message.GetType());
			writer.Serialize(ref typeIndex);

			message.Serialize(writer);
			return stream.ToArray();
		}
	}

	public static INetworkMessage Deserialize(byte[] bytes) {
		
		using (var reader = new MyReader(new MemoryStream(bytes))) {

			byte typeIndex = 0;
			reader.Serialize(ref typeIndex);
			var type = GetTypeBy(typeIndex);

			var message = (INetworkMessage)Activator.CreateInstance(type);
			message.Serialize(reader);
			return message;
		}
	}

	static NetworkMessageSerializer() {

		var types = AppDomain.CurrentDomain
			.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => HasNetworkMessageAttribute(type));

		messageTypes = types.ToArray();
		messageTypeNames = types.Select(type => GetMessageTypeName(type)).ToArray();
		PopulateTypeIndices();
	}

	static bool HasNetworkMessageAttribute(Type type)  {

		return GetNetworkMessageAttribute(type) != null;
	}

	static NetworkMessageAttribute GetNetworkMessageAttribute(Type type) {

		return type
			.GetCustomAttributes(inherit: true)
			.OfType<NetworkMessageAttribute>()
			.LastOrDefault();
	}

	static string GetMessageTypeName(Type type) {

		var attribute = GetNetworkMessageAttribute(type);
		return attribute.name ?? type.ToString();
	}

	static void PopulateTypeIndices() {
		
		int numTypes = messageTypes.Length;
		for (byte index = 0; index < numTypes; ++index) {

			messageTypeIndices.Add(messageTypes[index], index);
		}
	}

	static byte GetTypeIndexOf(Type type) {
		
		byte typeIndex;
		bool indexFound = messageTypeIndices.TryGetValue(type, out typeIndex);
		if (!indexFound) {

			throw new ArgumentException(String.Format(
				"Couldn't find type index of given type ({0})! Maybe it wasn't marked with NetworkMessageAttibute?",
				type
			));
		}

		return typeIndex;
	}

	static Type GetTypeBy(byte typeIndex) {

		if (typeIndex > messageTypes.Length) {

			throw new ArgumentException(String.Format(
				"Couldn't find type of given type index ({0})!",
				typeIndex
			));
		}

		return messageTypes[typeIndex];
	}
}
