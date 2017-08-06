using System;
using Entitas;

public class CreateWeaponSystem : IInitializeSystem {

    private readonly Contexts contexts;

    public CreateWeaponSystem(Contexts context) {

        this.contexts = context;
    }

    public void Initialize() {

        var entity = contexts.game.CreateEntityWithId();
        entity.AddPrefab("Weapon");
    }
}
