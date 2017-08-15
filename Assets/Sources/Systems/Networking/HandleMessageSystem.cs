using System;
using Entitas;
using UnityEngine;

/// An abstract system to inherit from if you want to create a system for handling messages.
/// Similar to ReactiveSystem. Processes one TMessage at top of the message queue.
public abstract class HandleMessageSystem<TMessage> : IExecuteSystem 
	where TMessage : class, INetworkMessage {

	readonly IGroup<NetworkingEntity> messageSources;

	protected HandleMessageSystem(IContext<NetworkingEntity> context) {

		messageSources = GetMessageSources(context);
	}

	public void Execute() {

		foreach (var source in messageSources.GetEntities()) {
			TryProcessMessageFrom(source);
		}
	}

	void TryProcessMessageFrom(NetworkingEntity messageSource) {

		if (!messageSource.hasIncomingMessages) return;

		var queue = messageSource.incomingMessages.queue;
		if (queue.Count == 0) return;

		var message = queue.Peek() as TMessage;
		if (message != null) {

			queue.Dequeue();
			messageSource.ReplaceIncomingMessages(queue);

			Process(message, messageSource);
		}
	}

	protected abstract IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context);

	protected abstract void Process(TMessage message, NetworkingEntity source);
}
