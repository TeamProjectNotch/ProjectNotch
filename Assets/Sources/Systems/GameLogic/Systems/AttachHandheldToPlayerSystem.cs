using System;
using System.Collections.Generic;
using Entitas;

// TODO Make a system that would detach the old handheld Entity.
/// Whenever the HandheldComponent of a player changes, attaches the GameObject of the Entity 
/// the player is holding to a handle on the player's GameObject.
public class AttachHandheldToPlayerSystem : ReactiveSystem<GameEntity> {

	readonly GameContext game;

	public AttachHandheldToPlayerSystem(Contexts contexts) : base(contexts.game) {

		game = contexts.game;
	}

	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {

		return context.CreateCollector(GameMatcher.Handheld.Added());
	}

	protected override bool Filter(GameEntity player) {

		return player.hasHandheld && player.hasGameObject;
	}

	protected override void Execute(List<GameEntity> players) {

		foreach (var player in players) Process(player); 
	}

	void Process(GameEntity player) {

		var entity = game.GetEntityWithId(player.handheld.id);
		if (entity == null) {
			
			UnityEngine.Debug.LogError("Invalid entity id in HandheldComponent!");
			return;
		};

		if (entity.hasGameObject) {
			
			Attach(entity, player);
		} else {

			AttachWhenGameObjectAdded(entity, player);
		}
	}

	void Attach(GameEntity entity, GameEntity player) {

		// TEMP There's probably a better way to do this than searching by name.
		var handleTransform = player.gameObject.value.transform.FindRecursive("HandheldHandle");

		var transform = entity.gameObject.value.transform;
		transform.SetParent(handleTransform, worldPositionStays: false);
		transform.position = handleTransform.position;
	}

	void AttachWhenGameObjectAdded(GameEntity entity, GameEntity player) {

		// Making lambdas unsubscribe themselves is kinda tricky, 
		// but we need to use a lambda since we need to capture the player variable.
		EntityComponentChanged onComponentAddedHandler = null;
		onComponentAddedHandler = (e, index, component) => {

			if (index != GameComponentsLookup.GameObject) return;

			if (!player.hasHandheld || player.handheld.id != entity.id.value) return;

			Attach(entity, player);

			entity.OnComponentAdded -= onComponentAddedHandler;
		};

		entity.OnComponentAdded += onComponentAddedHandler;
	}
}