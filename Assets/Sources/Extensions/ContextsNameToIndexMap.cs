using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

/// This part adds the ability to get the index of a context given its name.
public partial class Contexts {

	public Dictionary<String, int> contextNameToIndex {get; private set;}

	[PostConstructor]
	public void InitializeContextNameToIndexMap() {
		
		int numContexts = allContexts.Length;
		contextNameToIndex = new Dictionary<string, int>(numContexts);
		for (int contextIndex = 0; contextIndex < numContexts; ++contextIndex) {

			var contextName = allContexts[contextIndex].contextInfo.name;
			contextNameToIndex[contextName] = contextIndex;
		}
	}
}

