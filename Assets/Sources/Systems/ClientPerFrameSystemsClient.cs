using System;
using Entitas;

public class ClientPerFrameSystems : Feature {
	
	public ClientPerFrameSystems(Contexts contexts) : base("PerFrame (Client)") {

		Add(new ReadInputSystems(contexts));
	}
}

