using System;
using Entitas;

/// Server -> Client
/// Stores the server's current tick, the nextId counter.
[NetworkMessage]
public class TickUpdateMessage : INetworkMessage {

	public ulong tick;
    public ulong nextId;

    public TickUpdateMessage() {}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref tick);
	}

    public override string ToString() {

        return $"TickUpdateMessage(tick: {tick} nextId: {nextId})";
    }
}
