using Entitas;
using UnityEngine;

/// Stores a reference to the GameObject which represents the Entity.
[Game]
[NetworkSync(NetworkTargets.None)]
public class GameObjectComponent : WrapperComponent<GameObject> {}
