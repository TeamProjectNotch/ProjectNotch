using System;

public class EntityCreatorWeapon : EntityCreator
{
    public float Damage = -1f;
    public float FireRate = -1f;
    public float ProjectileSpeed = -1f;

    public override GameEntity CreateEntity(Contexts contexts)
    {
        var e = contexts.game.CreateEntity();

        if(Damage > 0)
        {
            e.AddDamage(Damage);
        }
        if(FireRate > 0)
        {
            e.AddFireRate(FireRate);
        }
        if(ProjectileSpeed > 0)
        {
            e.AddProjectileSpeed(ProjectileSpeed);
        }
        e.AddTransform(transform.GetState());

        return e;
    }
}
