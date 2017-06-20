using System;
using UnityEngine;

public static class GameContextExtensions {

	const string monitorPrefabPath = "Monitor";

	/// Creates a computer monitor with the specified transform and screen buffer contents.
	public static GameEntity CreateMonitor(this GameContext game, 
		TransformState transformState,
		ScreenBufferState screenData
	) {

		var e = game.CreateEntity();
		e.AddTransform(transformState);
		e.AddScreenBuffer(screenData);
		e.AddPrefab(monitorPrefabPath);

		return e;
	}

    public static GameEntity CreateBullet(this GameContext game,
        TransformState transformState,
        RigidbodyState rigidbodyState,
        float damage
	) {

		var e = game.CreateEntity();
		e.AddTransform(transformState);
		e.AddRigidbodyState(rigidbodyState);
		e.isGameObjectDriven = true;
		e.isBullet = true;
		e.AddDamage(damage);

		e.AddPrefab("Bullet");

		e.ReplaceNetworkUpdatePriority(100, 10000);

		return e;
	}

	public static GameEntity CreatePlayer(this GameContext game,
		int playerId,
		TransformState transformState,
		RigidbodyState rigidbodyState
	) {
		
		var e = game.CreateEntity();

		e.AddTransform(transformState);
		e.AddRigidbodyState(rigidbodyState);

		e.isGameObjectDriven = true;
		e.AddPlayer(playerId);
		e.ReplaceNetworkUpdatePriority(10000, 0);
		e.AddPrefab("Player");

		return e;
	}

	public static GameEntity CreatePlayer(this GameContext game,
		int playerId,
		TransformState transformState
	) {

		return game.CreatePlayer(playerId, transformState, RigidbodyState.rest);
	}
}
