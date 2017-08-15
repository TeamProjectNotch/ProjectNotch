using System.Collections.Generic;
using Entitas;

/// When an entity is flagged for destruction, but there is not enough space 
/// left in this tick's state update message, an EntityChange for that entity
/// is created but not sent, but stored here to be included in a later message.
[Networking]
public class DestroyedEntitiesBacklogComponent : IComponent {

    public HashSet<EntityChange> entityChanges;
}