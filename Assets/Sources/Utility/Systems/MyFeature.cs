
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

/// Just like Feature, but uses MySystems and MyDebugSystems instead of Systems and DebugSystems.
public class MyFeature : MyDebugSystems {

	public MyFeature(string name) : base(name) {}

	public MyFeature() : base(noInit: true) {
		
		var typeName = Entitas.Utils.TypeSerializationExtension.ToCompilableString(GetType());
		var shortType = Entitas.Utils.TypeSerializationExtension.ShortTypeName(typeName);
		var readableType = Entitas.Utils.StringExtension.ToSpacedCamelCase(shortType);

		initialize(readableType);
	}
}

#else

public class MyFeature : MySystems {

	public MyFeature(string name) : base() {}

	public MyFeature() : base() {}
}

#endif

