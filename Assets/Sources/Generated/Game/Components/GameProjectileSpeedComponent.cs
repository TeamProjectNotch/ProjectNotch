//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public ProjectileSpeedComponent projectileSpeed { get { return (ProjectileSpeedComponent)GetComponent(GameComponentsLookup.ProjectileSpeed); } }
    public bool hasProjectileSpeed { get { return HasComponent(GameComponentsLookup.ProjectileSpeed); } }

    public void AddProjectileSpeed(float newValue) {
        var index = GameComponentsLookup.ProjectileSpeed;
        var component = CreateComponent<ProjectileSpeedComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceProjectileSpeed(float newValue) {
        var index = GameComponentsLookup.ProjectileSpeed;
        var component = CreateComponent<ProjectileSpeedComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveProjectileSpeed() {
        RemoveComponent(GameComponentsLookup.ProjectileSpeed);
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
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherProjectileSpeed;

    public static Entitas.IMatcher<GameEntity> ProjectileSpeed {
        get {
            if (_matcherProjectileSpeed == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ProjectileSpeed);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherProjectileSpeed = matcher;
            }

            return _matcherProjectileSpeed;
        }
    }
}
