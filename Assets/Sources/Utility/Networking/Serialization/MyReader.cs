using System;
using System.IO;

public class MyReader : IUnifiedSerializer {

	BinaryReader reader;

	public bool isWriting {get{return false;}}

	public Stream BaseStream {get {return reader.BaseStream;}}

	public MyReader(Stream stream) {

		reader = new BinaryReader(stream);
	}

	public void Serialize(ref bool    value) {value = reader.ReadBoolean();}
	public void Serialize(ref byte    value) {value = reader.ReadByte();   }
	public void Serialize(ref sbyte   value) {value = reader.ReadSByte();  }
	public void Serialize(ref short   value) {value = reader.ReadInt16();  }
	public void Serialize(ref int     value) {value = reader.ReadInt32();  }
	public void Serialize(ref long    value) {value = reader.ReadInt64();  }
	public void Serialize(ref ushort  value) {value = reader.ReadUInt16(); }
	public void Serialize(ref uint    value) {value = reader.ReadUInt32(); }
	public void Serialize(ref ulong   value) {value = reader.ReadUInt64(); }
	public void Serialize(ref float   value) {value = reader.ReadSingle(); }
	public void Serialize(ref double  value) {value = reader.ReadDouble(); }
	public void Serialize(ref decimal value) {value = reader.ReadDecimal();}
	public void Serialize(ref char    value) {value = reader.ReadChar();   }
	public void Serialize(ref string  value) {value = reader.ReadString(); }

	public void Serialize(ref byte[]  value, int count) {value = reader.ReadBytes(count);}
	public void Serialize(ref char[]  value, int count) {value = reader.ReadChars(count);}

	public void Dispose() {

		((IDisposable)reader).Dispose();
	}
}
