using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lem1802 : Peripheral {

    public static int PIXE_WIDTH  = 128;
    public static int PIXE_HEIGHT = 96;
    public static int CELL_WIDTH  = 4;
    public static int CELL_HEIGHT = 8;
    public static int GRID_WIDTH  = 32;
    public static int GRID_HEIGHT = 12;

    public static int BLINK_RATE = 30;

    public ushort screen_Reg  = 0;
    public ushort palette_Reg = 0;
    public ushort font_Reg    = 0;

    private ushort[] defaultFont;
    private ushort[] defaultPalette;

    private Texture2D texture;
    private MeshRenderer meshRenderer;
    private bool blink = false;
    private long blink_Tick = 0;

    public Lem1802() : base(0x7349f615, 0x1802, 0x1c6c8b36 ) {
        defaultFont = new ushort[256];
        defaultPalette = new ushort[16];
        setupDefaultFont();
        setupDefaultPalette();
    }

    private void setupDefaultPalette(){
        defaultPalette[0x0] = 0x000;//black
        defaultPalette[0x1] = 0x00A;//dark blue
        defaultPalette[0x2] = 0x0A0;//dark green
        defaultPalette[0x3] = 0x0AA;//dark aqua
        defaultPalette[0x4] = 0xA00;//dark red
        defaultPalette[0x5] = 0xA0A;//dark purple
        defaultPalette[0x6] = 0xFA0;//gold
        defaultPalette[0x7] = 0xAAA;//gray
        defaultPalette[0x8] = 0x555;//dark gray
        defaultPalette[0x9] = 0x55F;//blue
        defaultPalette[0xA] = 0x5F5;//green
        defaultPalette[0xB] = 0x5FF;//aqua
        defaultPalette[0xC] = 0xF55;//red
        defaultPalette[0xD] = 0xF5F;//light pruple
        defaultPalette[0xE] = 0xFF5;//yellow
        defaultPalette[0xF] = 0xFFF;//white
    }

    private void setupDefaultFont(){
        ushort[] temp = { 0xb79e, 0x388e, 0x722c, 0x75f4, 0x19bb, 0x7f8f, 0x85f9, 0xb158, 0x242e, 0x2400, 0x082a, 0x0800, 0x0008, 0x0000, 0x0808, 0x0808, 0x00ff, 0x0000, 0x00f8, 0x0808, 0x08f8, 0x0000, 0x080f, 0x0000, 0x000f, 0x0808, 0x00ff, 0x0808, 0x08f8, 0x0808, 0x08ff, 0x0000, 0x080f, 0x0808, 0x08ff, 0x0808, 0x6633, 0x99cc, 0x9933, 0x66cc, 0xfef8, 0xe080, 0x7f1f, 0x0701, 0x0107, 0x1f7f, 0x80e0, 0xf8fe, 0x5500, 0xaa00, 0x55aa, 0x55aa, 0xffaa, 0xff55, 0x0f0f, 0x0f0f, 0xf0f0, 0xf0f0, 0x0000, 0xffff, 0xffff, 0x0000, 0xffff, 0xffff, 0x0000, 0x0000, 0x005f, 0x0000, 0x0300, 0x0300, 0x3e14, 0x3e00, 0x266b, 0x3200, 0x611c, 0x4300, 0x3629, 0x7650, 0x0002, 0x0100, 0x1c22, 0x4100, 0x4122, 0x1c00, 0x1408, 0x1400, 0x081c, 0x0800, 0x4020, 0x0000, 0x0808, 0x0800, 0x0040, 0x0000, 0x601c, 0x0300, 0x3e49, 0x3e00, 0x427f, 0x4000, 0x6259, 0x4600, 0x2249, 0x3600, 0x0f08, 0x7f00, 0x2745, 0x3900, 0x3e49, 0x3200, 0x6119, 0x0700, 0x3649, 0x3600, 0x2649, 0x3e00, 0x0024, 0x0000, 0x4024, 0x0000, 0x0814, 0x2200, 0x1414, 0x1400, 0x2214, 0x0800, 0x0259, 0x0600, 0x3e59, 0x5e00, 0x7e09, 0x7e00, 0x7f49, 0x3600, 0x3e41, 0x2200, 0x7f41, 0x3e00, 0x7f49, 0x4100, 0x7f09, 0x0100, 0x3e41, 0x7a00, 0x7f08, 0x7f00, 0x417f, 0x4100, 0x2040, 0x3f00, 0x7f08, 0x7700, 0x7f40, 0x4000, 0x7f06, 0x7f00, 0x7f01, 0x7e00, 0x3e41, 0x3e00, 0x7f09, 0x0600, 0x3e61, 0x7e00, 0x7f09, 0x7600, 0x2649, 0x3200, 0x017f, 0x0100, 0x3f40, 0x7f00, 0x1f60, 0x1f00, 0x7f30, 0x7f00, 0x7708, 0x7700, 0x0778, 0x0700, 0x7149, 0x4700, 0x007f, 0x4100, 0x031c, 0x6000, 0x417f, 0x0000, 0x0201, 0x0200, 0x8080, 0x8000, 0x0001, 0x0200, 0x2454, 0x7800, 0x7f44, 0x3800, 0x3844, 0x2800, 0x3844, 0x7f00, 0x3854, 0x5800, 0x087e, 0x0900, 0x4854, 0x3c00, 0x7f04, 0x7800, 0x047d, 0x0000, 0x2040, 0x3d00, 0x7f10, 0x6c00, 0x017f, 0x0000, 0x7c18, 0x7c00, 0x7c04, 0x7800, 0x3844, 0x3800, 0x7c14, 0x0800, 0x0814, 0x7c00, 0x7c04, 0x0800, 0x4854, 0x2400, 0x043e, 0x4400, 0x3c40, 0x7c00, 0x1c60, 0x1c00, 0x7c30, 0x7c00, 0x6c10, 0x6c00, 0x4c50, 0x3c00, 0x6454, 0x4c00, 0x0836, 0x4100, 0x0077, 0x0000, 0x4136, 0x0800, 0x0201, 0x0201, 0x0205, 0x0200 };
       defaultFont = temp;
    }


    private void Start(){
        texture = new Texture2D(PIXE_WIDTH, PIXE_HEIGHT, TextureFormat.ARGB32, false);
        meshRenderer = GetComponent<MeshRenderer>();
        for(int y = 0; y < PIXE_HEIGHT; y++){
            for(int x = 0; x < PIXE_WIDTH; x++){
                texture.SetPixel(x, y, new Color(0, 0, 0));
            }
        }
        texture.Apply();
        meshRenderer.material.SetTexture("_MainTex", texture);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
    }


    public void Update(){
        if(screen_Reg != 0){
            for(int y = 0; y < GRID_HEIGHT; y++){
                for(int x = 0; x < GRID_WIDTH; x++){
                    int i = x + y * GRID_WIDTH;
                    ushort value = dcpuController.state.memory[i + screen_Reg];
                    setGrid(x, y, value);
                }
            }
            texture.Apply();
            if(blink_Tick < BLINK_RATE){
                blink_Tick++;
            }else{
                blink_Tick = 0;
                blink = !blink;
            }
        }
    }

    private void setGrid(int x, int y, ushort value){
        int fontIndex  = value & 0x7F;
        int fgIndex    = (value >> 12) & 0xF;
        int bgIndex    = (value >> 8)  & 0xF;
        int shouldBlnk = (value >> 7)  & 1;

        int fontData = 0;

        if(font_Reg == 0){
            fontData = (defaultFont[fontIndex * 2] << 16 | defaultFont[(fontIndex * 2) + 1] & 0xFFFF);
        }else{
            fontData = (dcpuController.state.memory[fontIndex * 2] << 16 | dcpuController.state.memory[(fontIndex * 2) + 1] & 0xFFFF);
        }

        ushort fgPalette = 0;
        ushort bgPalette = 0;
        if(palette_Reg == 0){
            fgPalette = defaultPalette[fgIndex];
            bgPalette = defaultPalette[bgIndex];
        }else{
            fgPalette = dcpuController.state.memory[palette_Reg + fgIndex];
            bgPalette = dcpuController.state.memory[palette_Reg + bgIndex];
        }

        float fg_r = ((fgPalette >> 8) & 0xF);
        float fg_g = ((fgPalette >> 4) & 0xF);
        float fg_b = ((fgPalette) & 0xF);
        Color fgColor = new Color(fg_r / 0xF, fg_g / 0xF, fg_b / 0xF);


        float bg_r = ((bgPalette >> 8) & 0xF);
        float bg_g = ((bgPalette >> 4) & 0xF);
        float bg_b = ((bgPalette) & 0xF);
        Color bgColor = new Color(bg_r / 0xF, bg_g / 0xF, bg_b / 0xF);

        if(shouldBlnk == 1 && blink){
            Color temp = fgColor;
            fgColor = bgColor;
            bgColor = temp;
        }

        int xx = CELL_WIDTH - 1;
        for(int xt = 0; xt < CELL_WIDTH; xt++){
           for(int yt = CELL_HEIGHT - 1; yt >= 0; yt--){
                int j = (fontData >> (yt + (xx * (CELL_HEIGHT))) & 1);
                if (j == 1) {
                    texture.SetPixel((x * CELL_WIDTH) + xt, (y * CELL_HEIGHT) + yt, fgColor);
                } else {
                    texture.SetPixel((x * CELL_WIDTH) + xt, (y * CELL_HEIGHT) + yt, bgColor);
                }
            }
            xx--;
        }

    }

    public override void sendInterrupt(DCPUState state){
        switch (state.registers[DCPU.A_REG]){
            case 0:
                screen_Reg = state.registers[DCPU.B_REG];
                break;
        }
    }

    public override void updatePeripheral() {}
}
