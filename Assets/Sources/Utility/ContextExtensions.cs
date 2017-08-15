using System;
using System.Linq;
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

	/// A hackish way to get all entities in any context.
	public static TEntity[] GetEntities<TEntity>(this IContext context) where TEntity : IEntity {

		var methodInfo = context.GetType().GetMethod("GetEntities");
		var result = methodInfo.Invoke(context, parameters: null);

		return (TEntity[])result;
	}

    /// A hackish way to create an entity in any context.
    public static IEntity CreateEntity(this IContext context) {

        var methodInfo = context.GetType().GetMethod("CreateEntity");
        var result = methodInfo.Invoke(context, parameters: null);

        return (IEntity)result;
    }

    /// GameContext -> GameEntity, InputContext -> InputEntity etc.
	public static Type GetEntityType(this IContext context) {

        // IContext<GameEntity>, IContext<InputEntity> etc.
        var interfaceType = context
            .GetType()
            .GetInterfaces()
            .Where(type => type.IsGenericType)
            .Where(type => type.GetGenericTypeDefinition() == typeof(IContext<>))
            .SingleOrDefault();

        if (interfaceType == null) {
            throw new ArgumentException(String.Format("Given context {0} does not implement IContext<>!", context));
        }

        // IContext<GameEntity> -> GameEntity
        // IContext<InputEntity> -> InputEntity
        // IA<B> -> B
        return interfaceType.GetGenericArguments()[0];
    }

    public static bool EntityIs(this IContext context, Type type) {

        return type.IsAssignableFrom(context.GetEntityType());
    }

    public static bool EntityIs<T>(this IContext context) {
        
        return context.EntityIs(typeof(T));
    }
}

