using System;
using Entitas;

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
		var currentTick = message.tick + messageDelayTicks;

		game.ReplaceCurrentTick(currentTick);
	}
}
