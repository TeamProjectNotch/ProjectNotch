using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.Unity;

/// Updates the LookDirection of the linked entity to the direction the found camera is looking.
public class UpdaterLookDirection : MonoBehaviour {

	EntityLink _entityLink;
	EntityLink entityLink {
		get {
			
			return _entityLink ? _entityLink : _entityLink = gameObject.GetEntityLink();
		}
	}

	Transform cameraTransform;

	void Start() {
		
		cameraTransform = GetComponentInChildren<Camera>().transform;
	}
	
	void Update () {

		if (!entityLink) return;

		var entity = entityLink.entity as GameEntity;
		Debug.Assert(entity != null, this);

		entity.ReplaceLookDirection(cameraTransform.forward);
	}
}
