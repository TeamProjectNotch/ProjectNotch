using System;
using Entitas;

/// Server -> Client
/// Stores the tick number that was current on the server at time of sending.
[NetworkMessage]
public class TickUpdateMessage : INetworkMessage {

	public ulong tick;

	public TickUpdateMessage() {}

	public TickUpdateMessage(ulong tick) {

		this.tick = tick;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref tick);
	}
}
