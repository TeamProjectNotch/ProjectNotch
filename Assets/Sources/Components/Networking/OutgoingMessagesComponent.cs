using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Stores a queue of messages to be sent to either the clients or the server.
[Networking]
public class OutgoingMessagesComponent : IComponent {

	public Queue<INetworkMessage> queue;
}
