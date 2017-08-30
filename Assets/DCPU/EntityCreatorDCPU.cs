using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCreatorDCPU : EntityCreator {

    public override GameEntity CreateEntity(Contexts contexts) {

        var dcpu = contexts.game.CreateEntity();
        ContextsIdExtensions.AssignId(contexts, dcpu);

        DCPUState state = new DCPUState();
        ushort[] program = { // Simple Hello World LEM1802 Program
            0x1a00, 0x84e1, 0x1e20, 0x7c12, 0xf615, 0x7f81, 0x000d, 0x88e2, 0x1cd2, 0x7f81,
            0x0011, 0x7f81, 0x0002, 0x1fc1, 0x0029, 0x7f81, 0x0007, 0x7cc1, 0x002a, 0x7ce1,
            0x8000, 0x3801, 0x8412, 0x7f81, 0x0020, 0x7c0b, 0x7000, 0x01e1, 0x88c2, 0x88e2,
            0x7f81, 0x0015, 0x7de1, 0x709f, 0x8401, 0x7c21, 0x8000, 0x7a40, 0x0029, 0x7f81,
            0x0027, 0xffff, 0x0048, 0x0045, 0x004c, 0x004c, 0x004f, 0x0020, 0x0057, 0x004f,
            0x0052, 0x004c, 0x0044, 0x0000 };

        state.loadProgram(program);

        dcpu.AddDCPU(state);

        var lem = contexts.game.CreateEntity();
        ContextsIdExtensions.AssignId(contexts, lem);
        // LEMState lstate = new LEMState();
        // lem.AddLEM(lstate);
        state.peripheralIds.Add(lem.id.value);

        return dcpu;
    }
}
