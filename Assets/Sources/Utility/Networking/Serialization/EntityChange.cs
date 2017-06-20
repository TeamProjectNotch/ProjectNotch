using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Entitas;
using Entitas.Blueprints;

using fNbt;

/// A change in the state of a particular Entity.
/// If componentChanges is null, then the object is just a message that the Entity it refers to has been destroyed.
[Serializable]
public class EntityChange : IUnifiedSerializable {

	public ulong entityId;
	public bool isRemoval;

	public ComponentChange[] componentChanges;

	public static EntityChange MakeRemoval(ulong entityId) {

		var change = new EntityChange();

		change.entityId = entityId;
		change.isRemoval = true;

		return change;
	}

	public static EntityChange MakeUpdate(ulong entityId, ComponentChange[] componentChanges) {
		
		var change = new EntityChange();

		change.entityId = entityId;
		change.componentChanges = componentChanges;
		change.isRemoval = false;

		return change;
	}

	/// For later deserialization only.
	public EntityChange() {}

	public void Apply(IEntity entity) {

		if (isRemoval) {

			Destroy(entity);

			Debug.LogFormat("EntityChange destroyed Entity with id: {0}", entityId);
			return;
		}

		for (int i = 0; i < componentChanges.Length; ++i) {

			var componentChange = componentChanges[i];
			if (componentChange == null) continue;

			componentChange.Apply(entity);
		}

		//Debug.LogFormat("EntityChange applied changes to Entity with id: {0}", entityId);
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref entityId);

		bool hasComponentChanges = s.isWriting ? !isRemoval : false;
		s.Serialize(ref hasComponentChanges);
		isRemoval = !hasComponentChanges;

		if (hasComponentChanges) {

			int numComponentChanges = s.isWriting ? componentChanges.Length : 0;
			s.Serialize(ref numComponentChanges);

			if (s.isWriting) {
				
				for (int i = 0; i < numComponentChanges; ++i) {

					componentChanges[i].Serialize(s);
				}

			} else {

				componentChanges = new ComponentChange[numComponentChanges];
				for (int i = 0; i < numComponentChanges; ++i) {

					componentChanges[i] = ComponentChange.Deserialize(s);
				}
			}
		}
	}

	void Destroy(IEntity entity) {

		var destructible = entity as IDestroy;
		if (destructible != null) {
			destructible.flagDestroy = true;
		} else {
			entity.Destroy();
		}
	}

	int GetNumComponentChanges() {

		int count = 0;
		for (int i = 0; i < componentChanges.Length; ++i) {

			if (componentChanges[i] != null) {
				++count;
			}
		}

		return count;
	}
}
