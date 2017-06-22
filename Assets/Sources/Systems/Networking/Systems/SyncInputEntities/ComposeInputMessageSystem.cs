using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Entitas;

/// Composes and enqueues messages with input data to send to the server.
/// Just sends all the entities in the InputContext.
/// WARNING: only sends entity and component updates, not removals.
public class ComposeInputMessageSystem : IExecuteSystem {
	
	readonly NetworkingContext networking;
	readonly IGroup<NetworkingEntity> servers;

	readonly InputContext input;

	public ComposeInputMessageSystem(Contexts contexts) {

		networking = contexts.networking;
		servers = networking.GetGroup(
			NetworkingMatcher.AllOf(NetworkingMatcher.Server, NetworkingMatcher.OutgoingMessages)
		);

		input = contexts.input;
	}

	public void Execute() {

		var server = servers.GetSingleEntity();
		if (server == null) return;

		INetworkMessage message = MakeMessage();
		Enqueue(message, server);
	}

	INetworkMessage MakeMessage() {

		return new InputStateUpdateMessage(GetMessageChanges());
	}

	EntityChange[] GetMessageChanges() {
		
		var entities = input.GetEntities();
		var changes = new EntityChange[entities.Length];

		int i = 0;
		foreach (var e in entities) {

			changes[i++] = MakeChangeOf(e);
		}

		return changes;
	}

	EntityChange MakeChangeOf(InputEntity e) {

		int[] indices = e.GetComponentIndices();
		var components = e.GetComponents();

		int numComponents = components.Length;
		var componentChanges = new ComponentChange[numComponents];
		for (int i = 0; i < numComponents; ++i) {

			componentChanges[i] = ComponentChange.MakeUpdate(
				indices[i], 
				components[i]
			);
		}
			
		return EntityChange.MakeUpdate(e.id.value, componentChanges);
	}

	void Enqueue(INetworkMessage message, NetworkingEntity server) {

		var queue = server.outgoingMessages.queue;
		queue.Enqueue(message);
		server.ReplaceOutgoingMessages(queue);
	}
}
