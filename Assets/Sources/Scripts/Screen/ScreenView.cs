using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// A Behaviour that will handle the connect between underlying data and the Compute Shader that powers the display.
/// Data (a color index array and an array of colors) is sent to the gpu in a compressed form, i.e. 4-bits per color index. 
/// The Compute Shader generates the actual texture using that data.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ScreenView : MonoBehaviour {

	public const int numBytesPerColor = 16;
	public const int numBytesPerUint = 4;

	public const int numColors = 16;

	public float maxRecomputeInterval = 0.1f;

	/// The Compute Shader reference. Is replaced with an instance on Start.
	[SerializeField]
	ComputeShader computeShader;
	int csKernelIndex;

	// TODO Extract a pair of cpu-local and gpu-local buffers into a separate class.
	// Holds the color indices for each pixel. Compressed to 4 bits per index. 
	uint[] colorIndicesCPU;
	ComputeBuffer colorIndicesGPU; 	
	/// Are colorIndicesCPU and colorIndicesGPU out of sync?
	bool dirtyColorIndices;

	// Holds the color palette. A mapping from color indices to colors.
	Color[] colorPaletteCPU;
	ComputeBuffer colorPaletteGPU; 
	/// Are colorPaletteCPU and colorPaletteGPU out of sync?
	bool dirtyColorPalette;

	/// The texture the compute shader renders to.
	RenderTexture targetTexture;

	/// A timer used to reduce the number of updates performed by the compute shader.
	float recomputeTimer = 0;

	bool isVisibleToPlayer;

	public bool isInitialized {get; private set;}

	[SerializeField]
	Vector2Int _resolution = new Vector2Int(128, 96);
	public Vector2Int resolution {
		get {
			return _resolution;
		}
		set {
			
			if (isInitialized) {
				throw new Exception("[ScreenView]: You can't set resolution after initialization.");
			}

			_resolution = value;
 		}
	}
		
    /// Set a pixel in the display.
    public void SetPixel(int x, int y, byte colorIndex) {

		// TODO Move it somewhere else.
		if (!isInitialized) Initialize();

		colorIndex = (byte)(colorIndex % numColors);
		int pixelNum = (y * resolution.x) + x;
		BitPacker.Pack(colorIndex, pixelNum, colorIndicesCPU);

		dirtyColorIndices = true;
	}

	/// Sets all color indices at once.
	public void SetColorIndices(uint[] newColorIndices) {

		if (newColorIndices.Length != colorIndicesCPU.Length) {

			throw new Exception("The given color indices array doesn't have the expected size!");
		}
		
		Array.Copy(newColorIndices, colorIndicesCPU, newColorIndices.Length);

		dirtyColorIndices = true;
	}

	/// Set a color on the palette.
	public void SetColor(int index, Color color) {

		colorPaletteCPU[index] = color;
		dirtyColorPalette = true;
	}

	/// Create all the buffers, set up the Compute Shader etc. Can only be done once. Depends on the resolution
	public void Initialize() {

		if (isInitialized) {
			throw new Exception("The ScreenView is already initialized!");
		}

		// CPU side data containers.
		colorIndicesCPU = BitPacker.MakeStorageFor4BitValues(resolution.x * resolution.y);
		colorPaletteCPU = CreateDefaultColorPalette();

		// GPU-side data containers.
		colorIndicesGPU = new ComputeBuffer(resolution.x * resolution.y, numBytesPerUint);
		colorPaletteGPU = new ComputeBuffer(numColors, numBytesPerColor);

		targetTexture = CreateTargetTexture();
		SetupComputeShader();
		SetupMaterial();

		dirtyColorPalette = dirtyColorIndices = true;
		isVisibleToPlayer = true;

		isInitialized = true;
	}

	void Start() {

		if (!isInitialized) {
			Initialize();
		}

		RecomputeIfNeeded();
	}

	void LateUpdate() {

		// If the player isn't looking at the display, don't bother rendering.
		if (!isVisibleToPlayer) return;

		recomputeTimer -= Time.deltaTime;

		if (recomputeTimer <= 0) {

			recomputeTimer = Random.value * maxRecomputeInterval;

			RecomputeIfNeeded();
		}
	}

	void OnBecameInvisible() {

		isVisibleToPlayer = false;
	}

	void OnBecameVisible() {

		isVisibleToPlayer = true;
		dirtyColorIndices = dirtyColorPalette = true;
	}

	/// Release the buffers and tell the GPU to destroy them when we're deleting the monitor.
	void OnDestroy() {

		colorIndicesGPU.Release();
		colorPaletteGPU.Release();
	}

	Color[] CreateDefaultColorPalette() {

		return new Color[] {
			new Color(0f,0f,0f,1f), new Color(0f,0f,0.66f,1f),new Color(0f,0.66f,0f,1f),new Color(0f,0.66f,0.66f,1f),
			new Color(0.66f,0f,0f,1f), new Color(0.66f,0f,0.66f,1f),new Color(1f,0.66f,0f,1f),new Color(0.66f,0.66f,0.66f,1f),
			new Color(0.33f,0.33f,0.33f,1f), new Color(0.33f,0.33f,1f,1f),new Color(0.33f,1f,0.33f,1f),new Color(0.33f,1f,1f,1f),
			new Color(1f,0.33f,0.33f,1f), new Color(1f,0.33f,1f,1f),new Color(1f,1f,0.33f,1f),new Color(1f,1f,1f,1f) 
		};
	}

	/// Generate a RenderTexture for use with the Compute Shader
	RenderTexture CreateTargetTexture() {

		var texture = new RenderTexture(resolution.x, resolution.y, 24) {
			enableRandomWrite = true,
			filterMode = FilterMode.Point
		};
		texture.Create();

		return texture;
	}

	void SetupComputeShader() {

		computeShader = Instantiate(computeShader);
		csKernelIndex = computeShader.FindKernel("CSMain");

		computeShader.SetInt("_Width", (int)resolution.x);
		computeShader.SetInt("_Height", (int)resolution.y);

		computeShader.SetBuffer(csKernelIndex, "Data"  , colorIndicesGPU);
		computeShader.SetBuffer(csKernelIndex, "Colors", colorPaletteGPU);

		computeShader.SetTexture(csKernelIndex, "Result", targetTexture);
	}

	void SetupMaterial() {

		var material = GetComponent<MeshRenderer>().material;
		material.SetTexture("_MainTex", targetTexture);
	}

	// Sync the buffers and recompute the target texture if needed.
	void RecomputeIfNeeded() {

		if (!(dirtyColorIndices || dirtyColorPalette)) return;

		if (dirtyColorIndices) UpdateGPUColorIndices();
		if (dirtyColorPalette) UpdateGPUColorPalette();

		ComputeTargetTexture();
	}

	void UpdateGPUColorIndices() {

		colorIndicesGPU.SetData(colorIndicesCPU);
		computeShader.SetBuffer(csKernelIndex, "Data", colorIndicesGPU);

		dirtyColorIndices = false;
	}

	void UpdateGPUColorPalette() {

		colorPaletteGPU.SetData(colorPaletteCPU);
		computeShader.SetBuffer(csKernelIndex, "Colors", colorPaletteGPU);

		dirtyColorPalette = false;
	}

	void ComputeTargetTexture() {

		computeShader.Dispatch(csKernelIndex, resolution.x / 8, resolution.y / 8, 1);
	}
}
