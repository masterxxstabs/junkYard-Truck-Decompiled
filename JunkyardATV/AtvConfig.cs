using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace JunkyardATV
{
	// <game>/JunkyardATV/atv.cfg: which model to use and how it fits.
	internal class AtvConfig
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		public string model = "";
		public float scale = 1f;
		public Vector3 rotation = Vector3.zero;
		public Vector3 offset = Vector3.zero;
		public string wheelNames = "wheel,tire,tyre,rim";
		public float wheelRadius;
		public string handlebars = "";
		public string hide = "";
		// Positions on the ATV (meters, +Z forward, +Y up). NaN = work it out.
		public Vector3 seat = Auto;
		public Vector3 engine = Auto;
		public Vector3 fuelInlet = Auto;
		public float mass = 280f;

		public static readonly Vector3 Auto = new Vector3(float.NaN, float.NaN, float.NaN);

		public static bool IsAuto(Vector3 v)
		{
			return float.IsNaN(v.x);
		}

		public static string Folder
		{
			get { return Path.Combine(Path.GetDirectoryName(Application.dataPath), "JunkyardATV"); }
		}

		private static string FilePath
		{
			get { return Path.Combine(Folder, "atv.cfg"); }
		}

		public string ModelPath
		{
			get { return model.Length > 0 ? Path.Combine(Folder, model) : null; }
		}

		// The Suzuki Quadzilla 500 that ships with the mod, rigged by
		// tools/rig_quadzilla.py: already in meters, Y up and facing forward, with
		// its wheels and handlebars as separate parts; these points were measured
		// from it.
		private const string Quadzilla = "quadzilla.obj";

		private void UseQuadzilla()
		{
			model = Quadzilla;
			scale = 1f;
			rotation = Vector3.zero;
			offset = Vector3.zero;
			wheelRadius = 0f;
			handlebars = "Handlebars";
			seat = new Vector3(0f, 0.82f, -0.19f);
			engine = new Vector3(0f, 0.29f, -0.06f);
			fuelInlet = new Vector3(0f, 0.87f, 0.19f);
		}

		public static AtvConfig Load()
		{
			AtvConfig c = new AtvConfig();
			Directory.CreateDirectory(Folder);
			bool haveQuadzilla = File.Exists(Path.Combine(Folder, Quadzilla));
			if (!File.Exists(FilePath))
			{
				if (haveQuadzilla)
				{
					c.UseQuadzilla();
				}
				c.Save();
				return c;
			}
			foreach (string raw in File.ReadAllLines(FilePath))
			{
				string line = raw.Trim();
				int eq = line.IndexOf('=');
				if (line.Length == 0 || line[0] == '#' || eq < 0)
				{
					continue;
				}
				string key = line.Substring(0, eq).Trim();
				string value = line.Substring(eq + 1).Trim();
				try
				{
					switch (key)
					{
					case "model":
						c.model = value;
						break;
					case "scale":
						c.scale = F(value);
						break;
					case "rotation":
						c.rotation = V(value);
						break;
					case "offset":
						c.offset = V(value);
						break;
					case "wheelNames":
						c.wheelNames = value;
						break;
					case "wheelRadius":
						c.wheelRadius = F(value);
						break;
					case "handlebars":
						c.handlebars = value;
						break;
					case "hide":
						c.hide = value;
						break;
					case "seat":
						c.seat = V(value);
						break;
					case "engine":
						c.engine = V(value);
						break;
					case "fuelInlet":
						c.fuelInlet = V(value);
						break;
					case "mass":
						c.mass = F(value);
						break;
					}
				}
				catch (Exception e)
				{
					AtvMod.Log("atv.cfg: bad value for " + key + " (" + value + "): " + e.Message);
				}
			}
			// No model chosen yet (e.g. a config from the placeholder days), but the
			// Quadzilla is there: use it.
			if (c.model.Length == 0 && haveQuadzilla)
			{
				c.UseQuadzilla();
				c.Save();
				AtvMod.Log("Using the Suzuki Quadzilla 500 model.");
			}
			return c;
		}

		public void Save()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Junkyard ATV model settings. Press F7 next to an ATV to fit it in game.");
			sb.AppendLine("#");
			sb.AppendLine("# model: a .glb, .gltf or .obj file in this folder (empty = built-in placeholder).");
			sb.AppendLine("#   Name the wheel parts so they contain one of wheelNames; each corner's");
			sb.AppendLine("#   parts are grouped automatically and spin and steer.");
			sb.AppendLine("# scale / rotation (degrees x,y,z) / offset (meters x,y,z): fit the model so");
			sb.AppendLine("#   its front points forward (+Z) and the wheels sit on the ground.");
			sb.AppendLine("# seat / engine / fuelInlet: x,y,z on the ATV, or auto.");
			sb.AppendLine("# handlebars: name of the part that turns with the steering.");
			sb.AppendLine("# hide: names of parts to hide, comma-separated.");
			sb.AppendLine();
			sb.AppendLine("model=" + model);
			sb.AppendLine("scale=" + scale.ToString("R", Inv));
			sb.AppendLine("rotation=" + S(rotation));
			sb.AppendLine("offset=" + S(offset));
			sb.AppendLine("wheelNames=" + wheelNames);
			sb.AppendLine("wheelRadius=" + wheelRadius.ToString("R", Inv));
			sb.AppendLine("handlebars=" + handlebars);
			sb.AppendLine("hide=" + hide);
			sb.AppendLine("seat=" + S(seat));
			sb.AppendLine("engine=" + S(engine));
			sb.AppendLine("fuelInlet=" + S(fuelInlet));
			sb.AppendLine("mass=" + mass.ToString("R", Inv));
			try
			{
				Directory.CreateDirectory(Folder);
				File.WriteAllText(FilePath, sb.ToString());
			}
			catch (Exception e)
			{
				AtvMod.Log("Couldn't write atv.cfg: " + e.Message);
			}
		}

		public List<string> HideList()
		{
			return Split(hide);
		}

		public List<string> WheelNameList()
		{
			return Split(wheelNames.ToLowerInvariant());
		}

		private static List<string> Split(string s)
		{
			List<string> list = new List<string>();
			foreach (string part in s.Split(','))
			{
				if (part.Trim().Length > 0)
				{
					list.Add(part.Trim());
				}
			}
			return list;
		}

		private static float F(string s)
		{
			return float.Parse(s, NumberStyles.Float, Inv);
		}

		private static Vector3 V(string s)
		{
			if (s.Trim().ToLowerInvariant() == "auto")
			{
				return Auto;
			}
			string[] p = s.Split(',');
			return new Vector3(F(p[0]), F(p[1]), F(p[2]));
		}

		private static string S(Vector3 v)
		{
			if (IsAuto(v))
			{
				return "auto";
			}
			return v.x.ToString("0.###", Inv) + "," + v.y.ToString("0.###", Inv) + "," + v.z.ToString("0.###", Inv);
		}
	}
}
