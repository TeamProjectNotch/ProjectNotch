using System;
using Entitas;

public class CreateWeaponSystem : IInitializeSystem
{
    private readonly Contexts context;

    public CreateWeaponSystem(Contexts context)
    {
        this.context = context;
    }

    public void Initialize()
    {
        var entity = context.game.CreateEntity();
        entity.AddPrefab("Weapon");
    }
}
