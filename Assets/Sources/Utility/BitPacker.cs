using System;

/// Helps efficiently pack less-than-one-byte values into arrays of regular-sized numbers.
public static class BitPacker {

	/// Constant Masks used to reduce computation.
	static readonly uint[] masks = new uint[] {
		0xFFFFFFF0, 
		0xFFFFFF0F, 
		0xFFFFF0FF, 
		0xFFFF0FFF,
		0xFFF0FFFF, 
		0xFF0FFFFF, 
		0xF0FFFFFF,
		0x0FFFFFFF
	};

	const int numValuesPerArrayElement = 8;
	const int numBitsPerValue = 4;

	public static uint[] MakeStorageFor4BitValues(int numValues) {

		// Clever rounding up.
		int numUints = (numValues - 1) / numValuesPerArrayElement + 1;

		var array = new uint[numUints];
		return array;
	}
	 
	/// Packs a 4-bit value 8 values per uint into an array of uints.
	/// Treats the uint array as if it were an array of 4-bit values with 8 times more elements.
	public static void Pack(byte value, int valueIndex, uint[] array) {

		// There are 8 4-bit segments in each uint.
		int arrayIndex = valueIndex / numValuesPerArrayElement;
		int segmentNum = valueIndex % numValuesPerArrayElement;

		/* A more readable version with possible overhead.
		uint newArrayValue = array[arrayIndex];
		newArrayValue &= masks[segmentNum];
		newArrayValue |= (uint)value << (segmentNum * numBitsPerValue);
		array[arrayIndex] = newArrayValue;
		*/

		array[arrayIndex] = 
			(array[arrayIndex] & masks[segmentNum]) | 
			((uint)value << (segmentNum * numBitsPerValue));
	}

	/// Gets a 4-bit value from the array of packed uints.
	public static byte Unpack(int valueIndex, uint[] array) {

		/* A more readable version with possible overhead.
		int arrayIndex = valueIndex / numValuesPerArrayElement;
		int segmentNum = valueIndex % numValuesPerArrayElement;

		return (byte)(array[arrayIndex] >> segmentNum * numBitsPerValue);
		*/

		return (byte)(
			array[valueIndex / numValuesPerArrayElement] >> 
			((valueIndex % numValuesPerArrayElement) * numBitsPerValue));
	}
}

