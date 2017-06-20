using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Entitas;
using UnityEngine;

public class HandleInputStateUpdateSystem : ProcessMessageSystem<InputStateUpdateMessage> {
	
	readonly InputContext input;

	public HandleInputStateUpdateSystem(Contexts contexts) : base(contexts.networking) {

		input = contexts.input;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {

		return context.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Client, NetworkingMatcher.IncomingMessages)
		);
	}

	protected override void Process(InputStateUpdateMessage message, NetworkingEntity source) {

		message.changes.Each(Apply);
		Debug.LogFormat("Num inputs applied this step: {0}", message.changes.Length);
	}

	void Apply(EntityChange change) {

		var entity = input.CreateEntity();
		change.Apply(entity);
	}
}
