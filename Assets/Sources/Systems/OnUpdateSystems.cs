using System;

/// Systems that execute once per frame
public class OnUpdateSystems : MyFeature {

	public OnUpdateSystems(Contexts contexts) : base("On Update systems") {

		Add(new ReadInputSystems(contexts));
	}
}
