using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCPU{
    public static int MEM_SIZE = 0x10000;
    public static int REG_SIZE = 0x8;
    //
    public static int A_REG = 0;
    public static int B_REG = 1;
    public static int C_REG = 2;
    public static int X_REG = 3;
    public static int Y_REG = 4;
    public static int Z_REG = 5;
    public static int I_REG = 6;
    public static int J_REG = 7;

    //
    private static ushort aRef, bRef; // the ref modes are used to store the memory or register index for final processing
    private static ushort aMode, bMode; // the modes are used to store the current mode of the operation variable
    //
    private enum varMode {MEM_MODE, REG_MODE, LIT_MODE, PC_MODE, SP_MODE, EX_MODE};
    private enum opCodes {spi, SET, ADD, SUB, MUL, MLI, DIV, DVI, MOD, MDI, AND, BOR, XOR, SHR, ASR, SHL, IFB, IFC, IFE, IFN, IFG, IFA, IFL, IFU, NU18, NU19, ADX, SBX, NU1C, NU1D, STI, STD}
    private enum spOpCodes {rfe, JSR, NU02, NU03, NU04, NU05, NU06, NU07, INT, IAG, IAS, RFI, IAQ, NU0D, NU0E, NU0F, HWN, HWQ, HWI};
    // takes the current dcpu state 
    // moves it one cycle forward
    // and returns a  new state
    public static DCPUState step(DCPUState state){
        aRef  = 0;  bRef = 0;
        aMode = 0; bMode = 0;
        ushort addr = state.memory[state.PC++];
        ushort aa = (ushort)((addr >> 10) & 0x3F), bb = (ushort)((addr >> 5) & 0x1F), op = (ushort)((addr) & 0x1F);
        ushort nxtA = 0, nxtB = 0;

        if(op != 0){
            if(needsNextVar(aa)){
                nxtA = state.memory[state.PC++];
                tick(state);
            }
            if (needsNextVar(bb)){
                nxtB = state.memory[state.PC++];
                tick(state);
            }
            ushort a = handleVar(state, aa, nxtA, false);
            ushort b = handleVar(state, bb, nxtB, true);
            handleOpCode(state, op, b, a);
        }else{
            if(needsNextVar(aa)){
                nxtA = state.memory[state.PC++];
                tick(state);
            }
            ushort a = handleSpecialVar(state, aa, nxtA);
            handleSpecialOpCode(state, bb, a);
        }
        tick(state);
        return state;
    }



    //helper functions for basic op codes
    private static ushort handleVar(DCPUState state, ushort value, ushort nxtVal, bool isB){
        if(value <= 0x07){ // register (A, B, C, X, Y, Z, I, J)
            if (isB){
                bMode = (ushort)varMode.REG_MODE;
                bRef = value;
            }
            return state.registers[value];

        }else if(value <= 0x0F){ // [register]
            if (isB){
                bMode = (ushort)varMode.MEM_MODE;
                bRef = state.registers[value % REG_SIZE];
            }
            return state.memory[state.registers[value % REG_SIZE]];

        }else if(value <= 0x17){ // [register + next word]
            if(isB){
                bMode = (ushort)varMode.MEM_MODE;
                bRef = (ushort)(state.registers[value % REG_SIZE] + nxtVal);
            }
            return state.memory[state.registers[value % REG_SIZE] + nxtVal];

        }else if(value == 0x18){ // PUSH/[--SP] POP/[SP++]
            if(isB){
            bMode = (ushort)varMode.MEM_MODE;
                ushort temp = --state.SP;
            bRef = (temp);
            return state.memory[temp];
            }else{
                return state.memory[state.SP++];
            }
        }else if(value == 0x19){ // [SP] / PEEK
            if(isB){
                bMode = (ushort)varMode.MEM_MODE;
                bRef = state.SP;
            }
            return state.memory[state.SP];

        }else if(value == 0x1A){ // [SP + next word] / PICK n
            if(isB){
                bMode = (ushort)varMode.MEM_MODE;
                bRef = (ushort)(state.SP + nxtVal);
            }
            return state.memory[state.SP + nxtVal];

        }else if(value == 0x1B){ // SP
            if(isB){
                bMode = (ushort)varMode.SP_MODE;
                bRef = state.SP; 
            }
            return state.SP;

        } else if(value == 0x1C){ // PC
            if(isB){
                bMode = (ushort)varMode.PC_MODE;
                bRef = state.PC;
            }
            return state.PC;

        }else if(value == 0x1D){ // EX
            if(isB){
                bMode = (ushort)varMode.EX_MODE;
                bRef = state.EX;
            }
            return state.EX;

        }else if(value == 0x1E){ // [next word]
            if(isB){
                bMode = (ushort)varMode.MEM_MODE;
                bRef = nxtVal;
            }
            return state.memory[nxtVal];

        }else if(value == 0x1F){ // next word (lit)
            if(isB){
                bMode = (ushort)varMode.LIT_MODE;
                bRef = nxtVal;
            }
            return nxtVal;
        }else if(value >= 0x20 && value <= 0x3F){
            if (isB){
                bMode = (ushort)varMode.LIT_MODE;
                bRef = (ushort)(value - 33);
            }
            return (ushort)(value - 33);
        }else{
            //this is for if the a or b variable exceeds its bounds
        }
        return 0;
    }
    private static void handleOpCode(DCPUState state, ushort op, ushort b, ushort a){

        ushort res = 0; // temp container for result
        bool needsProcessing = true; // this will only be true if data needs to be stored back to the dcpu
        ushort amt = 0; //used for calculating the skip length of the dcpu's if statements
        switch (op){
            case (ushort)opCodes.SET: //sets b to a
                res = a;
                tick(state);
                break;

            case (ushort)opCodes.ADD: //add b and a together and stores it in b
                res = (ushort)(b + a);
                if((b + a) > 0xFFFF){state.EX = (ushort)((b + a) % 0xFFFF);}
                tick(state, 2);
                break;

            case (ushort)opCodes.SUB: //subtracts b from a
                res = (ushort)(b - a);
                if ((b - a) < 0) state.EX = 0xFFFF;
                tick(state, 2);
                break;

            case (ushort)opCodes.MUL: // multiply b and a unsgined
                res = (ushort)(b * a);
                state.EX = (ushort)(((b * a) >> 16) & 0xFFFF);
                tick(state, 2);
                break;

            case (ushort)opCodes.MLI: // multiply b and a signed
                res = (ushort)((short)b * (short)a);
                state.EX = (ushort)((((short)b * (short)a) >> 16) & 0xFFFF);
                break;

            case (ushort)opCodes.DIV: // divide b by a signed
                if (a == 0) { state.EX = 0;}
                else{res = (ushort)(b / a); state.EX = (ushort)(((b << 16) / a) &0xFFFF);}
                tick(state, 3);
                break;

            case (ushort)opCodes.DVI: // divide b by a unsigned
                if (a == 0) { state.EX = 0; }
                else { res = (ushort)((short)b / (short)a); state.EX = (ushort)((((short)b << 16) / (short)a) & 0xFFFF); }
                tick(state, 3);
                break;

            case (ushort)opCodes.MOD: // gets the modulus of b using a signed
                if(a == 0){ res = b;}
                else{res = (ushort)(b % a); }
                tick(state, 3);
                break;

            case (ushort)opCodes.MDI: // gets the modulus of b using a unsigned
                if(a == 0) { res = b; }
                else { res = (ushort)((short)b % (short)a); }
                tick(state, 3);
                break;

            case (ushort)opCodes.AND: // and b and a together
                res = (ushort)(b & a);
                tick(state);
                break;

            case (ushort)opCodes.BOR: // its or's a and b together 
                res = (ushort)(b | a);
                tick(state);
                break;

            case (ushort)opCodes.XOR: // exclusively or's a and b
                res = (ushort)(b ^ a);
                tick(state);
                break;

            case (ushort)opCodes.SHR: // does a logical right shift
                res = (ushort)(b >> a);
                state.EX = (ushort)(((b << 16) >> a) & 0xFFFF);
                tick(state);
                break;

            case (ushort)opCodes.ASR: // does a arithmatic right shift
                res = (ushort)((short)b >> (short)a);
                state.EX = (ushort)((((short)b << 16) >> (short)a) & 0xFFFF);
                tick(state);
                break;

            case (ushort)opCodes.SHL: // shifts to the left
                res = (ushort)(b << a);
                state.EX = (ushort)(((b << a) >> 16) & 0xFFFF);
                tick(state);
                break;

            case (ushort)opCodes.IFB: // processes the next instruction if binary and'ing dosen't equal 
                amt = ifJumpNum(state);
                if((b & a) == 0) { state.PC += amt; tick(state, amt);}
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFC:// processes the next instruction if binary and'ing equal 0
                amt = ifJumpNum(state);
                if ((b & a) != 0) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFE:// processes the next instruction if a and b are equal
                amt = ifJumpNum(state);
                if(b != a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;
            case (ushort)opCodes.IFN: // processes the next instruction if a and b are not equal
                amt = ifJumpNum(state);
                if (b == a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFG: // processes the next instruction if b is greater than a (unsigned)
                amt = ifJumpNum(state);
                if (b < a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFA: // processes the next instruction if b is greate than a (signed)
                amt = ifJumpNum(state);
                if ((short)b < (short)a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFL: // processes the next instruction if b is less than a (unsigned)
                amt = ifJumpNum(state);
                if (b > a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.IFU: // processes the next instruction if b is less than a (signed)
                amt = ifJumpNum(state);
                if ((short)b > (short)a) { state.PC += amt; tick(state, amt); }
                tick(state, 2);
                needsProcessing = false;
                break;

            case (ushort)opCodes.ADX: // sets b to b + a + EX and set EX to 0x0001 if there is an overflow otherwise EX = 0
                res = (ushort)(b + a + state.EX);
                if((int)(b + a + state.EX) > 0xffff) { state.EX = 1; }
                else { state.EX = 0; }
                break;

            case (ushort)opCodes.SBX: // sets b to b - a + EX and set EX to 0xFFFF if there is an overflow otherwise EX = 0
                res = (ushort)(b - a + state.EX);
                if ((b - a + state.EX) < 0x0) { state.EX = 0xFFFF; }
                else { state.EX = 0; }
                break;

            case (ushort)opCodes.STI: //sets a to b and adds one to the I and J reigsters
                res = a;
                ++state.registers[I_REG];
                ++state.registers[J_REG];
                break;

            case (ushort)opCodes.STD: //sets a to b and subtracts one from the I and J reigsters
                res = a;
                --state.registers[I_REG];
                --state.registers[J_REG];
                break;
        }
        if(needsProcessing){
            if(bMode == (ushort)varMode.MEM_MODE){
                state.memory[bRef] = res;
            }else if(bMode == (ushort)varMode.REG_MODE){
                state.registers[bRef] = res;
            }else if(bMode == (ushort)varMode.PC_MODE){
                state.PC = res;
            }else if(bMode == (ushort)varMode.SP_MODE){
                state.SP = res;
            }else if(bMode == (ushort)varMode.EX_MODE){
                state.EX = res;
            } else{

            }
        }
    }

    //helper functions for advanced op codes
    private static void handleSpecialOpCode(DCPUState state, ushort op, ushort a){
        bool needsProcessing = false;
        ushort res = 0;
        switch (op){
            case (ushort)spOpCodes.JSR:
                state.memory[--state.SP] = state.PC;
                state.PC = a;
                tick(state, 3);
                break;

            case (ushort)spOpCodes.INT:
                if (state.IA == 0 || state.canQueue) {
                    return;
                }

                state.memory[--state.SP] = state.PC;
                state.memory[--state.SP] = state.registers[DCPU.A_REG];
                state.PC = state.IA;
                state.registers[DCPU.A_REG] = a;
                tick(state, 4);
                break;

            case (ushort)spOpCodes.IAG:
                res = state.IA;
                needsProcessing = true;
                tick(state);
                break;

            case (ushort)spOpCodes.IAS:
                state.IA = a;
                tick(state);
                break;

            case (ushort)spOpCodes.RFI:
                state.registers[DCPU.A_REG] = state.memory[state.SP++];
                state.PC = state.memory[state.SP++];
                tick(state, 3);
                break;

            case (ushort)spOpCodes.IAQ:
                if(a == 0) { state.canQueue = false;}
                else { state.canQueue = true; }
                break;

            case (ushort)spOpCodes.HWN:
                res = (ushort)state.peripherals.Count;
                tick(state, 2);
                needsProcessing = true;
                break;

            case (ushort)spOpCodes.HWQ:
                if(a < state.peripherals.Count){
                    //id
                    state.registers[DCPU.A_REG] = (ushort)(state.peripherals[a].id & 0xFFFF);
                    state.registers[DCPU.B_REG] = (ushort)((state.peripherals[a].id >> 16) & 0xFFFF);
                    //version
                    state.registers[DCPU.C_REG] = (ushort)(state.peripherals[a].version);
                    //manufacturer
                    state.registers[DCPU.X_REG] = (ushort)(state.peripherals[a].manufacturer & 0xFFFF);
                    state.registers[DCPU.Y_REG] = (ushort)((state.peripherals[a].manufacturer >> 16) & 0xFFFF);
                }
                tick(state, 4);
                break;

            case (ushort)spOpCodes.HWI:
                if(a < state.peripherals.Count){
                    state.peripherals[a].sendInterrupt(state);
                }
                tick(state, 4);
                break;
        }
        if (needsProcessing){
            if (aMode == (ushort)varMode.MEM_MODE){
                state.memory[aRef] = res;
            }else if (aMode == (ushort)varMode.REG_MODE){
                state.registers[aRef] = res;
            }else if (aMode == (ushort)varMode.PC_MODE){
                state.PC = res;
            }else if (aMode == (ushort)varMode.SP_MODE){
                state.SP = res;
            }else if (aMode == (ushort)varMode.EX_MODE){
                state.EX = res;
            }else{}
        }
    }
    private static ushort handleSpecialVar(DCPUState state, ushort value, ushort nxtVal){
        if (value <= 0x07){ // register (A, B, C, X, Y, Z, I, J)
            aMode = (ushort)varMode.REG_MODE;
            aRef = value;
            return state.registers[value];

        }
        else if (value <= 0x0F){ // [register]
            aMode = (ushort)varMode.MEM_MODE;
            aRef = state.registers[value % REG_SIZE];
            return state.memory[state.registers[value % REG_SIZE]];

        }
        else if (value <= 0x17){ // [register + next word]
            aMode = (ushort)varMode.MEM_MODE;
            aRef = (ushort)(state.registers[value % REG_SIZE] + nxtVal);
            return state.memory[state.registers[value % REG_SIZE] + nxtVal];

        }
        else if (value == 0x18){ // PUSH/[--SP] POP/[SP++]
            aMode = (ushort)varMode.MEM_MODE;
            aRef = state.SP++;
            return state.memory[aRef];
        }
        else if (value == 0x19){ // [SP] / PEEK
            aMode = (ushort)varMode.MEM_MODE;
            aRef = state.SP;
            return state.memory[state.SP];

        }
        else if (value == 0x1A){ // [SP + next word] / PICK n
            aMode = (ushort)varMode.MEM_MODE;
            aRef = (ushort)(state.SP + nxtVal);
            return state.memory[state.SP + nxtVal];

        }
        else if (value == 0x1B){ // SP  
            aMode = (ushort)varMode.SP_MODE;
            aRef = state.SP;
            return state.SP;

        }
        else if (value == 0x1C){ // PC
            aMode = (ushort)varMode.PC_MODE;
            aRef = state.PC;
            return state.PC;

        }
        else if (value == 0x1D){ // EX
            aMode = (ushort)varMode.EX_MODE;
            aRef = state.EX;
            return state.EX;

        }
        else if (value == 0x1E){ // [next word]
            aMode = (ushort)varMode.MEM_MODE;
            aRef = nxtVal;
            return state.memory[nxtVal];

        }
        else if (value == 0x1F){ // next word (lit)
            aMode = (ushort)varMode.LIT_MODE;
            aRef = nxtVal;
            return nxtVal;
        }
        else if (value >= 0x20 && value <= 0x3F){
            aMode = (ushort)varMode.LIT_MODE;
            aRef = (ushort)(value - 33);
            return (ushort)(value - 33);
        }else{
            //this is for if the a or b variable exceeds its boudns 
        }
        return 0;
    }    
    //utils
    private static void tick(DCPUState state, int n = 1){
        state.ticks += n;
    }

    private static bool needsNextVar(ushort value){
        if ((value >= 0x10 && value < 0x17) || value == 0xA1 || value == 0x1E || value == 0x1F){
            return true;
        }
        return false;
    }

    private static ushort ifJumpNum(DCPUState state){
        ushort amount = 1;
        ushort addr = state.memory[state.PC];
        ushort aa = (ushort)((addr >> 10) & 0x3F), bb = (ushort)((addr >> 5) & 0x1F);
        if (needsNextVar(aa))
            amount++;
        if (needsNextVar(bb))
            amount++;
        return amount;
    }
}
