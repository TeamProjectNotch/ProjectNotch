using System;
using Entitas;

/// Marks the Entity as a client. 
/// E.G. a NetworkingEntity with a ConnectionComponent and a ClientComponent represents a connection with a client.
[Networking]
public class ClientComponent : IComponent {}
