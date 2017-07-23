using System;

public class SendStateUpdatesSystems : MyFeature {

	public SendStateUpdatesSystems(Contexts contexts) : base("SendStateUpdates systems") {
		
		Add(new EnsureChangeFlagsSystem(contexts));
		Add(new UpdateChangeFlagsSystem(contexts));

		Add(new EnsureNetworkUpdatePrioritySystem(contexts));
		Add(new IncreaseNetworkUpdatePrioritySystem(contexts));

		Add(new ComposeStateUpdateMessageSystem(contexts));
	}
}
