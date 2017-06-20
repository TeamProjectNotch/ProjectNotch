using Entitas;

[System.Serializable]
public abstract class WrapperComponent<T> : IComponent {

	public T value;

	public static implicit operator T(WrapperComponent<T> component) {
		return component.value;
	}
}
