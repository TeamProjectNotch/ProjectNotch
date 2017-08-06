using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// A context-global component which stores the id of the host/socket from Unity's Transport Layer.
[Networking, Unique]
public class HostComponent : IComponent {

	public int id;
}
