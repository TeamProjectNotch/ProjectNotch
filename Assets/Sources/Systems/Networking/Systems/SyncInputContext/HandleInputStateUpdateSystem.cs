using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Entitas;
using UnityEngine;

[SystemAvailability(InstanceKind.Server)]
public class HandleInputStateUpdateSystem : HandleMessageSystem<InputStateUpdateMessage> {
	
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
		Debug.LogFormat("Num inputs received this step: {0}", message.changes.Length);
	}

	void Apply(EntityChange change) {
		
		var e = input.GetEntityWithId(change.entityId);
		if (e == null) {

			if (change.isRemoval) {

				UnityEngine.Debug.Log("Can't apply an EntityChange, since it's Entity doesn't exist.");
				return;
			}

			Debug.LogFormat("Entity with id {0} not found. Creating...", change.entityId);
			e = input.CreateEntity();
		}

		change.Apply(e);
	}
}
