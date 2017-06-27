using System;
using Entitas;

/// Server-side system.
/// Initializes the tick counter. 
/// Increments it on Execute.
[SystemAvailability(InstanceKind.All)]
public class TicksSystem : IInitializeSystem, IExecuteSystem {

	readonly GameContext game;

	public TicksSystem(Contexts contexts) {

		game = contexts.game;
	}

	public void Initialize() {

		game.SetCurrentTick(0);
	}

	public void Execute() {

		game.ReplaceCurrentTick(game.currentTick.value + 1);
	}
}

