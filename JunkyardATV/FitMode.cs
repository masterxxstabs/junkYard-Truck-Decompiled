using System.Collections.Generic;
using UnityEngine;

namespace JunkyardATV
{
	// Line a model up in game: F7 next to an ATV. [ and ] pick what to adjust, arrow
	// keys and Page Up/Down move it (Shift = bigger steps), R reloads atv.cfg and
	// the model file, F7 again saves atv.cfg and refits every ATV.
	internal class FitMode
	{
		private static readonly string[] Targets = { "Model position", "Model rotation", "Model scale", "Wheel size", "Seat", "Engine", "Fuel inlet", "Engine turn", "Engine size" };

		private AtvVehicle atv;
		private int target;
		private bool wasKinematic;
		private readonly List<GameObject> markers = new List<GameObject>();

		public bool IsOpen
		{
			get { return atv != null; }
		}

		public void Open(AtvVehicle vehicle)
		{
			atv = vehicle;
			Rigidbody body = atv.GetComponent<Rigidbody>();
			wasKinematic = body.isKinematic;
			body.isKinematic = true; // hold still while fitting
			ShowMarkers();
		}

		public void Close(bool save)
		{
			if (atv == null)
			{
				return;
			}
			ClearMarkers();
			if (atv != null)
			{
				atv.GetComponent<Rigidbody>().isKinematic = wasKinematic;
			}
			atv = null;
			if (save)
			{
				AtvMod.Config.Save();
				foreach (AtvVehicle other in AtvVehicle.All)
				{
					other.BuildLook(AtvMod.Config);
				}
				AtvMod.Log("Saved the fit to atv.cfg.");
			}
		}

		public void Update()
		{
			if (atv == null)
			{
				return;
			}
			// Not Tab: that's likely the game's inventory key.
			if (Input.GetKeyDown(KeyCode.RightBracket))
			{
				target = (target + 1) % Targets.Length;
			}
			if (Input.GetKeyDown(KeyCode.LeftBracket))
			{
				target = (target + Targets.Length - 1) % Targets.Length;
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				AtvMod.Config = AtvConfig.Load();
				Rebuild();
				return;
			}
			bool big = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			Vector3 d = Vector3.zero;
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				d.x += 1f;
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				d.x -= 1f;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				d.z += 1f;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				d.z -= 1f;
			}
			if (Input.GetKeyDown(KeyCode.PageUp))
			{
				d.y += 1f;
			}
			if (Input.GetKeyDown(KeyCode.PageDown))
			{
				d.y -= 1f;
			}
			if (d == Vector3.zero)
			{
				return;
			}
			AtvConfig cfg = AtvMod.Config;
			switch (target)
			{
			case 0:
				cfg.offset += d * (big ? 0.05f : 0.01f);
				Rebuild();
				break;
			case 1:
				// Left/right turn it, up/down tip it nose up/down, PgUp/PgDn roll it.
				cfg.rotation += new Vector3(d.z, d.x, d.y) * (big ? 90f : 5f);
				Rebuild();
				break;
			case 2:
				cfg.scale = Mathf.Max(0.001f, cfg.scale * (1f + (d.z + d.y + d.x) * (big ? 0.1f : 0.01f)));
				Rebuild();
				break;
			case 3:
				if (cfg.wheelRadius <= 0f)
				{
					cfg.wheelRadius = 0.29f;
				}
				cfg.wheelRadius = Mathf.Max(0.05f, cfg.wheelRadius + (d.z + d.y + d.x) * (big ? 0.02f : 0.005f));
				Rebuild();
				break;
			case 7:
				// Same keys as model rotation.
				cfg.engineRotation += new Vector3(d.z, d.x, d.y) * (big ? 90f : 5f);
				atv.ApplyPoints(cfg);
				break;
			case 8:
				cfg.engineScale = Mathf.Max(0.05f, cfg.engineScale * (1f + (d.z + d.y + d.x) * (big ? 0.1f : 0.01f)));
				atv.ApplyPoints(cfg);
				break;
			default:
				string which = target == 4 ? "seat" : target == 5 ? "engine" : "fuel";
				Vector3 p = atv.PointOf(which) + d * (big ? 0.05f : 0.01f);
				if (target == 4)
				{
					cfg.seat = p;
				}
				else if (target == 5)
				{
					cfg.engine = p;
				}
				else
				{
					cfg.fuelInlet = p;
				}
				atv.ApplyPoints(cfg);
				ShowMarkers();
				break;
			}
		}

		private void Rebuild()
		{
			atv.BuildLook(AtvMod.Config);
			ShowMarkers();
		}

		private void ShowMarkers()
		{
			ClearMarkers();
			Marker("seat", new Color(0.2f, 1f, 0.3f));
			Marker("engine", new Color(1f, 0.3f, 0.2f));
			Marker("fuel", new Color(1f, 0.9f, 0.2f));
		}

		private void Marker(string which, Color color)
		{
			GameObject m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Object.DestroyImmediate(m.GetComponent<Collider>());
			m.transform.SetParent(atv.transform, false);
			m.transform.localPosition = atv.PointOf(which);
			m.transform.localScale = Vector3.one * 0.08f;
			m.GetComponent<Renderer>().material.color = color;
			markers.Add(m);
		}

		private void ClearMarkers()
		{
			foreach (GameObject m in markers)
			{
				if (m != null)
				{
					Object.Destroy(m);
				}
			}
			markers.Clear();
		}

		public void OnGUI()
		{
			if (atv == null)
			{
				return;
			}
			AtvConfig cfg = AtvMod.Config;
			Rect rect = new Rect(20f, 80f, 400f, 350f);
			GUI.Box(rect, "");
			GUI.Box(rect, "");
			GUILayout.BeginArea(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f));
			GUILayout.Label("<b>ATV fit mode</b>  (model: " + (cfg.model.Length > 0 ? cfg.model : "placeholder") + ")", Rich());
			for (int i = 0; i < Targets.Length; i++)
			{
				GUILayout.Label((i == target ? "<color=#ffd23f>> " : "   ") + Targets[i] + ": " + Value(i) + (i == target ? "</color>" : ""), Rich());
			}
			GUILayout.Space(6f);
			GUILayout.Label("[ ]: choose   Arrows / PgUp / PgDn: adjust (Shift: bigger)\nR: reload atv.cfg + model   F7: save and exit\nMarkers: green seat, red engine, yellow fuel inlet", Rich());
			GUILayout.EndArea();
		}

		private string Value(int i)
		{
			AtvConfig cfg = AtvMod.Config;
			switch (i)
			{
			case 0:
				return cfg.offset.ToString("F2");
			case 1:
				return cfg.rotation.ToString("F0");
			case 2:
				return cfg.scale.ToString("0.###");
			case 3:
				return cfg.wheelRadius > 0f ? cfg.wheelRadius.ToString("0.###") + " m" : "auto";
			case 4:
				return atv.PointOf("seat").ToString("F2");
			case 5:
				return atv.PointOf("engine").ToString("F2");
			case 7:
				return cfg.engineRotation.ToString("F0");
			case 8:
				Vector3 size = AtvEngine.PlacedSize(cfg.engineRotation, cfg.engineScale);
				return cfg.engineScale.ToString("0.###") + "  (" + size.x.ToString("0.00") + " x " + size.y.ToString("0.00") + " x " + size.z.ToString("0.00") + " m)";
			default:
				return atv.PointOf("fuel").ToString("F2");
			}
		}

		private static GUIStyle rich;

		private static GUIStyle Rich()
		{
			if (rich == null)
			{
				rich = new GUIStyle(GUI.skin.label);
				rich.richText = true;
				rich.fontSize = 14;
			}
			return rich;
		}
	}
}
