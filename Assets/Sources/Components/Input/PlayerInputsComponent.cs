using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;

/// The state of a player's input at a particular moment in time.
public struct PlayerInputRecord : IUnifiedSerializable{

	public ulong timestamp;
	public PlayerInputState inputState;

	public PlayerInputRecord(ulong timestamp, PlayerInputState inputState) {

		this.timestamp = timestamp;
		this.inputState = inputState;
	}

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref timestamp);
		s.Serialize(ref inputState);
	}
}

/// Stores the inputs of a player matched with their timesteps.
[Input]
public class PlayerInputsComponent : IComponent, IUnifiedSerializable {
	
	public List<PlayerInputRecord> inputs;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref inputs);
	}
}
