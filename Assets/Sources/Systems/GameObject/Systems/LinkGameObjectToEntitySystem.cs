using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;

/// Makes sure GameObjects have an EntityLink to their owning Entities. 
public class LinkGameObjectToEntitySystem : ReactiveSystem<GameEntity> {

	readonly GameContext game;
	
	public LinkGameObjectToEntitySystem(Contexts contexts) : base(contexts.game) {

		game = contexts.game;
	}

	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) {

		return context.CreateCollector(GameMatcher.GameObject.Added());
	}

	protected override bool Filter(GameEntity entity) {

		return true;
	}

	protected override void Execute(List<GameEntity> entities) {

		foreach (var e in entities) Process(e);
	}

	void Process(GameEntity e) {

		var gameObject = e.gameObject.value;

		var link = gameObject.GetEntityLink();
		if (link) {

			if (link.entity == e) return;

			gameObject.Unlink();
		}

		gameObject.Link(e, game);
	}

}
