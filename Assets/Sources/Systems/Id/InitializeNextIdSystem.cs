using Entitas;

/// Initializes the nextId counter to zero if it's not initialized already.
/// Gives the nextId counter maximum network update priority when on server.
[SystemAvailability(InstanceKind.All)]
public class InitializeNextIdSystem : IInitializeSystem {

    readonly GameContext game;

    public InitializeNextIdSystem(Contexts contexts) {
        
        game = contexts.game;
    }

    public void Initialize() {

        if (!game.hasNextId) {

            game.SetNextId(0ul);
        }

        if (ProgramInstance.isServer) {

            // To make sure game.nextId is included in every message.
            game.nextIdEntity.ReplaceNetworkUpdatePriority(int.MaxValue, 0);
        }
    }
}
