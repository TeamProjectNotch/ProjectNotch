using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lem1802 : Peripheral {

    private ushort screen_Reg  = 0;
    private ushort palette_Reg = 0;
    private ushort font_Reg    = 0;


    public Lem1802() : base(0x7349f615, 0x1802, 0x1c6c8b36 ) {}

    public override void sendInterrupt(dcpuState state){
        switch(state.registers[DCPU.A_REG]){
            case 0:
                screen_Reg = state.registers[DCPU.B_REG];
                break;
        }
    }

    public override void step(){
    }
}
