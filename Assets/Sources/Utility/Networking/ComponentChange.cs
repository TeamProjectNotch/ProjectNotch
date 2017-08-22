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

    // TODO Extract a ComponentData class to hold these two fields.
	Type   componentType;
	byte[] componentData;

    public bool isRemoval => componentData == null;

	public static ComponentChange MakeRemoval(int componentIndex) {

        var change = new ComponentChange() {
            componentIndex = componentIndex
        };
        //Debug.Log($"ComponentChange: made removal {change}");

        return change;
	}

	public static ComponentChange MakeUpdate(int componentIndex, IComponent newComponent) {

        var change = new ComponentChange() {
            componentIndex = componentIndex
        };
        change.SaveState(newComponent);
        //Debug.Log($"ComponentChange: made update {change}");

		return change;
	}

	public ComponentChange() {}

	public void Apply(IEntity e) {

        if (isRemoval) 
            ApplyRemove(e);
        else 
            ApplyReplace(e);
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

        //Debug.Log($"ComponentChange: serialized {this.ToString()}");
	}

    public override string ToString() {

        var str = $"[ComponentChange: componentIndex : {componentIndex}, ";

        if (isRemoval) {
            str += "removal";
        } else {
            str += $"componentType : {componentType}, ";
            str += $"componentData : {componentData}";
        }

        str += "]";

        return str;
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

    string NameOfComponent(IEntity e, int componentIndex) {

        return e.contextInfo.componentNames[componentIndex];
    }

    void ApplyRemove(IEntity e) {

        if (e.HasComponent(componentIndex)) {

            e.RemoveComponent(componentIndex);

            Debug.LogFormat(
                "ComponentChange: removed {0}",
                e.contextInfo.componentNames[componentIndex]
            );

        } else {

            Debug.Log(
                "ComponentChange: " +
                "component not found, can't remove " +
                $"{NameOfComponent(e, componentIndex)}"
            );
        }
    }

    void ApplyReplace(IEntity e) {

        var component = e.CreateComponent(componentIndex, componentType);
        var asSerializable = component as IUnifiedSerializable;

        if (asSerializable == null) {
            Debug.LogError(
                "ComponentChange: " +
                $"component type {component.GetType()} " +
                "is not IUnifiedSerializable, can't deserialize"
            );
            return;
        }

        using (var reader = new MyReader(new MemoryStream(componentData))) {

            asSerializable.Serialize(reader);
            e.ReplaceComponent(componentIndex, component);
            Debug.Log(
                "ComponentChange: " +
                $"deserialized {NameOfComponent(e, componentIndex)}"
            );
        }
    }
}
