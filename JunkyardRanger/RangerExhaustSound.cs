using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;

namespace JunkyardRanger
{
	// How a Ranger sounds with the performance exhaust on. After the game's
	// AudioControl.Update has set the engine sound for the frame:
	//   * the engine loop that's playing (I4, V8 or 2J) is pitched down, deeper;
	//   * two loops of our own play from the exhaust, made here when first needed,
	//     a cammed V8's: the lope at idle (lumpy, uneven "brap ... brap-brap"
	//     cycles) that fades as the revs rise, and an even, raspy rumble that takes
	//     over on the throttle. Both follow the engine's revs (the gearbox's
	//     pitch) and its volume.
	internal static class RangerExhaustSound
	{
		private const float Deeper = 0.9f;
		private const int Rate = 44100;

		private static AudioClip rumbleClip;
		private static AudioClip chopClip;

		private static FieldInfo carField, gbField, sourcesField, crField;
		private static FieldInfo pitchField, usingV8Field, usingI6Field, controlledField;
		private static FieldInfo vertField, gasField;

		private class Sound
		{
			public AudioSource rumble;
			public AudioSource chop;
			public float level;
		}

		private static readonly Dictionary<int, Sound> sounds = new Dictionary<int, Sound>();

		public static void Patch(HarmonyLib.Harmony harmony)
		{
			Type audio = AccessTools.TypeByName("AudioControl");
			MethodInfo update = audio != null ? AccessTools.Method(audio, "Update") : null;
			if (update == null)
			{
				RangerMod.Log("The game's AudioControl wasn't found: the performance exhaust won't change the sound.");
				return;
			}
			carField = AccessTools.Field(audio, "car1");
			gbField = AccessTools.Field(audio, "GB");
			sourcesField = AccessTools.Field(audio, "aSources");
			crField = AccessTools.Field(audio, "cr");
			harmony.Patch(update, postfix: new HarmonyMethod(typeof(RangerExhaustSound).GetMethod("UpdatePostfix", BindingFlags.Static | BindingFlags.NonPublic)));
		}

		public static void Reset()
		{
			sounds.Clear();
		}

		private static bool warned;

		private static void UpdatePostfix(Component __instance)
		{
			try
			{
				Apply(__instance);
			}
			catch (Exception e)
			{
				if (!warned)
				{
					warned = true;
					RangerMod.Log("Exhaust sound: " + e.Message);
				}
			}
		}

		private static void Apply(Component audio)
		{
			Component car = carField != null ? carField.GetValue(audio) as Component : null;
			if (car == null || car.GetComponent<RangerBody>() == null)
			{
				return;
			}
			Renderer exhaust = RangerUpgrades.ExhaustOf(car);
			Sound s;
			sounds.TryGetValue(audio.GetInstanceID(), out s);
			bool fitted = exhaust != null && exhaust.enabled;
			if (!fitted)
			{
				if (s != null)
				{
					Fade(s, 0f, 0f, 1f);
				}
				return;
			}
			AudioSource[] sources = sourcesField != null ? sourcesField.GetValue(audio) as AudioSource[] : null;
			if (sources == null)
			{
				return;
			}
			if (controlledField == null)
			{
				controlledField = AccessTools.Field(car.GetType(), "controlled");
				usingV8Field = AccessTools.Field(car.GetType(), "usingV8");
				usingI6Field = AccessTools.Field(car.GetType(), "usingI6");
			}
			bool running = controlledField != null && (bool)controlledField.GetValue(car);
			bool v8 = usingV8Field != null && (bool)usingV8Field.GetValue(car);
			bool i6 = usingI6Field != null && (bool)usingI6Field.GetValue(car);
			int[] engine = i6 ? new[] { 37, 38 } : v8 ? new[] { 32, 33 } : new[] { 0 };

			// The engine loop the game is playing: deeper. (The game sets its pitch
			// every frame, so this never adds up.)
			float level = 0f;
			AudioSource template = null;
			foreach (int i in engine)
			{
				if (i >= sources.Length || sources[i] == null)
				{
					continue;
				}
				if (running)
				{
					sources[i].pitch *= Deeper;
				}
				if (sources[i].volume >= level)
				{
					level = sources[i].volume;
					template = sources[i];
				}
			}
			if (s == null)
			{
				s = Make(audio, exhaust, template ?? sources[0]);
				sounds[audio.GetInstanceID()] = s;
			}

			object gb = gbField != null ? gbField.GetValue(audio) : null;
			if (gb != null && pitchField == null)
			{
				pitchField = AccessTools.Field(gb.GetType(), "currentPitch");
			}
			float revs = gb != null && pitchField != null ? (float)pitchField.GetValue(gb) : 1f;
			object cr = crField != null ? crField.GetValue(audio) : null;
			if (cr != null && vertField == null)
			{
				vertField = AccessTools.Field(cr.GetType(), "Vert");
				gasField = AccessTools.Field(cr.GetType(), "GasInput");
			}
			float throttle = 0f;
			if (cr != null)
			{
				throttle = Mathf.Max(vertField != null ? (float)vertField.GetValue(cr) : 0f, gasField != null ? (float)gasField.GetValue(cr) : 0f);
			}
			if (!running)
			{
				level = 0f;
			}
			s.level = Mathf.MoveTowards(s.level, Mathf.Clamp01(level), Time.deltaTime * 3f);
			// Revs: 1 at idle, up to the gearbox's top (about 2.4). The lope rules at
			// idle and fades as the revs come up; the rumble takes over on the gas.
			float idle = Mathf.Clamp01(1f - (revs - 1f) / 0.6f);
			float rumbleVolume = s.level * (0.25f + 0.55f * Mathf.Clamp01(throttle) + 0.25f * (1f - idle));
			float chopVolume = s.level * idle;
			Fade(s, rumbleVolume, chopVolume, revs);
		}

		private static void Fade(Sound s, float rumble, float chop, float revs)
		{
			if (s.rumble != null)
			{
				s.rumble.volume = Mathf.MoveTowards(s.rumble.volume, rumble, Time.deltaTime * 2f);
				s.rumble.pitch = revs;
			}
			if (s.chop != null)
			{
				s.chop.volume = Mathf.MoveTowards(s.chop.volume, chop, Time.deltaTime * 2f);
				s.chop.pitch = revs;
			}
		}

		// Two sources at the exhaust, set up like the game's engine sound (3D
		// settings, mixer group), playing our loops.
		private static Sound Make(Component audio, Renderer exhaust, AudioSource like)
		{
			GameObject go = new GameObject("RangerExhaustSound");
			go.transform.SetParent(exhaust.transform, false);
			go.transform.position = exhaust.bounds.center;
			Sound s = new Sound();
			s.rumble = Source(go, like, RumbleClip());
			s.chop = Source(go, like, ChopClip());
			return s;
		}

		private static AudioSource Source(GameObject go, AudioSource like, AudioClip clip)
		{
			AudioSource a = go.AddComponent<AudioSource>();
			if (like != null)
			{
				a.outputAudioMixerGroup = like.outputAudioMixerGroup;
				a.spatialBlend = like.spatialBlend;
				a.rolloffMode = like.rolloffMode;
				a.minDistance = like.minDistance;
				a.maxDistance = like.maxDistance;
				a.dopplerLevel = like.dopplerLevel;
				a.priority = like.priority;
			}
			a.clip = clip;
			a.loop = true;
			a.playOnAwake = false;
			a.volume = 0f;
			a.Play();
			return a;
		}

		// --- The loops, made once. ---

		// A big-cam V8 at about 800 rpm. Each loop is a whole number of engine
		// cycles (14), so it repeats seamlessly; the sources' pitch follows the revs.
		private const float IdleRpm = 800f;

		// Lumpy, uneven cycles: brap ... brap-brap.
		private static readonly float[] Lope = { 1.0f, 0.3f, 0.75f, 1.0f, 0.28f, 0.5f, 0.95f, 0.35f, 0.85f, 1.0f, 0.3f, 0.65f, 0.4f, 0.9f };
		// On the throttle: every cycle near full.
		private static readonly float[] Even = { 1f, 0.95f, 1f, 0.97f, 1f, 0.94f, 1f, 0.96f, 1f, 0.95f, 1f, 0.97f, 0.99f, 0.95f };

		private static AudioClip RumbleClip()
		{
			if (rumbleClip == null)
			{
				rumbleClip = Make("RangerExhaustRumble", Engine(IdleRpm, Even, 0.9f, 84f, 0.007f, 1.6f, 9));
			}
			return rumbleClip;
		}

		private static AudioClip ChopClip()
		{
			if (chopClip == null)
			{
				chopClip = Make("RangerExhaustChop", Engine(IdleRpm, Lope, 0.8f, 92f, 0.0075f, 1.25f, 5));
			}
			return chopClip;
		}

		private static AudioClip Make(string name, float[] data)
		{
			AudioClip clip = AudioClip.Create(name, data.Length, 1, Rate, false);
			clip.SetData(data, 0);
			return clip;
		}

		// The exhaust of a cross-plane V8: per engine cycle (two turns), eight
		// firings 90 degrees apart, alternating banks unevenly (L R R L R L L R, the
		// banks a little different in strength and tone); each cycle as strong as
		// `cycles` says (the cam's lope), with a little jitter, and in weak cycles
		// some firings nearly missing. Each pulse: the pipe's ring (a tone and its
		// octave), a lower boom, and a short burst of band-passed noise (the rasp);
		// then a muffler low-pass and a soft clip.
		private static float[] Engine(float rpm, float[] cycles, float rasp, float thumpHz, float decay, float drive, int seed)
		{
			double cycle = 120.0 / rpm;
			int n = (int)Math.Round(cycle * cycles.Length * Rate);
			double[] buf = new double[n];
			Random rng = new Random(seed);
			int[] bank = { 0, 1, 1, 0, 1, 0, 0, 1 };
			int pulseLen = (int)(Rate * 0.09);
			for (int c = 0; c < cycles.Length; c++)
			{
				for (int f = 0; f < 8; f++)
				{
					double jitter = (rng.NextDouble() - 0.5) * 0.0025;
					double t0 = (c + f / 8.0) * cycle + jitter;
					double a = cycles[c] * (bank[f] == 0 ? 1.0 : 0.8) * (0.85 + 0.3 * rng.NextDouble());
					if (cycles[c] < 0.6 && rng.NextDouble() < 0.35)
					{
						a *= 0.25;
					}
					double hz = thumpHz * (bank[f] == 0 ? 1.0 : 1.12);
					int start = (int)(t0 * Rate);
					double hp = 0, lp = 0, prev = 0;
					for (int j = 0; j < pulseLen; j++)
					{
						double t = j / (double)Rate;
						double thump = Math.Exp(-t / decay) * (Math.Sin(2 * Math.PI * hz * t) + 0.55 * Math.Sin(2 * Math.PI * hz * 2 * t + 0.4));
						double body = 0.6 * Math.Exp(-t / (decay * 2.6)) * Math.Sin(2 * Math.PI * hz * 0.5 * t);
						double w = rng.NextDouble() * 2 - 1;
						hp = 0.85 * (hp + w - prev);
						prev = w;
						lp += (hp - lp) * 0.09;
						double r = rasp * 1.8 * Math.Exp(-t / 0.006) * lp;
						buf[(start + j + n) % n] += a * (thump + body + r);
					}
				}
			}
			// Twice round, so the filter's state meets itself at the loop's ends.
			double y = 0;
			float[] o = new float[n];
			for (int pass = 0; pass < 2; pass++)
			{
				for (int i = 0; i < n; i++)
				{
					y += (buf[i] - y) * 0.2;
					if (pass == 1)
					{
						o[i] = (float)y;
					}
				}
			}
			float peak = 0.0001f;
			for (int i = 0; i < n; i++)
			{
				o[i] = (float)Math.Tanh(o[i] * drive);
				peak = Mathf.Max(peak, Mathf.Abs(o[i]));
			}
			for (int i = 0; i < n; i++)
			{
				o[i] *= 0.9f / peak;
			}
			return o;
		}
	}
}
