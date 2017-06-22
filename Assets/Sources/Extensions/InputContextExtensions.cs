using System;
using System.Collections.Generic;
using Entitas;

public static class InputContextExtensions {

	public static InputEntity CreatePlayerInputEntity(this InputContext input, int playerId) {

		var e = input.CreateEntity();
		e.AddPlayer(playerId);
		e.AddPlayerInputs(new List<PlayerInputRecord>());

		return e;
	}
}