using Entitas;

/// All the systems that are supposed to be run on the server during FixedUpdate.
public class ServerFixedTimestepSystems : Feature {
   
	public ServerFixedTimestepSystems(Contexts contexts) : base("FixedTimestep (server)") {

		Add(new EnsureGameEntityIdSystem(contexts));

		Add(new ProcessInputSystems(contexts));
        Add(new GameLogicSystems(contexts));
        Add(new GameObjectSystems(contexts));
		Add(new ServerNetworkingSystems(contexts));

		Add(new DestroySystem(contexts));

		Add(new TestScreenBufferSystem(contexts));
		Add(new TestCreateMonitorEntitySystem(contexts));
    }
}
