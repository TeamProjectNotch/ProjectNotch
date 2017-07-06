using System;
using Entitas;
using UnityEngine;

/// The script which sets up and runs all the systems. 
/// Sets ProgramInstance.thisInstanceKind on Start based on this.thisInstanceKind.
public class GameController : MonoBehaviour {

	public InstanceKind thisInstanceKind;

	Contexts contexts;

	Systems systemsFixedUpdate;
	Systems systemsUpdate;

	void Start() {

		ProgramInstance.thisInstanceKind = thisInstanceKind;

		contexts = Contexts.sharedInstance;
		systemsFixedUpdate = new OnFixedUpdateSystems(contexts);
		systemsUpdate = new OnUpdateSystems(contexts);

		systemsFixedUpdate.Initialize();
		systemsUpdate.Initialize();
	}

	void FixedUpdate() {

		systemsFixedUpdate.Execute();
		systemsFixedUpdate.Cleanup();
	}

	void Update() {

		systemsUpdate.Execute();
		systemsUpdate.Cleanup();
	}

	void OnDestroy() {

		systemsFixedUpdate.TearDown();
		systemsUpdate.TearDown();
	}
}
