using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// A context-global component that holds the data needed for networking, like host and channel ids.
/// Connection id's of connections with clients or the server are in their respective Entities.
[Networking, Unique]
public class IdsComponent : IComponent {

	public int host = 0;
	public int channelReliableFragmented, channelUnreliableFragmented;
}
