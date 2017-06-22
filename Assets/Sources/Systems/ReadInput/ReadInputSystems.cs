using Entitas;

/// Systems which read user input and translate that into Entities. Should execute in Update().
public class ReadInputSystems : Feature {

    public ReadInputSystems(Contexts contexts) : base("ReadInput") {

		Add(new ReadPlayerInputSystem(contexts));
    }
}
