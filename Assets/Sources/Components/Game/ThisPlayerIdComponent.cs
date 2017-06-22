using Entitas;
using Entitas.CodeGeneration.Attributes;

/// The id of the player of this client. Should exist on a client.
[Game]
[Unique]
public class ThisPlayerIdComponent : WrapperComponent<int> {}
