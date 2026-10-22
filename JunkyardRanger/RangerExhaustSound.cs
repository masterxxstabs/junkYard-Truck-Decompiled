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
	//   * two loops of our own play from the exhaust, made here when first needed:
	//     a low rumble (exhaust pulses through the big muffler) that grows with the
	//     throttle, and a lopey "chop" (uneven pulses, a big cam's idle) that's
	//     strongest at idle and fades as the revs rise. Both follow the engine's
	//     revs (the gearbox's pitch) and its volume.
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
			// Revs: 1 at idle, up to the gearbox's top (about 2.4).
			float idle = Mathf.Clamp01(1f - (revs - 1f) / 0.55f);
			float rumbleVolume = s.level * (0.35f + 0.45f * Mathf.Clamp01(throttle) + 0.15f * idle);
			float chopVolume = s.level * 0.85f * idle;
			Fade(s, rumbleVolume, chopVolume, revs);
		}

		private static void Fade(Sound s, float rumble, float chop, float revs)
		{
			if (s.rumble != null)
			{
				s.rumble.volume = Mathf.MoveTowards(s.rumble.volume, rumble, Time.deltaTime * 2f);
				s.rumble.pitch = 0.78f + (revs - 1f) * 0.62f;
			}
			if (s.chop != null)
			{
				s.chop.volume = Mathf.MoveTowards(s.chop.volume, chop, Time.deltaTime * 2f);
				s.chop.pitch = 0.92f + (revs - 1f) * 0.5f;
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

		// Two seconds each, so every pattern (45 exhaust pulses a second, the
		// lope at 1.5 a second) repeats a whole number of times and the loop is
		// seamless.
		private const float Length = 2f;
		private const float Pulses = 45f;

		private static AudioClip RumbleClip()
		{
			if (rumbleClip == null)
			{
				float[] weights = { 1.0f, 0.8f, 0.95f, 0.75f, 1.0f, 0.85f, 0.9f, 0.7f };
				rumbleClip = Make("RangerExhaustRumble", Synth(weights, 0.25f, 58f, 0.014f, 0.0f, 0.22f, 11));
			}
			return rumbleClip;
		}

		private static AudioClip ChopClip()
		{
			if (chopClip == null)
			{
				// Uneven: some pulses big, some barely there, and the whole thing
				// swelling and dipping 1.5 times a second.
				float[] weights = { 1.0f, 0.35f, 0.9f, 0.2f, 1.0f, 0.55f, 0.75f, 0.15f };
				chopClip = Make("RangerExhaustChop", Synth(weights, 0.9f, 46f, 0.02f, 0.0035f, 0.12f, 23));
			}
			return chopClip;
		}

		private static AudioClip Make(string name, float[] data)
		{
			AudioClip clip = AudioClip.Create(name, data.Length, 1, Rate, false);
			clip.SetData(data, 0);
			return clip;
		}

		// A train of exhaust pulses: each a decaying low thump (a tone and its
		// octave) with a puff of filtered noise, weighted by the firing pattern,
		// a slow lope (depth 0..1), and a little timing gallop on every 4th pulse.
		private static float[] Synth(float[] weights, float lope, float tone, float decay, float gallop, float noise, int seed)
		{
			int n = (int)(Rate * Length);
			float[] buf = new float[n];
			Random rng = new Random(seed);
			int pulses = (int)(Pulses * Length);
			int pulseLen = (int)(Rate * 0.08f);
			for (int k = 0; k < pulses; k++)
			{
				float t0 = k / Pulses + (k % 4 == 3 ? gallop : 0f);
				float swell = 1f - lope * 0.5f * (1f + Mathf.Cos(2f * Mathf.PI * 1.5f * t0));
				float amp = weights[k % weights.Length] * (0.35f + 0.65f * swell) * (0.9f + 0.2f * (float)rng.NextDouble());
				int start = (int)(t0 * Rate);
				float lp = 0f;
				for (int j = 0; j < pulseLen; j++)
				{
					float tt = j / (float)Rate;
					float env = Mathf.Exp(-tt / decay);
					lp += ((float)rng.NextDouble() * 2f - 1f - lp) * 0.08f;
					float v = Mathf.Sin(2f * Mathf.PI * tone * tt) + 0.45f * Mathf.Sin(2f * Mathf.PI * tone * 2f * tt + 0.6f) + noise * 6f * lp;
					buf[(start + j) % n] += amp * env * v;
				}
			}
			// Low-pass (the muffler), then a soft clip for some grit, normalized.
			float y = 0f;
			for (int pass = 0; pass < 2; pass++)
			{
				for (int i = 0; i < n; i++)
				{
					y += (buf[i] - y) * 0.18f;
					if (pass == 1)
					{
						buf[i] = y;
					}
				}
			}
			float peak = 0.0001f;
			for (int i = 0; i < n; i++)
			{
				buf[i] = (float)Math.Tanh(buf[i] * 1.6f);
				peak = Mathf.Max(peak, Mathf.Abs(buf[i]));
			}
			for (int i = 0; i < n; i++)
			{
				buf[i] *= 0.85f / peak;
			}
			return buf;
		}
	}
}
