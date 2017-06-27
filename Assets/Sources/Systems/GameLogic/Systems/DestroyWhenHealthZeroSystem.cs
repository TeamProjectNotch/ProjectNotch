using System.Collections.Generic;
using Entitas;
using System.Linq;

[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class DestroyWhenHealthZeroSystem : ReactiveSystem<GameEntity> {
	
    public DestroyWhenHealthZeroSystem(Contexts contexts) : base(contexts.game) {}

    protected override void Execute(List<GameEntity> entities) {
		
		entities.Where(e => e.health.health <= 0f).Each(e => e.flagDestroy = true);
    }

    protected override bool Filter(GameEntity entity) {
		
		return entity.hasHealth;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {
		
        return context.CreateCollector(GameMatcher.Health.Added());
    }
}
