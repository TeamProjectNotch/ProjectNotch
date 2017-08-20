using System;

/// An attribute that marks the type as a network message.
/// Network message types need to be marked like this so that NetworkMessageSerializer 
/// could create efficient type ids for each such type. 
/// This attribute allows adding some debug info to a message type, like a custom name.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class NetworkMessageAttribute : Attribute {

	public string name;

	public NetworkMessageAttribute(string name = null) {

		this.name = name;
	}
}
