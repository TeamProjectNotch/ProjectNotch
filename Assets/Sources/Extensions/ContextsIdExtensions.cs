
public static class ContextsIdExtensions {

    /// This is a shortcut for contexts.game.AssignId(entity). 
    /// In the future the nextId counter may get moved to a different context, say gameState. 
    /// By using this you will only have to change this method and not all the code that uses it.
    public static ulong AssignId(this Contexts contexts, IId entity) {

        return contexts.game.AssignId(entity);
    }
}
