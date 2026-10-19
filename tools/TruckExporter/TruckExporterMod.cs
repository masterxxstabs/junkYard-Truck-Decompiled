using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(TruckExporter.TruckExporterMod), "Truck Exporter", "1.0.0", "masterxxstabs")]
[assembly: MelonGame(null, null)]

namespace TruckExporter
{
	// A modding tool, not a gameplay mod: writes the game's vehicles (the
	// Diamondback, the F100, the AMC, the golf cart, the dirt bike) to
	// <game>/UserData/TruckExport/, one folder each, so they can be worked on in
	// Blender: every part as its own OBJ group, its materials and textures, and a
	// hierarchy.txt with every object's name, position, components, joints and
	// the vehicle script's fields. Runs once a few seconds after a save loads;
	// press F11 to export again.
	public class TruckExporterMod : MelonMod
	{
		private static readonly string[] VehicleScripts = { "car", "car4", "car3", "golfcart", "Dirtbike" };

		private float exportAt = -1f;
		private bool exported;

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			exportAt = Time.realtimeSinceStartup + 8f;
		}

		public override void OnUpdate()
		{
			bool auto = !exported && exportAt > 0f && Time.realtimeSinceStartup > exportAt;
			if (!auto && !Input.GetKeyDown(KeyCode.F11))
			{
				return;
			}
			exportAt = -1f;
			List<GameObject> vehicles = FindVehicles();
			if (vehicles.Count == 0)
			{
				if (!auto)
				{
					LoggerInstance.Msg("No vehicles in this scene; load a save first.");
				}
				return;
			}
			exported = true;
			string root = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.dataPath), "UserData"), "TruckExport");
			Directory.CreateDirectory(root);
			Exporter exporter = new Exporter(Path.Combine(root, "textures"));
			HashSet<string> used = new HashSet<string>();
			foreach (GameObject v in vehicles)
			{
				string name = Exporter.Clean(v.name);
				while (!used.Add(name))
				{
					name += "_2";
				}
				try
				{
					string report = exporter.Export(v, Path.Combine(root, name));
					LoggerInstance.Msg("Exported " + v.name + ": " + report);
				}
				catch (Exception e)
				{
					LoggerInstance.Error("Couldn't export " + v.name + ": " + e);
				}
			}
			LoggerInstance.Msg("Done. Zip the folder " + root + " and send it over.");
		}

		// Every vehicle the game has, including inactive ones (not yet found or
		// bought), but not copies other mods made (they share the original's name).
		private static List<GameObject> FindVehicles()
		{
			List<GameObject> list = new List<GameObject>();
			HashSet<string> names = new HashSet<string>();
			foreach (string script in VehicleScripts)
			{
				Type type = null;
				foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (a.GetName().Name == "Assembly-CSharp")
					{
						type = a.GetType(script, false);
						break;
					}
				}
				if (type == null)
				{
					continue;
				}
				foreach (Object o in Resources.FindObjectsOfTypeAll(type))
				{
					Component c = o as Component;
					if (c == null || !c.gameObject.scene.IsValid())
					{
						continue;
					}
					if (names.Add(script + "|" + c.gameObject.name))
					{
						list.Add(c.gameObject);
					}
				}
			}
			return list;
		}
	}
}
