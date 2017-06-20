using System.Collections.Generic;
using Entitas;

/// Stores a queue of messages received over the network.
[Networking]
public class IncomingMessagesComponent : IComponent {

	public Queue<INetworkMessage> queue;
}

