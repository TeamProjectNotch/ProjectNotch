using System;

/// The different kinds of thing the program instance can be. E.G. server, client etc.
/// Also a bitmask of things a system may be available on. 
/// So `InstanceKind.Server | InstanceKind.Singleplayer` means a system
/// is available on a server or a singleplayer instance, but not a client one.
[Flags]
public enum InstanceKind {

	None = 0,

	Server = 1,
	Client = 2,
	Singleplayer = 4,

	// Shortcuts
	ServerAndSingleplayer = Server | Singleplayer,
	ClientAndSingleplayer = Client | Singleplayer,
	/// Server | Client
	Networked = Server | Client,

	All = Server | Client | Singleplayer
}

/// Stores the InstanceKind of the current program instance.
public static class ProgramInstance {

	static InstanceKind? _thisInstanceKind;
	public static InstanceKind thisInstanceKind {
		get {

			if (!_thisInstanceKind.HasValue) {

				throw new Exception("ProgramInstance.thisInstanceKind was not set!");
			}

			return _thisInstanceKind.Value;
		}
		set {

			if (_thisInstanceKind.HasValue) {

				throw new Exception("ProgramInstance.thisInstanceKind was already set!");
			}

			_thisInstanceKind = value;
		}
	}

	public static bool isClient {
		get {return (thisInstanceKind & InstanceKind.Client) != 0;} 
	}

	public static bool isServer {
		get {return (thisInstanceKind & InstanceKind.Server) != 0;} 
	}

	public static bool isClientOrServer {

		get {return (thisInstanceKind & InstanceKind.Networked) != 0;}
	}
}
