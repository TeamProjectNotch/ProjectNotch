using Entitas;

[Game]
public class HealthComponent : IComponent, IUnifiedSerializable {

    public float health;
    public float maxHealth;

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref health);
		s.Serialize(ref maxHealth);
	}
}
