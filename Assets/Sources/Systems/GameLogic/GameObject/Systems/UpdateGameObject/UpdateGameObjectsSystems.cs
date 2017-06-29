using System;

/// Updates GameObjects when their Entities' state changes.
public class UpdateGameObjectsSystems : MyFeature {

	public UpdateGameObjectsSystems(Contexts contexts) : base("UpdateGameObjects") {

		Add(new UpdateGameObjectTransformSystem(contexts));
		Add(new UpdateGameObjectRigidbodySystem(contexts));
	}
}

