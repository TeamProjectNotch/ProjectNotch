using Entitas;

/// Systems which read user input and translate that into Entities. Should execute in Update().
public class ReadInputSystems : Feature {

    public ReadInputSystems(Contexts contexts) : base("ReadInput") {

		Add(new EmitPressFireSystem(contexts));
		Add(new EmitMoveInputSystem(contexts));
		Add(new EmitMouseMoveSystem(contexts));
		Add(new EmitJumpSystem(contexts));
    }
}
