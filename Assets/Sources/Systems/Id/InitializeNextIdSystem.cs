using Entitas;

/// Initializes the nextId counter to zero if it's not initialized already.
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
    }
}
