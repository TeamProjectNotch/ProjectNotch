using Entitas;

[Game]
public class DamageComponent : WrapperComponent<float> {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}

[Game]
public class FireRateComponent : WrapperComponent<float> {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}

[Game]
public class ProjectileSpeedComponent : WrapperComponent<float> {

	public void Serialize<T>(T s) where T : IUnifiedSerializer {

		s.Serialize(ref value);
	}
}
