using Entitas;
using UnityEngine;

/// Stores a reference to the GameObject which represents the Entity.
[Game]
[NetworkSync(NetworkSyncTargets.None)]
public class GameObjectComponent : WrapperComponent<GameObject> {}
