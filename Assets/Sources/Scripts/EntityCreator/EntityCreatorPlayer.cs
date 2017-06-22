using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// UNIFIED-ONLY script!
/// Creates a player Entity the GameObject of this script would represent.
/// Sets game.thisPlayerId to the id of the current player.
public class EntityCreatorPlayer : EntityCreator {

	public override GameEntity CreateEntity(Contexts contexts) {

		// This part is the reason you should only 
		// use this script in a unified client-server setup.
		// On a regular client the player id would come from the server.
		var playerId = GetPlayerId(contexts);
		contexts.game.ReplaceThisPlayerId(playerId);

		var e = contexts.game.CreatePlayer(playerId, transform.GetState());
		TryAddRigidbodyState(e);

		return e;
	}

	int GetPlayerId(Contexts contexts) {

		var game = contexts.game;
		return game.hasThisPlayerId ? game.thisPlayerId.value + 1 : 0;
	}

	void TryAddRigidbodyState(GameEntity e) {

		var rigidbody = GetComponentInChildren<Rigidbody>();
		if (rigidbody != null && !rigidbody.isKinematic) {
			e.ReplaceRigidbodyState(rigidbody.GetState());
		}
	}
}
