using System;
using Entitas;

public class EnsureAllEntityIdSystem : AllEntitiesSystem<IId> {

	readonly Contexts contexts;

	// TODO Make a Unique NextId component to avoid storing it in a system.
	ulong nextId;

	public EnsureAllEntityIdSystem(Contexts contexts) : base(contexts) {}

	protected override void Execute(IId entity) {
		
		if (entity.hasId)
			nextId = Math.Max(nextId, entity.id.value + 1);
		else
			entity.AddId(nextId++);
	}
}