using System;
using UnityEngine;

public interface ICharacterBehaviour {

	void SimulateStep(PlayerInputState inputState);
}

/// A facade for all the scripts that do character movement simulation.
public class Character : MonoBehaviour {

	ICharacterBehaviour[] characterBehaviours;

	void Start() {

		characterBehaviours = GetComponentsInChildren<ICharacterBehaviour>(includeInactive: true);
	}
	
	public void SimulateStep(PlayerInputState inputState = new PlayerInputState()) {
		
		foreach (var behaviour in characterBehaviours) {

			behaviour.SimulateStep(inputState: inputState);
		}
	}
}
