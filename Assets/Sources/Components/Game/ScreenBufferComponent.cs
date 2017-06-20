using Entitas;

/// Stores the pixel data of a screen buffer. Packs the data 4-bits per color index.
public struct ScreenBufferState {

	public readonly Vector2Int size;
	public uint[] colorIndicesPacked;

	public ScreenBufferState(Vector2Int size) {

		this.size = size;
		colorIndicesPacked = BitPacker.MakeStorageFor4BitValues(size.x * size.y);
	}

	public void SetColorCodeAt(int x, int y, ColorCode colorCode) {

		BitPacker.Pack((byte)colorCode, IndexAt(x, y), colorIndicesPacked);
	}

	public ColorCode GetColorCodeAt(int x, int y) {

		return (ColorCode)BitPacker.Unpack(IndexAt(x, y), colorIndicesPacked);
	}

	int IndexAt(int x, int y) {
		
		return y * size.x + x;
	}
}

/// Indicates that entity has a ScreenBuffer that stores the color of each pixel.
[Game]
[Entitas.VisualDebugging.Unity.DontDrawComponent]
public class ScreenBufferComponent : IComponent {

	public ScreenBufferState data;
}
