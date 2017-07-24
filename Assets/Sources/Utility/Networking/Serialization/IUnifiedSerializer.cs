using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// Either writes the given values to a stream, 
/// or reads data into the given values from that stream.
public interface IUnifiedSerializer : IDisposable {

	bool isWriting {get;}
	bool isReading {get;}

	void Serialize(ref bool    value);
	void Serialize(ref byte    value);
	void Serialize(ref sbyte   value);
	void Serialize(ref short   value);
	void Serialize(ref int     value);
	void Serialize(ref long    value);
	void Serialize(ref ushort  value);
	void Serialize(ref uint    value);
	void Serialize(ref ulong   value);
	void Serialize(ref float   value);
	void Serialize(ref double  value);
	void Serialize(ref decimal value);
	void Serialize(ref char    value);
	void Serialize(ref string  value);

	void Serialize(ref byte[]  value, int count);
	void Serialize(ref char[]  value, int count);
}
