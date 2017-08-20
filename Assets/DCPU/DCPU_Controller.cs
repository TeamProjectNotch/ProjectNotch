using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class DCPU_Controller : MonoBehaviour {
    public DCPUState state;
    private bool running = false;
    private Thread thread;

    public List<Peripheral> peripherals = new List<Peripheral>();

    private void Start(){
        state = new DCPUState();

        for(int i = 0; i < peripherals.Count; i++){
            peripherals[i].dcpuController = this;
        }

        state.peripherals = peripherals;
        ushort[] program = { // Simple Hello World LEM1802 Program
            0x1a00, 0x84e1, 0x1e20, 0x7c12, 0xf615, 0x7f81, 0x000d, 0x88e2, 0x1cd2, 0x7f81,
            0x0011, 0x7f81, 0x0002, 0x1fc1, 0x0029, 0x7f81, 0x0007, 0x7cc1, 0x002a, 0x7ce1,
            0x8000, 0x3801, 0x8412, 0x7f81, 0x0020, 0x7c0b, 0x7000, 0x01e1, 0x88c2, 0x88e2,
            0x7f81, 0x0015, 0x7de1, 0x709f, 0x8401, 0x7c21, 0x8000, 0x7a40, 0x0029, 0x7f81,
            0x0027, 0xffff, 0x0048, 0x0045, 0x004c, 0x004c, 0x004f, 0x0020, 0x0057, 0x004f,
            0x0052, 0x004c, 0x0044, 0x0000 };

        state.loadProgram(program);
        
    }


    public void StartDCPU(){
        running = true;
        thread = new Thread(Run);
        thread.Start();
    }

    private void OnDestroy(){
        running = false;
        if(thread != null){
            thread.Join();
        }
    }


    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space) && !running){
            StartDCPU();
        }

        if(Input.GetKeyDown(KeyCode.Return) && !running){
            state = DCPU.step(state);

            if (state.ticks % 1000 == 0){
                for (int i = 0; i < state.peripherals.Count; i++){
                    state.peripherals[i].updatePeripheral();
                }
            }
        }
    }

    private long lastElapsed;
    private long tts = 0;
    private long lastMilli = 0;
    private long ticks = 0;
    private void Run(){
        Stopwatch sw = Stopwatch.StartNew();
        while (running){
            state = DCPU.step(state);
            tts++;
            ticks++;
            if(state.ticks % 1000 == 0){
                for(int i = 0; i < state.peripherals.Count; i++){
                    state.peripherals[i].updatePeripheral();
                }
            }

            if(sw.ElapsedMilliseconds - lastMilli >= 1000){
                lastElapsed = tts;
                tts = 0;
                lastMilli = sw.ElapsedMilliseconds;
            }

            while(sw.ElapsedTicks < ticks * 100){}
        }
    }

    private void OnGUI(){
        GUI.Label(new Rect(0, 0, 400, 20), "Last Elapsed: " + lastElapsed);
        GUI.Label(new Rect(0, 20, 400, 20), "ticks: " + state.ticks);
        GUI.Label(new Rect(0, 40, 400, 20), "Actual:    " + state.ticks.ToString());
        GUI.Label(new Rect(0, 60, 400, 20), "A:  0x" + state.registers[DCPU.A_REG].ToString("X") + " |B: 0x" + state.registers[DCPU.B_REG].ToString("X") + " |C: 0x" + state.registers[DCPU.C_REG].ToString("X"));
        GUI.Label(new Rect(0, 80, 400, 20), "X:  0x" + state.registers[DCPU.X_REG].ToString("X") + " |Y: 0x" + state.registers[DCPU.Y_REG].ToString("X") + " |Z: 0x" + state.registers[DCPU.Z_REG].ToString("X"));
        GUI.Label(new Rect(0, 100, 400, 20), "I:  0x" + state.registers[DCPU.I_REG].ToString("X") + " |J: 0x" + state.registers[DCPU.J_REG].ToString("X"));
        GUI.Label(new Rect(0, 120, 400, 20), "PC:  0x" + state.PC + " |EX: 0x" + state.EX.ToString("X") + " |ADDR: 0x" + state.memory[state.PC].ToString("X"));
    }
}
