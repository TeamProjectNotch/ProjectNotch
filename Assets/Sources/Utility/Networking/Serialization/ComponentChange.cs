using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entitas;
using System.IO;

/// A change in a Component of an Entity.
/// If the change in question is a Component getting removed, it's just a message of that removal. 
/// If not, also stores the state of the Component as a byte[].
[Serializable]
public class ComponentChange : IUnifiedSerializable {

	public int componentIndex;
	public bool isRemoval;

	Type componentType;
	byte[] componentData;

	public static ComponentChange MakeRemoval(int componentIndex) {

		var change = new ComponentChange();
		change.componentIndex = componentIndex;
		change.isRemoval = true;

		return change;
	}

	public static ComponentChange MakeUpdate(int componentIndex, IComponent newComponent) {

		var change = new ComponentChange();
		change.componentIndex = componentIndex;
		change.isRemoval = false;
		change.SaveState(newComponent);

		return change;
	}

	public static ComponentChange Deserialize<T>(T serializer) where T : IUnifiedSerializer {

		if (serializer.isWriting) {
			
			throw new ArgumentException("Can't deserialize a ComponentChange using a reading serializer!");
		}

		var change = new ComponentChange();
		change.Serialize(serializer);
		return change;
	}

	ComponentChange() {}

	public void Apply(IEntity entity) {

		if (isRemoval) {
			
			entity.RemoveComponent(componentIndex);

			//Debug.LogFormat("ComponentChange removed {0}", entity.contextInfo.componentNames[componentIndex]);
			return;
		}

		var component = entity.CreateComponent(componentIndex, componentType);
		var asSerializable = component as IUnifiedSerializable;

		if (component == null) return;
		if (componentData == null) return;
		using (var reader = new MyReader(new MemoryStream(componentData))) {
			
			asSerializable.Serialize(reader);
			entity.ReplaceComponent(componentIndex, component);
			//Debug.LogFormat("ComponentChange deserialized {0}", entity.contextInfo.componentNames[componentIndex]);
		}
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref componentIndex);

		s.Serialize(ref isRemoval);
		if (!isRemoval) {

			// TEMP Currently serializes the actual type name instead of an index.
			var typeName = s.isWriting ? componentType.ToString() : null;
			s.Serialize(ref typeName);
			if (!s.isWriting) componentType = Type.GetType(typeName);

			bool hasComponentData = s.isWriting ? componentData != null : false;
			s.Serialize(ref hasComponentData);

			if (hasComponentData) {

				int componentDataLength = s.isWriting ? componentData.Length : 0;
				s.Serialize(ref componentDataLength);

				s.Serialize(ref componentData, componentDataLength);
			}
		}
	}

	void SaveState(IComponent c) {

		if (isRemoval) {
			throw new InvalidOperationException("Can't save the state of a Component in a Removal ComponentChange!");
		}

		componentType = c.GetType();

		var stream = new MemoryStream();
		using (var writer = new MyWriter(stream)) {

			var component = c as IUnifiedSerializable;
			if (component == null) return;

			component.Serialize(writer);
			componentData = stream.ToArray();
		}
	}
}
