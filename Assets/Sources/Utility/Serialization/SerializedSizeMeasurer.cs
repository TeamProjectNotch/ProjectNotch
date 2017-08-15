
/// Pretends to be a writer IUnifiedSerializer, but actually does nothing but 
/// keep track of how much data it is asked to serialize. 
/// This can be used to find out how much data an object will take up
/// after serialization. Just call IUnifiedSerializable.Serialize(serializer)
/// with this and `measuredSize` will be the number of bytes the object takes.
/// Or use the .Measure(IUnifiedSerializable) method which does just that.
public class SerializedSizeMeasurer : IUnifiedSerializer {

    public bool isWriting => true;
    public bool isReading => false;

    public int measuredSize { get; set; } = 0;

    /// Measures the serilized size of the given object, 
    /// resetting before and after.
    public int Measure(IUnifiedSerializable obj) {

        Reset();

        obj.Serialize(this);
        int size = measuredSize;

        Reset();

        return size;
    }

    public void Reset() {
        
        measuredSize = 0;
    }

    #region IUnifiedSerialzer implementation
    // I really hope the editor won't screw up the formatting here *again*.

    public void Serialize(ref bool value) => measuredSize += sizeof(bool);
    public void Serialize(ref byte value) => measuredSize += sizeof(byte);

    public void Serialize(ref sbyte value) => measuredSize += sizeof(sbyte);
    public void Serialize(ref short value) => measuredSize += sizeof(short);
    public void Serialize(ref int value) => measuredSize += sizeof(int);
    public void Serialize(ref long value) => measuredSize += sizeof(long);
    public void Serialize(ref ushort value) => measuredSize += sizeof(ushort);
    public void Serialize(ref uint value) => measuredSize += sizeof(uint);
    public void Serialize(ref ulong value) => measuredSize += sizeof(ulong);
    public void Serialize(ref float value) => measuredSize += sizeof(float);
    public void Serialize(ref double value) => measuredSize += sizeof(double);
    public void Serialize(ref decimal value) => measuredSize += sizeof(decimal);
    public void Serialize(ref char value) => measuredSize += sizeof(char);

    public void Serialize(ref string value) {

        measuredSize += sizeof(char) * value.Length;
    }

    public void Serialize(ref byte[] value, int count) {

        measuredSize += sizeof(byte) * count;
    }

    public void Serialize(ref char[] value, int count) {

        measuredSize += sizeof(char) * count;
    }

    public void Dispose() {}
    #endregion
}
