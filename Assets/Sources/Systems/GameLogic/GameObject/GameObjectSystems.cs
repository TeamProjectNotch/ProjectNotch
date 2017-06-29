using Entitas;

/// Systems which manage GameObjects and their relations with Entities.
public class GameObjectSystems : MyFeature {

    public GameObjectSystems(Contexts contexts) : base("GameObject") {
        
		Add(new CreateEntitiesFromGameObjectsSystem(contexts));
		Add(new CreateGameObjectsFromEntitiesSystem(contexts));
		Add(new LinkGameObjectToEntitySystem(contexts));

		Add(new SyncGameObjectsAndEntitiesSystems(contexts));

		Add(new AttachHandheldToPlayerSystem(contexts));

		Add(new InitializeScreenViewSystem(contexts));
		Add(new UpdateScreenViewSystem(contexts));

		Add(new RemoveGameObjectOnEntityDestroySystem(contexts));
    }
}
