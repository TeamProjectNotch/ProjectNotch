using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

public enum NetworkTargets {

	None = 0,

	Server = 1,
	Client = 2,

	All = Client | Server
}

[AttributeUsage(AttributeTargets.Class)]
public class NetworkSyncAttribute : Attribute {

	public NetworkTargets targets;

    public bool toClient => (targets & NetworkTargets.Client) != 0;
    public bool toServer => (targets & NetworkTargets.Server) != 0;

	public NetworkSyncAttribute(NetworkTargets targets) {

		this.targets = targets;
	}
}