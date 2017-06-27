using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[GameState]
[Unique]
public class ProgramInstanceKindComponent : WrapperComponent<InstanceKind> {}
