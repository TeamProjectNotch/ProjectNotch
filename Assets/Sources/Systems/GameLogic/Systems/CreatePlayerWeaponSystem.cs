using System;
using Entitas;
using UnityEngine;

/// In the beginning of the game, creates a simple weapon and gives it to the player as a handheld item.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class CreatePlayerWeaponSystem : IInitializeSystem {

	readonly GameContext game;
	readonly IGroup<GameEntity> players;

	public CreatePlayerWeaponSystem(Contexts contexts) {

		game = contexts.game;
		players = game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.GameObject)
		);
	}

	public void Initialize() {

		var player = players.GetSingleEntity();
		if (player != null) {
			CreateWeaponForPlayer(player);
		} else {
			players.OnEntityAdded += HandleOnPlayerCreated;
		}
	}

	void CreateWeaponForPlayer(GameEntity player) {

		var e = CreateWeapon(); 
		player.ReplaceHandheld(e.id.value);
	}

	void HandleOnPlayerCreated(IGroup<GameEntity> group, GameEntity player, int index, IComponent component) {
	
		CreateWeaponForPlayer(player);
		group.OnEntityAdded -= HandleOnPlayerCreated;
	}

	GameEntity CreateWeapon() {

		var e = game.CreateEntity();

		e.AddFireRate(10f);
		e.AddDamage(5f);
		e.AddProjectileSpeed(100f);

		e.isGameObjectDriven = true;
		e.AddPrefab("Weapon");

		return e;
	}
}