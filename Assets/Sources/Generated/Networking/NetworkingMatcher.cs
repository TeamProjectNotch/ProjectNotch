//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class NetworkingMatcher {

    public static Entitas.IAllOfMatcher<NetworkingEntity> AllOf(params int[] indices) {
        return Entitas.Matcher<NetworkingEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<NetworkingEntity> AllOf(params Entitas.IMatcher<NetworkingEntity>[] matchers) {
          return Entitas.Matcher<NetworkingEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<NetworkingEntity> AnyOf(params int[] indices) {
          return Entitas.Matcher<NetworkingEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<NetworkingEntity> AnyOf(params Entitas.IMatcher<NetworkingEntity>[] matchers) {
          return Entitas.Matcher<NetworkingEntity>.AnyOf(matchers);
    }
}
