using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

/// Shoots a bullet out of a player's weapon when they press the fire button.
public class ShootWeaponOnPressFireSystem : ReactiveSystem<InputEntity> {

	readonly GameContext game;
	readonly IGroup<GameEntity> players;

	readonly IMatcher<GameEntity> weaponMatcher;

	const float bulletSpeed = 100f;

	public ShootWeaponOnPressFireSystem(Contexts contexts) : base(contexts.input) {

		game = contexts.game;
		players = game.GetGroup(
			GameMatcher.AllOf(GameMatcher.Player, GameMatcher.Handheld)
		);

		weaponMatcher = GameMatcher.AllOf(
			GameMatcher.Transform,
			GameMatcher.ProjectileSpeed, 
			GameMatcher.Damage
		);
	}

	protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context) {

		return context.CreateCollector(InputMatcher.PressFire);
	}

	protected override bool Filter(InputEntity entity) {

		return true;
	}

	protected override void Execute(List<InputEntity> presses) {

		foreach (var press in presses) Process(press);
	}

	void Process(InputEntity press) {
		
		press.flagDestroy = true;

		// TEMP. Will somehow match the player to the client the button press came from.
		var player = players.GetSingleEntity();
		if (player == null) return;

		var handheld = game.GetEntityWithId(player.handheld.id);
		if (handheld == null) return;
		if (!weaponMatcher.Matches(handheld)) return;

		ShootBulletFrom(handheld);
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
