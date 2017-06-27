using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Entitas;

/// Should be a server-side system.
/// Shoots a bullet out of a player's weapon when they press the fire button.
/// TEMP Only processes the last of the provided inputs.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class ShootWeaponOnPressFireSystem : ProcessInputSystem {

	readonly GameContext game;
	readonly IMatcher<GameEntity> weaponMatcher;

	const float bulletSpeed = 100f;

	public ShootWeaponOnPressFireSystem(Contexts contexts) : base(contexts) {

		game = contexts.game;

		weaponMatcher = GameMatcher.AllOf(
			GameMatcher.Transform,
			GameMatcher.ProjectileSpeed, 
			GameMatcher.Damage
		);
	}

	protected override void Process(GameEntity player, List<PlayerInputRecord> inputs) {

		var inputState = inputs[inputs.Count - 1].inputState;
		if (inputState.buttonPressedFire) {

			var weapon = GetWeaponOf(player);
			if (weapon == null) return;

			ShootBulletFrom(weapon);
		}
	}

	GameEntity GetWeaponOf(GameEntity player) {

		if (!player.hasHandheld) return null;

		var handheld = game.GetEntityWithId(player.handheld.id);
		if (handheld == null) return null;
		if (!weaponMatcher.Matches(handheld)) return null;

		return handheld;
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
