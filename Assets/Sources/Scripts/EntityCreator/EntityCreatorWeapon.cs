using System;

public class EntityCreatorWeapon : EntityCreator {

    public float damage = -1f;
    public float fireRate = -1f;
    public float projectileSpeed = -1f;

    public override GameEntity CreateEntity(Contexts contexts) {

        var e = contexts.game.CreateEntity();
        contexts.AssignId(e);

        if (damage > 0) {

            e.AddDamage(damage);
        }

        if (fireRate > 0) {

            e.AddFireRate(fireRate);
        }

        if (projectileSpeed > 0) {

            e.AddProjectileSpeed(projectileSpeed);
        }

        e.AddTransform(transform.GetState());

        return e;
    }
}
