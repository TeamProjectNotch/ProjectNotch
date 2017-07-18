using System;
using Entitas;

public static class ContextExtensions {
	
	public static int FindIndexOfComponent(this IContext context, Type componentType) {

		var types = context.contextInfo.componentTypes;
		var index = types.IndexOf(componentType);

		if (index == -1) {

			throw new Exception(String.Format(
				"The context {0} does not have a component type {1}", 
				context,
				componentType
			));
		}

		return index;
	}

	public static int FindIndexOfComponent<TComponent>(this IContext context) 
		where TComponent : IComponent {

		return context.FindIndexOfComponent(typeof(TComponent));
	}
}

