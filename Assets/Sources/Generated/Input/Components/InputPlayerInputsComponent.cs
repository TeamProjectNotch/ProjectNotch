//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public PlayerInputsComponent playerInputs { get { return (PlayerInputsComponent)GetComponent(InputComponentsLookup.PlayerInputs); } }
    public bool hasPlayerInputs { get { return HasComponent(InputComponentsLookup.PlayerInputs); } }

    public void AddPlayerInputs(System.Collections.Generic.List<PlayerInputRecord> newInputs) {
        var index = InputComponentsLookup.PlayerInputs;
        var component = CreateComponent<PlayerInputsComponent>(index);
        component.inputs = newInputs;
        AddComponent(index, component);
    }

    public void ReplacePlayerInputs(System.Collections.Generic.List<PlayerInputRecord> newInputs) {
        var index = InputComponentsLookup.PlayerInputs;
        var component = CreateComponent<PlayerInputsComponent>(index);
        component.inputs = newInputs;
        ReplaceComponent(index, component);
    }

    public void RemovePlayerInputs() {
        RemoveComponent(InputComponentsLookup.PlayerInputs);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherPlayerInputs;

    public static Entitas.IMatcher<InputEntity> PlayerInputs {
        get {
            if (_matcherPlayerInputs == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.PlayerInputs);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherPlayerInputs = matcher;
            }

            return _matcherPlayerInputs;
        }
    }
}
