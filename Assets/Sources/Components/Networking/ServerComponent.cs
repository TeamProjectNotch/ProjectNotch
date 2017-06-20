using System;
using Entitas;

/// Marks the entity as a server. 
/// E.G. a NetworkingEntity with a ConnectionComponent and a ServerComponent represents a connection with a server.
[Networking]
public class ServerComponent : IComponent {}
