using System;
using System.Linq;
using UnityEngine;
using Entitas;

/// A change in the state of a particular Entity.
/// If componentChanges is null, then the object is just a message that 
/// the Entity it refers to has been destroyed.
[Serializable]
public class EntityChange : IUnifiedSerializable {

	public int contextIndex;
	public ulong entityId;
	public ComponentChange[] componentChanges;

	public bool isRemoval => componentChanges == null;

	public static EntityChange MakeRemoval(int contextIndex, ulong entityId) {

        var change = new EntityChange {
            contextIndex = contextIndex,
            entityId = entityId,
            componentChanges = null
        };

        return change;
	}

    public static EntityChange MakeUpdate(
        int contextIndex, ulong entityId, 
        ComponentChange[] componentChanges
    ) {

        return new EntityChange() {
            contextIndex = contextIndex,
            entityId = entityId,
            componentChanges = componentChanges
        };
	}

	/// For deserialization only.
	public EntityChange() {}

	public void Apply(IEntity entity) {

        if (isRemoval) {

            Destroy(entity);
            Debug.Log($"EntityChange: (id : {entityId}) destroyed");

        } else {

            foreach (var componentChange in componentChanges) {

                componentChange.Apply(entity);
            }

            Debug.Log(
                $"EntityChange: (id : {entityId}) " +
                $"applied {componentChanges.Length} component changes"
            );
        }
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
}
