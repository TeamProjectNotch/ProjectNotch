using Entitas;

/// Destroys entities that were flagged for destruction
public class DestroySystem : ICleanupSystem {

    readonly IGroup<GameEntity> flaggedInGame;
	readonly IGroup<InputEntity> flaggedInInput;
	readonly IGroup<EventsEntity> flaggedInEvents;

	public DestroySystem(Contexts contexts) {
		
        flaggedInGame = contexts.game.GetGroup(GameMatcher.Destroy);
        flaggedInInput = contexts.input.GetGroup(InputMatcher.Destroy);
		flaggedInEvents = contexts.events.GetGroup(EventsMatcher.Destroy);
	}

	public void Cleanup() {

        foreach (var e in flaggedInGame.GetEntities()) e.Destroy();
        foreach (var e in flaggedInInput.GetEntities()) e.Destroy();
		foreach (var e in flaggedInEvents.GetEntities()) e.Destroy();
	}
}
