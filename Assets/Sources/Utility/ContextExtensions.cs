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

	/// A moderately hackish way to get all entities in any context.
	public static TEntity[] GetEntities<TEntity>(this IContext context) where TEntity : IEntity {

		var methodInfo = context.GetType().GetMethod("GetEntities");
		var result = methodInfo.Invoke(context, parameters: null);

		return (TEntity[])result;
	}

	public static IEntity CreateEntity(this IContext context) {

		var methodInfo = context.GetType().GetMethod("CreateEntity");
		var result = methodInfo.Invoke(context, parameters: null);

		return (IEntity)result;
	}
}

