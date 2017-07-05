using System;
using UnityEngine.Networking;

/// Helper class to work with the config of connections between clients and servers.
public static class ClientServerConnectionConfig {

	public static readonly ConnectionConfig config;

	public static readonly byte reliableFragmentedChannelId;
	public static readonly byte unreliableFragmentedChannelId;

	static ClientServerConnectionConfig() {
		
		config = new ConnectionConfig();
		reliableFragmentedChannelId = config.AddChannel(QosType.ReliableFragmented);
		unreliableFragmentedChannelId = config.AddChannel(QosType.UnreliableFragmented);
	}
}
