using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TruckPartsQOL
{
	// Plays a CD or the FM radio through every speaker installed in the same
	// vehicle. With no speakers you only get the tinny built-in one.
	public class HeadUnit : MonoBehaviour
	{
		public const int ModeCd = 0;

		public const int ModeRadio = 1;

		// Public so they're saved and copied when a vehicle is cloned.
		public bool powerOn;

		public float volume = 0.6f;

		public int mode = ModeCd;

		public int insertedCd;

		public int track;

		private AudioSource internalSpeaker;

		private TextMesh display;

		private AudioClip clip;

		private bool ownsClip;

		private bool loading;

		private int loadToken;

		private bool playing;

		private float volumeShownUntil;

		private string status = "";

		private float stationFreq;

		// Radio: which song the current station is on.
		private int radioSong;

		// Don't hammer the disk when a CD folder is empty or a track won't load.
		private float retryAt;

		private void Awake()
		{
			// A head unit copied with a cloned vehicle already has these.
			internalSpeaker = GetComponent<AudioSource>();
			if (internalSpeaker == null)
			{
				internalSpeaker = gameObject.AddComponent<AudioSource>();
			}
			internalSpeaker.playOnAwake = false;
			internalSpeaker.spatialBlend = 1f;
			internalSpeaker.dopplerLevel = 0f;
			internalSpeaker.minDistance = 0.3f;
			internalSpeaker.maxDistance = 6f;
			if (GetComponent<AudioHighPassFilter>() == null)
			{
				gameObject.AddComponent<AudioHighPassFilter>().cutoffFrequency = 600f;
			}
			Transform t = transform.Find("Display");
			if (t != null)
			{
				display = t.GetComponent<TextMesh>();
			}
		}

		private TruckPart Part
		{
			get { return GetComponent<TruckPart>(); }
		}

		// Installed in a vehicle whose battery can run accessories.
		private bool HasPower()
		{
			TruckPart part = Part;
			if (part == null || !part.installed)
			{
				return false;
			}
			GameObject vehicle = part.Vehicle;
			return vehicle != null && VehiclePower.CanRunAccessories(vehicle);
		}

		private List<AudioSource> Speakers(out List<TruckPart> parts)
		{
			List<AudioSource> sources = new List<AudioSource>();
			parts = new List<TruckPart>();
			GameObject vehicle = Part != null ? Part.Vehicle : null;
			if (vehicle == null)
			{
				return sources;
			}
			foreach (TruckPart other in TruckPart.All)
			{
				if (other.IsSpeaker && other.installed && other.Vehicle == vehicle)
				{
					AudioSource source = other.GetComponent<AudioSource>();
					if (source != null)
					{
						sources.Add(source);
						parts.Add(other);
					}
				}
			}
			return sources;
		}

		private void Update()
		{
			bool powered = powerOn && HasPower();
			if (!powered)
			{
				StopPlayback();
				SetDisplay("");
				return;
			}
			if (mode == ModeCd && insertedCd == 0)
			{
				StopPlayback();
				SetDisplay(Volume("NO DISC"));
				return;
			}
			if (clip == null && !loading && Time.time >= retryAt)
			{
				StartTrack();
			}
			if (loading || clip == null)
			{
				SetDisplay(Volume(status));
				return;
			}
			// A track that finished on its own: next one.
			if (playing && !internalSpeaker.isPlaying && Application.isFocused)
			{
				if (mode == ModeRadio)
				{
					// Same station, next song.
					radioSong++;
					StopPlayback();
				}
				else
				{
					Next();
				}
				return;
			}
			List<TruckPart> speakerParts;
			List<AudioSource> speakers = Speakers(out speakerParts);
			internalSpeaker.volume = speakers.Count == 0 ? volume * 0.15f : 0f;
			int drift = clip.frequency / 12;
			for (int i = 0; i < speakers.Count; i++)
			{
				AudioSource source = speakers[i];
				speakerParts[i].drivenFrame = Time.frameCount;
				source.volume = volume * (speakerParts[i].kind == PartKind.Subwoofer ? 1.2f : 1f);
				if (source.clip != clip || !source.isPlaying)
				{
					source.clip = clip;
					source.timeSamples = internalSpeaker.timeSamples;
					source.Play();
				}
				else if (Mathf.Abs(source.timeSamples - internalSpeaker.timeSamples) > drift)
				{
					source.timeSamples = internalSpeaker.timeSamples;
				}
			}
			if (mode == ModeCd)
			{
				SetDisplay(Volume("CD" + insertedCd + " T" + (track + 1).ToString("00")));
			}
			else
			{
				SetDisplay(Volume("FM " + stationFreq.ToString("0.0")));
			}
		}

		private string Volume(string text)
		{
			return Time.time < volumeShownUntil ? "VOL " + Mathf.RoundToInt(volume * 30f) : text;
		}

		private void SetDisplay(string text)
		{
			if (display != null && display.text != text)
			{
				display.text = text;
			}
		}

		private void StartTrack()
		{
			StopPlayback();
			if (mode == ModeRadio)
			{
				List<AudioClip> stations = MusicLibrary.RadioClips();
				AudioClip station = stations.Count > 0 ? stations[Mathf.Abs(track + radioSong) % stations.Count] : MusicLibrary.Static();
				stationFreq = 88.1f + (Mathf.Abs(track) * 3.7f) % 19.8f;
				// Tuning in mid-song, like a real radio.
				Play(station, false, Random.Range(0, Mathf.Max(1, station.samples - station.frequency * 20)));
				return;
			}
			List<string> tracks = MusicLibrary.Tracks(insertedCd);
			if (tracks.Count == 0)
			{
				status = "NO TRACKS";
				retryAt = Time.time + 2f;
				return;
			}
			track = (track % tracks.Count + tracks.Count) % tracks.Count;
			loading = true;
			status = "LOADING";
			int token = ++loadToken;
			StartCoroutine(MusicLibrary.Load(tracks[track], delegate(AudioClip loaded)
			{
				if (token != loadToken || this == null)
				{
					if (loaded != null)
					{
						Destroy(loaded);
					}
					return;
				}
				loading = false;
				if (loaded == null)
				{
					// Skip the unplayable track after a moment.
					status = "READ ERROR";
					track++;
					retryAt = Time.time + 1.5f;
					return;
				}
				Play(loaded, true, 0);
			}));
		}

		private void Play(AudioClip newClip, bool owned, int startSample)
		{
			clip = newClip;
			ownsClip = owned;
			internalSpeaker.clip = clip;
			internalSpeaker.timeSamples = Mathf.Clamp(startSample, 0, Mathf.Max(0, clip.samples - 1));
			internalSpeaker.loop = !owned && clip == MusicLibrary.Static();
			internalSpeaker.Play();
			playing = true;
		}

		private void StopPlayback()
		{
			playing = false;
			loading = false;
			loadToken++;
			internalSpeaker.Stop();
			internalSpeaker.clip = null;
			if (clip != null && ownsClip)
			{
				Destroy(clip);
			}
			clip = null;
			ownsClip = false;
			if (status != "NO TRACKS" && status != "READ ERROR")
			{
				status = "";
			}
		}

		// Controls. Called by the mod when the player presses a key at this unit.

		public void TogglePower()
		{
			powerOn = !powerOn;
			if (!powerOn)
			{
				StopPlayback();
			}
		}

		public void Next()
		{
			track++;
			radioSong = 0;
			retryAt = 0f;
			StopPlayback();
		}

		public void Previous()
		{
			track--;
			radioSong = 0;
			retryAt = 0f;
			StopPlayback();
		}

		public void ToggleMode()
		{
			mode = mode == ModeCd ? ModeRadio : ModeCd;
			track = 0;
			StopPlayback();
		}

		public void ChangeVolume(float delta)
		{
			volume = Mathf.Clamp01(volume + delta);
			volumeShownUntil = Time.time + 1.5f;
		}

		public bool InsertCd(int number)
		{
			if (insertedCd != 0)
			{
				return false;
			}
			insertedCd = number;
			track = 0;
			if (mode == ModeCd)
			{
				StopPlayback();
			}
			return true;
		}

		// Returns the disc number that came out (0 if none).
		public int EjectCd()
		{
			int number = insertedCd;
			insertedCd = 0;
			if (mode == ModeCd)
			{
				StopPlayback();
			}
			return number;
		}

		private void OnDisable()
		{
			StopPlayback();
		}
	}

	// Is the vehicle's battery up? The game's car scripts keep a "canAcc" flag
	// (accessories allowed); if a vehicle has none, assume it has power.
	internal static class VehiclePower
	{
		private static readonly Dictionary<System.Type, System.Reflection.FieldInfo> fields = new Dictionary<System.Type, System.Reflection.FieldInfo>();

		public static bool CanRunAccessories(GameObject vehicle)
		{
			foreach (MonoBehaviour behaviour in vehicle.GetComponents<MonoBehaviour>())
			{
				if (behaviour == null)
				{
					continue;
				}
				System.Type type = behaviour.GetType();
				System.Reflection.FieldInfo field;
				if (!fields.TryGetValue(type, out field))
				{
					field = type.GetField("canAcc", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
					if (field != null && field.FieldType != typeof(bool))
					{
						field = null;
					}
					fields[type] = field;
				}
				if (field != null)
				{
					return (bool)field.GetValue(behaviour);
				}
			}
			return true;
		}
	}
}
