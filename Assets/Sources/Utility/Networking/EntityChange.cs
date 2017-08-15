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

	public int contextIndex;
	public ulong entityId;
	public ComponentChange[] componentChanges;

	public bool isRemoval {get {return componentChanges == null;}}

	public static EntityChange MakeRemoval(int contextId, ulong entityId) {

		var change = new EntityChange();

		change.contextIndex = contextId;
		change.entityId = entityId;

		return change;
	}

	public static EntityChange MakeUpdate(int contextId, ulong entityId, ComponentChange[] componentChanges) {
		
		var change = new EntityChange();

		change.contextIndex = contextId;
		change.entityId = entityId;
		change.componentChanges = componentChanges;

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

		s.Serialize(ref contextIndex);
		s.Serialize(ref entityId);

		bool hasComponentChanges = s.isWriting ? componentChanges != null : false;
		s.Serialize(ref hasComponentChanges);

		if (hasComponentChanges) {
			
			s.Serialize(ref componentChanges);
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
