using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Creates an Entity from its GameObject.
public abstract class EntityCreator : MonoBehaviour {

	public abstract GameEntity CreateEntity(Contexts contexts);
}
