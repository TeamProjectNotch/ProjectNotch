//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkingEntity {

    public OwnerComponent owner { get { return (OwnerComponent)GetComponent(NetworkingComponentsLookup.Owner); } }
    public bool hasOwner { get { return HasComponent(NetworkingComponentsLookup.Owner); } }

    public void AddOwner(int newValue) {
        var index = NetworkingComponentsLookup.Owner;
        var component = CreateComponent<OwnerComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceOwner(int newValue) {
        var index = NetworkingComponentsLookup.Owner;
        var component = CreateComponent<OwnerComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveOwner() {
        RemoveComponent(NetworkingComponentsLookup.Owner);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class NetworkingEntity : IOwner { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class NetworkingMatcher {

    static Entitas.IMatcher<NetworkingEntity> _matcherOwner;

    public static Entitas.IMatcher<NetworkingEntity> Owner {
        get {
            if (_matcherOwner == null) {
                var matcher = (Entitas.Matcher<NetworkingEntity>)Entitas.Matcher<NetworkingEntity>.AllOf(NetworkingComponentsLookup.Owner);
                matcher.componentNames = NetworkingComponentsLookup.componentNames;
                _matcherOwner = matcher;
            }

            return _matcherOwner;
        }
    }
}