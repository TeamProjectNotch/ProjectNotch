using System;
using Entitas;

/// Indicates that the Transform and RigibodyState of the Entity depend on Unity's physics simulation.
[Game]
public class GameObjectDrivenComponent : StatelessUnifiedSerializable, IComponent {}
