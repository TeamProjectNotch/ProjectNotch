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

	Type componentType;
	byte[] componentData;

    public bool isRemoval => componentData == null;

	public static ComponentChange MakeRemoval(int componentIndex) {

        var change = new ComponentChange() {
            componentIndex = componentIndex
        };

        return change;
	}

	public static ComponentChange MakeUpdate(int componentIndex, IComponent newComponent) {

        var change = new ComponentChange() {
            componentIndex = componentIndex
        };

        change.SaveState(newComponent);

		return change;
	}

	public ComponentChange() {}

	public void Apply(IEntity entity) {

        if (isRemoval) {

            entity.RemoveComponent(componentIndex);

            Debug.LogFormat(
                "ComponentChange: removed {0}", 
                entity.contextInfo.componentNames[componentIndex]
            );

        } else {

            var component = entity.CreateComponent(componentIndex, componentType);
            var asSerializable = component as IUnifiedSerializable;

            if (asSerializable == null) {
                Debug.Log(
                    $"ComponentChange: trying to deserialize component " +
                    "of type ({component.GetType()}), which is " +
                    "not IUnifiedSerializable!"
                );
            }

            using (var reader = new MyReader(new MemoryStream(componentData))) {

                asSerializable.Serialize(reader);
                entity.ReplaceComponent(componentIndex, component);
                Debug.LogFormat(
                    "ComponentChange: deserialized {0}", 
                    entity.contextInfo.componentNames[componentIndex]
                );
            }
        }
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref componentIndex);

		bool hasComponentData = s.isWriting ? componentData != null : false;
		s.Serialize(ref hasComponentData);

		if (hasComponentData) {

            SerializeComponentType(s);

			int componentDataLength = s.isWriting ? componentData.Length : 0;
			s.Serialize(ref componentDataLength);
			s.Serialize(ref componentData, componentDataLength);
		}
	}

    void SerializeComponentType<T>(T s) where T : IUnifiedSerializer {

		// TEMP Currently serializes the actual type name instead of some index.
		var typeName = s.isWriting ? componentType.ToString() : null;
		s.Serialize(ref typeName);

        if (s.isReading) {
            
			componentType = Type.GetType(typeName);
		}
    }

	void SaveState(IComponent c) {

		componentType = c.GetType();

		var stream = new MemoryStream();
		using (var writer = new MyWriter(stream)) {

			var component = c as IUnifiedSerializable;
            if (component == null) {
                throw new InvalidOperationException(
                   $"ComponentChange trying to serialize a non-IUnifiedSerializable component ({c})!"
                );
            }

			component.Serialize(writer);
			componentData = stream.ToArray();
		}
	}
}
