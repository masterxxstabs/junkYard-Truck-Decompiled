using System;
using System.Globalization;
using UnityEngine;

namespace TruckPartsQOL
{
	// A rock light: a small LED pod that lights the ground under and around a
	// truck. Fitted like the speakers (on any surface of a vehicle, facing out of
	// it, two bolts), so under the body, in the wheel wells, on the frame... It
	// shines only while fitted, in the color from the settings (RockLightColor),
	// and the rock light key switches them all on and off.
	public class RockLight : MonoBehaviour
	{
		// Settings, from the mod (TruckPartsQOLMod).
		public static Color Color = UnityEngine.Color.white;
		public static float Brightness = 1f;
		public static bool On = true;

		private Light glow;
		private Renderer lens;
		private Material lensMaterial;

		private void Awake()
		{
			Transform g = transform.Find("Glow");
			glow = g != null ? g.GetComponent<Light>() : null;
			Transform l = transform.Find("Lens");
			lens = l != null ? l.GetComponent<Renderer>() : null;
			if (lens != null)
			{
				lensMaterial = lens.material;
				lensMaterial.EnableKeyword("_EMISSION");
			}
		}

		private void Update()
		{
			TruckPart part = GetComponent<TruckPart>();
			bool lit = On && part != null && part.installed;
			if (glow != null)
			{
				glow.enabled = lit;
				glow.color = Color;
				glow.intensity = 2.2f * Brightness;
			}
			if (lensMaterial != null)
			{
				lensMaterial.color = lit ? Color.Lerp(Color, UnityEngine.Color.white, 0.6f) : new Color(0.75f, 0.75f, 0.75f);
				lensMaterial.SetColor("_EmissionColor", lit ? Color * (1.5f * Brightness) : UnityEngine.Color.black);
			}
		}

		public static bool ParseColor(string s, out Color c)
		{
			c = UnityEngine.Color.white;
			s = (s ?? "").Trim().TrimStart('#');
			int rgb;
			if (s.Length != 6 || !int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out rgb))
			{
				return false;
			}
			c = new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f);
			return true;
		}
	}
}
