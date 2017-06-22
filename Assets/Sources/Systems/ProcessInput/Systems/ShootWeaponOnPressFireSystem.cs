using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Should be a server-side system.
/// Shoots a bullet out of a player's weapon when they press the fire button.
public class ShootWeaponOnPressFireSystem : IExecuteSystem {

	readonly GameContext game;
	readonly IGroup<GameEntity> players;

	readonly InputContext input;

	readonly IMatcher<GameEntity> weaponMatcher;

	const float bulletSpeed = 100f;

	public ShootWeaponOnPressFireSystem(Contexts contexts){

		game = contexts.game;
		players = game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.Handheld)
		);

		input = contexts.input;

		weaponMatcher = GameMatcher.AllOf(
			GameMatcher.Transform,
			GameMatcher.ProjectileSpeed, 
			GameMatcher.Damage
		);
	}

	public void Execute() {

		foreach (var player in players.GetEntities()) {

			Process(player);
		}
	}

	void Process(GameEntity player) {

		PlayerInputState inputState;
		bool didGetInput = GetCurrentInput(player, out inputState);
		if (!didGetInput) return;

		if (inputState.buttonPressedFire) {
			
			var handheld = game.GetEntityWithId(player.handheld.id);
			if (handheld == null) return;
			if (!weaponMatcher.Matches(handheld)) return;

			ShootBulletFrom(handheld);
		}
	}

	// TEMP Code duplication! Same method in ProcessPlayerInputSystem
	bool GetCurrentInput(GameEntity playerEntity, out PlayerInputState result) {

		result = new PlayerInputState();

		var inputEntity = input.GetEntityWithPlayer(playerEntity.player.id);
		if (inputEntity == null) return false;
		if (!inputEntity.hasPlayerInputs) return false;

		var inputs = inputEntity.playerInputs.inputs;
		if (inputs.Count <= 0) return false;

		var mostRecentRecord = inputs[inputs.Count - 1];

		var currentTick = game.currentTick.value;
		var timestamp = mostRecentRecord.timestamp;
		if (timestamp > currentTick) { 

			throw new Exception(String.Format(
				"The most recent input record is {0}, which is later than the current tick {1}", 
				timestamp, 
				currentTick
			));
		}
		if (timestamp < currentTick) return false;

		result = mostRecentRecord.inputState;

		return true;
	}

	void ShootBulletFrom(GameEntity weapon) {

		var transformState = weapon.transform.state;
		var forward = new Vector3D(transformState.rotation * Vector3.forward);
		transformState.position += forward;

		var velocity = forward * weapon.projectileSpeed.value;
		var rigidbodyState = new RigidbodyState(velocity);

		var bulletDamage = weapon.damage.value;

		game.CreateBullet(transformState, rigidbodyState, bulletDamage);
	}
}
