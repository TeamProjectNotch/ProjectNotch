using Entitas;

/// Links the Entity to a player with a player id.
[Game, Input]
public class PlayerComponent : IComponent, IUnifiedSerializable {

	public int id;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref id);
	}
}
