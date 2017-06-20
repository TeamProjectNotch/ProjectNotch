using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Creates a player Entity the GameObject of this script would represent.
public class EntityCreatorPlayer : EntityCreator {

	public override GameEntity CreateEntity(Contexts contexts) {

		// TEMP Need to get the new player id from the server.
		var playerId = 0;

		var rigidbody = GetComponentInChildren<Rigidbody>();
		if (rigidbody != null && !rigidbody.isKinematic) {

			return contexts.game.CreatePlayer(
				playerId, 
				transform.GetState(),
				rigidbody.GetState()
			);
		}

		return contexts.game.CreatePlayer(
			playerId, 
			transform.GetState()
		);
	}

	void TryAddRigidbodyState(GameEntity e) {

		var rigidbody = GetComponentInChildren<Rigidbody>();
		if (rigidbody != null && !rigidbody.isKinematic) {
			e.AddRigidbodyState(rigidbody.GetState());
		}
	}
}
