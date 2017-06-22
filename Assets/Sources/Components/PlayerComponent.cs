using Entitas;
using Entitas.CodeGeneration.Attributes;

/// The Entity represents a player with an id. Specifically:
/// GameEntity with this represents a player entity.
/// InputEntity with this represents the input-related part of a player.
/// NetworkingEntity with this represents a connection with a player.
[Game, Input, Networking]
public class PlayerComponent : IComponent, IUnifiedSerializable {

	[PrimaryEntityIndex]
	public int id;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref id);
	}
}
