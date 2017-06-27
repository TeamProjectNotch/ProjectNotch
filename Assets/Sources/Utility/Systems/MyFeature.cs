
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

// /
public class MyFeature : MyDebugSystems {

	public MyFeature(Contexts contexts, string name) : base(contexts, name) {}

	public MyFeature(Contexts contexts) : base(contexts, true) {
		
		var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
		var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
		var readableType = Entitas.Utils.StringExtension.ToSpacedCamelCase(shortType);

		initialize(readableType);
	}
}

#else

public class MyFeature : MySystems {

	public MyFeature(Contexts contexts, string name) : base(contexts) {}

	public MyFeature(Contexts contexts) : base(contexts) {}
}

#endif

