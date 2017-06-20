using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;

/// Stores the inputs of a player matched with their timesteps.
[Input]
public class PlayerInputsComponent : IComponent {
	
	public List<KeyValuePair<int, PlayerInputState>> inputs;
}
