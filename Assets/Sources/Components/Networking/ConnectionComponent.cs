using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// Marks the Entity as a connection. 
/// Stores the id of a Unity Transport Layer connection.
[Networking]
public class ConnectionComponent : IComponent {

	[PrimaryEntityIndex]
	public int id;
}
