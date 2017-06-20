using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Server-side component.
/// The next free unique id to assign to a newly created player.
[Networking]
[Unique]
public class NextPlayerIdComponent : WrapperComponent<int> {}
