using Entitas;

/// Stores the priority of this Entity for networking. 
/// If the priority is high, then the changes in the Entity's state will be sent more rapidly.
/// Accumulate priority each simstep in this.accumulated.
[Game, Input, Events]
[NetworkSync(NetworkTargets.None)]
public class NetworkUpdatePriorityComponent : IComponent {

	public int basePriority;
	public int accumulated;
}
