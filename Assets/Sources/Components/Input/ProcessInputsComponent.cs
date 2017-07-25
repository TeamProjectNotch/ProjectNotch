using Entitas;

/// A command component.
/// Some inputs stored in PlayerInputsComponent need to be processed.
/// Only those inputs need to be processed which were recorded after or during `startTick`.
[Input]
[NetworkSyncAttribute(NetworkTargets.Server)]
public class ProcessInputsComponent : IComponent {

	public ulong startTick;
}
