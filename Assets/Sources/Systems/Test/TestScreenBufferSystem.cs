using Entitas;
using UnityEngine;

/// Tests screens by modifying the screen buffers on entities.
[SystemAvailability(InstanceKind.Server | InstanceKind.Singleplayer)]
public class TestScreenBufferSystem : IExecuteSystem {

	readonly IGroup<GameEntity> screens;

	public TestScreenBufferSystem(Contexts contexts) {

		screens = contexts.game.GetGroup(GameMatcher.ScreenBuffer);
	}

	public void Execute() {

        var teststrings = new string[] { "google","compliant","notch","HELLO WORLD","Sonicpixelation","LudereGames","FOR TEST PURPOSES ONLY", "Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "Here is a very long string, does this all render?", "!@#%&@(*#%&^", "FUUUUU" };

		foreach (var e in screens.GetEntities()) {

			var buffer = e.screenBuffer;
			var data = buffer.data;

            data.Fill(ColorCode.Black);

            data.WriteStr(0, 0, teststrings[Random.Range(0, teststrings.Length)], ColorCode.White, ColorCode.Black);
			data.SetColorCodeAt(0, 0, GetRandomColorCode());

			e.ReplaceScreenBuffer(data);
		}
	}

	ColorCode GetRandomColorCode() {

		return (ColorCode)Random.Range(0, 15);
	}

	void TestAllCharacters(ScreenBufferState data) {

		data.WriteStr(0, 0, " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", ColorCode.White, ColorCode.Black);
	}

	void RandomNoise(ScreenBufferState data, int numNoisePixels) {

		for (int i = 0; i < numNoisePixels; i++) {

			int x = Random.Range(0, (int)data.size.x);
			int y = Random.Range(0, (int)data.size.y);

			data.SetColorCodeAt(x, y, (ColorCode)Random.Range(0, 15));
		}
	}
}
