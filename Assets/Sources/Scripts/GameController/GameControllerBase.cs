using System;
using UnityEngine;
using Entitas;

/// The base class all possible GameControllers (Server/Client/Unified) should inherit from.
public abstract class GameControllerBase : MonoBehaviour {
	
	Contexts contexts;

	protected Systems systemsFixedUpdate;
	protected Systems systemsUpdate;

	void Start() {

		contexts = Contexts.sharedInstance;
		CreateSystems(contexts);

		if (systemsFixedUpdate == null) {
			throw new NullReferenceException("systemsFixedUpdate is null! It should've been set in CreateSystems.");
		}

		if (systemsUpdate == null) {
			throw new NullReferenceException("systemsUpdate is null! It should've been set in CreateSystems.");
		}

		systemsFixedUpdate.Initialize();
		systemsUpdate.Initialize();
	}

	protected abstract void CreateSystems(Contexts contexts);

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
