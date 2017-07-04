using System;

/// Systems for sending state updates of the GameContext to the clients over the network.
public class SendGameContextUpdatesSystems : MyFeature {

	public SendGameContextUpdatesSystems(Contexts contexts) : base("NetworkSendGameContext systems") {

		Add(new EnsureChangeFlagsInGameContextSystem(contexts));
		Add(new UpdateChangeFlagsInGameContextSystem(contexts));
		Add(new EnsureNetworkUpdatePriorityInGameContextSystem(contexts));
		Add(new IncreaseNetworkUpdatePriorityInGameContextSystem(contexts));

		Add(new ComposeGameStateUpdateMessageSystem(contexts));
	}
}

// TODO Create ComposeContextStateUpdateMessageSystem and HandleContextStateUpdateSystem which would work with arbitrary contexts.

public class SendInputContextUpdatesSystems : MyFeature {

	public SendInputContextUpdatesSystems(Contexts contexts) : base("NetworkSendInputContext systems") {

		Add(new ComposeInputStateUpdateMessageSystem(contexts));
	}
}
