using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class DCPU_Controller : MonoBehaviour {
    private dcpuState state;
    private bool running = false;
    private Thread thread;

    private void Start(){
        state = new dcpuState();

    }


    public void StartDCPU(){
        running = true;
        thread = new Thread(Run);
        thread.Start();
    }

    private void OnDestroy(){
        running = false;
        thread.Join();
    }


    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space) && !running){
            StartDCPU();
        }
    }

    private long lastElapsed;
    private long tts = 0;
    private long lastMilli = 0;
    private void Run(){
        Stopwatch sw = Stopwatch.StartNew();
        while (running){
            state = DCPU.step(state);
            tts++;

            if(sw.ElapsedMilliseconds - lastMilli >= 1000){
                lastElapsed = tts;
                tts = 0;
                lastMilli = sw.ElapsedMilliseconds;
            }
        }


    }

    private void OnGUI(){
        GUI.Label(new Rect(0, 0, 400, 20), "Last Elapsed: " + lastElapsed);
        GUI.Label(new Rect(0, 20, 400, 20), "Actual:" + state.ticks.ToString());
        GUI.Label(new Rect(0, 40, 400, 20), "ticks: " + state.ticks);
        GUI.Label(new Rect(0, 60, 400, 20), "PC: " + state.PC);
        //GUI.Label(new Rect(0, 40, 400, 20), "Expected: " + (Time.time * 100000f).ToString("00"));
    }
}
