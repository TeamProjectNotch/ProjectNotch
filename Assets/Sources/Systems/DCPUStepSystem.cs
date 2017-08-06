using System;
using Entitas;

[SystemAvailability(InstanceKind.All)]
public class DCPUStepSystem : IExecuteSystem {

    private IGroup<GameEntity> dcpuGroup;

    public DCPUStepSystem(Contexts contexts) {

        dcpuGroup = contexts.game.GetGroup(GameMatcher.DCPU);
    }

    public void Execute() {

        foreach (var e in dcpuGroup.GetEntities()) Process(e);
    }

    private void Process(GameEntity e) {

        var dcpu = e.dCPU;
        if (dcpu != null) {

            for (int cycles = 1666; cycles > 0; cycles -= 1) {

                DCPU.step(dcpu.state);
            }
        }
    }
}