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

        UpdateCurrentTick(message, source);
        UpdateNextId     (message, source);
	}

    void UpdateCurrentTick(TickUpdateMessage message, NetworkingEntity source) {

        float messageDelay = source.hasLatency ? source.latency.ticks : 0;
        ulong messageDelayTicks = (ulong)Math.Round(messageDelay);
        ulong newCurrentTick = message.tick + messageDelayTicks;

        if (newCurrentTick >= game.currentTick.value) {
            
            Debug.Log(
                $"HandleTickUpdateSystem: " +
                $"Server is {newCurrentTick - game.currentTick.value} " +
                "ticks ahead of client"
            );
        } else {
            Debug.Log(
                $"HandleTickUpdateSystem: " +
                $"Server is {game.currentTick.value - newCurrentTick} " +
                "ticks behind client"
            );
        }

        //Debug.LogFormat("Replacing currentTick {0} with {1}.", game.currentTick.value, newCurrentTick);
        game.ReplaceCurrentTick(newCurrentTick);
    }

    void UpdateNextId(TickUpdateMessage message, NetworkingEntity source) {

        game.ReplaceNextId(message.nextId);
    }
}
