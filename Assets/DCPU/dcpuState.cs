﻿using System.Collections.Generic;
using System.IO;
public class dcpuState{
    public ushort[] memory;
    public ushort[] registers;
    public ushort PC, EX, SP, IA;
    public long ticks;
    //interrupt crap
    public bool canQueue;
    public ushort[] IQ;
    public int iqIndex;
    //
    public List<Peripheral> peripherals = new List<Peripheral>();

    public dcpuState(){
        memory    = new ushort[DCPU.MEM_SIZE];
        registers = new ushort[DCPU.REG_SIZE];
        canQueue = false;
        IQ = new ushort[256];
        iqIndex = 0;
        PC = 0;
        EX = 0;
        SP = 0;
        IA = 0;
    }

    public void loadProgram(ushort[] program){
        for(int i = 0; i < program.Length; i++){
            memory[i] = program[i];
        }
    }

    public dcpuState copy(){
        dcpuState newState = new dcpuState();
        for(int i = 0; i < DCPU.MEM_SIZE; i++){
            newState.memory[i] = memory[i];
        }

        for(int i = 0; i < DCPU.REG_SIZE; i++){
            newState.registers[i] = registers[i];
        }
        newState.PC = PC;
        newState.EX = EX;
        newState.SP = SP;
        newState.IA = IA;
        newState.ticks = ticks;

        return newState;
    }
}
