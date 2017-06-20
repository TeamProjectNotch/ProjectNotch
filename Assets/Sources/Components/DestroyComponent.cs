using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Flags the entity for destruction during the cleanup stage of systems' execution.
[Game, Input, Events]
[CustomPrefix("flag")]
public class DestroyComponent : IComponent {}
