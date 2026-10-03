using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace TruckStereo
{
	// Each folder <game>/TruckStereo/CD1, CD2, ... is one CD, played in file-name
	// order. Like My Summer Car's CD folders.
	internal static class MusicLibrary
	{
		public const int MaxCds = 9;

		private static readonly string[] Extensions = { ".ogg", ".wav", ".mp3" };

		private static List<AudioClip> radioClips;

		private static AudioClip staticClip;

		public static string Root
		{
			get { return Path.Combine(Path.GetDirectoryName(Application.dataPath), "TruckStereo"); }
		}

		public static string CdFolder(int number)
		{
			return Path.Combine(Root, "CD" + number);
		}

		public static void EnsureFolders()
		{
			try
			{
				for (int i = 1; i <= 3; i++)
				{
					Directory.CreateDirectory(CdFolder(i));
				}
				string readme = Path.Combine(Root, "README.txt");
				if (!File.Exists(readme))
				{
					File.WriteAllText(readme, "Put music files in CD1, CD2, ... (up to CD" + MaxCds + "). Each folder is one CD,\r\nplayed in file-name order. Use .ogg or .wav; .mp3 only works if your Unity\r\nversion can decode it, so convert to .ogg if a track won't play.\r\nBuy the CD at the stereo shop (F9) to get a disc for that folder.\r\n");
				}
			}
			catch (Exception e)
			{
				TruckStereoMod.Log("Could not create music folders: " + e.Message);
			}
		}

		public static List<string> Tracks(int cd)
		{
			List<string> tracks = new List<string>();
			string folder = CdFolder(cd);
			if (!Directory.Exists(folder))
			{
				return tracks;
			}
			foreach (string file in Directory.GetFiles(folder))
			{
				if (Array.IndexOf(Extensions, Path.GetExtension(file).ToLowerInvariant()) >= 0)
				{
					tracks.Add(file);
				}
			}
			tracks.Sort(StringComparer.OrdinalIgnoreCase);
			return tracks;
		}

		public static IEnumerator Load(string path, Action<AudioClip> done)
		{
			AudioType type;
			switch (Path.GetExtension(path).ToLowerInvariant())
			{
			case ".wav":
				type = AudioType.WAV;
				break;
			case ".mp3":
				type = AudioType.MPEG;
				break;
			default:
				type = AudioType.OGGVORBIS;
				break;
			}
			UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(new Uri(path).AbsoluteUri, type);
			yield return request.SendWebRequest();
			AudioClip clip = null;
			if (string.IsNullOrEmpty(request.error))
			{
				try
				{
					clip = DownloadHandlerAudioClip.GetContent(request);
				}
				catch (Exception e)
				{
					TruckStereoMod.Log("Can't decode " + Path.GetFileName(path) + ": " + e.Message);
				}
			}
			else
			{
				TruckStereoMod.Log("Can't load " + Path.GetFileName(path) + ": " + request.error);
			}
			request.Dispose();
			if (clip != null && clip.length <= 0f)
			{
				TruckStereoMod.Log("Can't play " + Path.GetFileName(path) + " (unsupported format? try .ogg).");
				UnityEngine.Object.Destroy(clip);
				clip = null;
			}
			if (clip != null)
			{
				clip.name = Path.GetFileNameWithoutExtension(path);
			}
			done(clip);
		}

		// FM radio plays the songs the game ships for its own radios.
		public static List<AudioClip> RadioClips()
		{
			if (radioClips != null && radioClips.Count > 0)
			{
				return radioClips;
			}
			radioClips = new List<AudioClip>();
			foreach (Radio radio in Resources.FindObjectsOfTypeAll<Radio>())
			{
				if (radio.soundtrack == null)
				{
					continue;
				}
				foreach (AudioClip clip in radio.soundtrack)
				{
					if (clip != null && !radioClips.Contains(clip))
					{
						radioClips.Add(clip);
					}
				}
			}
			return radioClips;
		}

		// Between stations, or when the game has no radio songs loaded.
		public static AudioClip Static()
		{
			if (staticClip == null)
			{
				const int rate = 22050;
				float[] samples = new float[rate * 2];
				System.Random random = new System.Random(1);
				for (int i = 0; i < samples.Length; i++)
				{
					samples[i] = (float)(random.NextDouble() * 2.0 - 1.0) * 0.25f;
				}
				staticClip = AudioClip.Create("static", samples.Length, 1, rate, false);
				staticClip.SetData(samples, 0);
			}
			return staticClip;
		}

		public static void ClearCache()
		{
			radioClips = null;
		}
	}
}
