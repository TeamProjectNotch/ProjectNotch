using System;

/// A message to be sent over the network. Must have a public argument-less constctuctor (new()).
public interface INetworkMessage : IUnifiedSerializable {}
