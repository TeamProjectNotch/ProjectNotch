using System;
using Entitas;

/// Ensures that all entities have an IdComponent.
public class EnsureIdSystem : AllEntitiesSystem<IId> {

	readonly GameContext game;

	public EnsureIdSystem(Contexts contexts) : base(contexts) {

        game = contexts.game;
	}

	public override void Initialize() {

		if (!game.hasNextId) {

			game.SetNextId(0ul);
		}

        if (ProgramInstance.isServer) {

            // To make sure game.nextId is included in every message.
            game.nextIdEntity.ReplaceNetworkUpdatePriority(int.MaxValue, 0);
        }

		base.Initialize();
	}

	protected override void Apply(IId entity) {

        if (!entity.hasId) {

            var newId = game.nextId.value;
            entity.AddId(newId);
            game.ReplaceNextId(newId + 1ul);
        }
	}
}