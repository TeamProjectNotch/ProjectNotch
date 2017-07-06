using System;
using Entitas;
using UnityEngine;

using NM = NetworkingMatcher;

[SystemAvailability(InstanceKind.Client)]
public class HandleTickUpdateSystem : HandleMessageSystem<TickUpdateMessage> {

	readonly GameContext game;

	public HandleTickUpdateSystem(Contexts contexts) : base(contexts.networking) {

		game = contexts.game;
	}

	protected override IGroup<NetworkingEntity> GetMessageSources(IContext<NetworkingEntity> context) {

		return context.GetGroup(
			NM.AllOf(NM.Server, NM.Connection, NM.IncomingMessages)
		);
	}

	protected override void Process(TickUpdateMessage message, NetworkingEntity source) {

		var messageDelay = source.hasLatency ? source.latency.ticks : 0;
		var messageDelayTicks = (ulong)Math.Round(messageDelay);
		var newCurrentTick = message.tick + messageDelayTicks;

		if (game.currentTick.value <= newCurrentTick) {
			Debug.LogFormat("Server is {0} ticks ahead of client", newCurrentTick - game.currentTick.value);
		} else {
			Debug.LogFormat("Server is {0} ticks behind client", game.currentTick.value - newCurrentTick);
		}
		//Debug.LogFormat("Replacing currentTick {0} with {1}.", game.currentTick.value, newCurrentTick);

		game.ReplaceCurrentTick(newCurrentTick);
	}
}
