using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

public enum NetworkTargets {

	None = 0,

	Client = 1,
	Server = 2,

	All = Client | Server
}

[AttributeUsage(AttributeTargets.Class)]
public class NetworkSyncAttribute : Attribute {

	public NetworkTargets targets;

	public bool toClient {get {return (targets & NetworkTargets.Client) != 0;}}
	public bool toServer {get {return (targets & NetworkTargets.Server) != 0;}}

	public NetworkSyncAttribute(NetworkTargets targets) {

		this.targets = targets;
	}
}