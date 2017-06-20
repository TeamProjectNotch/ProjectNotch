using System;
using Entitas;
using fNbt;

public interface INbtSerializableComponent : IComponent {
	
	NbtCompound Serialize(NbtCompound compound);
	void Deserialize(NbtCompound compound);
}
