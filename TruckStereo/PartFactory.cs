using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TruckStereo
{
	// Builds the stereo parts out of Unity primitives so the mod needs no asset
	// files. Every part faces local +Z (the side you look at once it's installed).
	internal static class PartFactory
	{
		private static readonly Color Black = new Color(0.05f, 0.05f, 0.05f);
		private static readonly Color DarkGrey = new Color(0.15f, 0.15f, 0.16f);
		private static readonly Color Grey = new Color(0.35f, 0.35f, 0.37f);
		private static readonly Color Silver = new Color(0.75f, 0.75f, 0.78f);
		private static readonly Color Carpet = new Color(0.1f, 0.1f, 0.11f);

		private static Font font;

		// parent: an inactive parent makes a dormant template (nothing runs until a
		// copy is Instantiated), which is what the Parts Store needs.
		public static StereoPart Create(PartKind kind, int cdNumber, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			GameObject root = new GameObject("TS_" + kind + (kind == PartKind.CD ? cdNumber.ToString() : ""));
			if (parent != null)
			{
				root.transform.SetParent(parent, false);
			}
			root.transform.position = position;
			root.transform.rotation = rotation;
			StereoPart part = root.AddComponent<StereoPart>();
			part.kind = kind;
			part.cdNumber = cdNumber;
			Vector3 size;
			switch (kind)
			{
			case PartKind.HeadUnit:
				size = BuildHeadUnit(root.transform);
				part.mass = 1.5f;
				root.AddComponent<HeadUnit>();
				break;
			case PartKind.Speaker:
				size = BuildSpeaker(root.transform);
				part.mass = 1f;
				AddSpeakerAudio(root, false);
				break;
			case PartKind.Subwoofer:
				size = BuildSubwoofer(root.transform);
				part.mass = 14f;
				AddSpeakerAudio(root, true);
				break;
			default:
				size = BuildCd(root.transform, cdNumber);
				part.mass = 0.1f;
				break;
			}
			part.halfDepth = size.z / 2f;
			part.halfHeight = size.y / 2f;
			// PickUp needs a BoxCollider on the root and a Rigidbody.
			BoxCollider box = root.AddComponent<BoxCollider>();
			box.size = size;
			Rigidbody body = root.AddComponent<Rigidbody>();
			body.mass = part.mass;
			body.interpolation = RigidbodyInterpolation.Interpolate;
			PickUp pick = root.AddComponent<PickUp>();
			InitLikeUnity(pick);
			pick.pickable = true;
			pick.price = 0f;
			pick.trueMass = part.mass;
			return part;
		}

		// A component added from code starts with null strings and arrays, but every
		// PickUp in the game was loaded from a scene, where Unity fills them with ""
		// and empty arrays. PickUp's code expects the scene version (e.g. it checks
		// attachTo != "" to decide whether the item snaps into a slot).
		private static void InitLikeUnity(Component component)
		{
			for (Type type = component.GetType(); type != null && type != typeof(MonoBehaviour); type = type.BaseType)
			{
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
				{
					bool serialized = field.IsPublic && !field.IsNotSerialized || field.IsDefined(typeof(SerializeField), false);
					if (!serialized || field.GetValue(component) != null)
					{
						continue;
					}
					Type fieldType = field.FieldType;
					if (fieldType == typeof(string))
					{
						field.SetValue(component, "");
					}
					else if (fieldType.IsArray)
					{
						field.SetValue(component, Array.CreateInstance(fieldType.GetElementType(), 0));
					}
					else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
					{
						field.SetValue(component, Activator.CreateInstance(fieldType));
					}
				}
			}
		}

		private static void AddSpeakerAudio(GameObject root, bool sub)
		{
			AudioSource source = root.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 1f;
			source.dopplerLevel = 0f;
			source.rolloffMode = AudioRolloffMode.Logarithmic;
			source.minDistance = sub ? 2.5f : 1.5f;
			source.maxDistance = sub ? 60f : 40f;
			if (sub)
			{
				root.AddComponent<AudioLowPassFilter>().cutoffFrequency = 160f;
			}
			else
			{
				root.AddComponent<AudioHighPassFilter>().cutoffFrequency = 90f;
			}
		}

		// 1-DIN head unit: 18 x 5 cm face, 16 cm deep.
		private static Vector3 BuildHeadUnit(Transform root)
		{
			Vector3 size = new Vector3(0.18f, 0.05f, 0.16f);
			Box(root, "Chassis", size, Vector3.zero, Grey);
			Box(root, "Faceplate", new Vector3(0.182f, 0.052f, 0.01f), new Vector3(0f, 0f, 0.08f), Black);
			GameObject screen = Box(root, "Screen", new Vector3(0.1f, 0.018f, 0.002f), new Vector3(0.015f, 0.008f, 0.0855f), new Color(0.02f, 0.12f, 0.12f));
			Glow(screen, new Color(0.02f, 0.25f, 0.25f));
			Box(root, "CdSlot", new Vector3(0.12f, 0.003f, 0.002f), new Vector3(0.015f, -0.013f, 0.0855f), new Color(0f, 0f, 0f));
			GameObject knob = Cylinder(root, "Knob", 0.024f, 0.008f, new Vector3(-0.065f, 0f, 0.088f), DarkGrey);
			knob.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			for (int i = 0; i < 4; i++)
			{
				Box(root, "Button" + i, new Vector3(0.016f, 0.006f, 0.004f), new Vector3(-0.025f + i * 0.02f, -0.019f, 0.087f), DarkGrey);
			}
			Text(root, "Display", new Vector3(0.015f, 0.008f, 0.0868f), 0.0016f, new Color(0.3f, 1f, 0.95f));
			return size;
		}

		// 6.5" coaxial door speaker.
		private static Vector3 BuildSpeaker(Transform root)
		{
			Vector3 size = new Vector3(0.17f, 0.17f, 0.07f);
			GameObject basket = Cylinder(root, "Basket", 0.17f, 0.05f, new Vector3(0f, 0f, -0.01f), Black);
			basket.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			GameObject cone = Cylinder(root, "Cone", 0.14f, 0.01f, new Vector3(0f, 0f, 0.02f), DarkGrey);
			cone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			GameObject cap = Cylinder(root, "DustCap", 0.045f, 0.01f, new Vector3(0f, 0f, 0.03f), Silver);
			cap.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			return size;
		}

		// 12" sub in a carpeted box.
		private static Vector3 BuildSubwoofer(Transform root)
		{
			Vector3 size = new Vector3(0.45f, 0.35f, 0.35f);
			Box(root, "Enclosure", size, Vector3.zero, Carpet);
			GameObject surround = Cylinder(root, "Surround", 0.3f, 0.01f, new Vector3(0f, 0f, 0.176f), Black);
			surround.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			GameObject cone = Cylinder(root, "Cone", 0.25f, 0.01f, new Vector3(0f, 0f, 0.18f), DarkGrey);
			cone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			GameObject cap = Cylinder(root, "DustCap", 0.08f, 0.01f, new Vector3(0f, 0f, 0.184f), Grey);
			cap.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			return size;
		}

		// Jewel case with the disc visible through the front.
		private static Vector3 BuildCd(Transform root, int number)
		{
			Vector3 size = new Vector3(0.142f, 0.125f, 0.01f);
			Box(root, "Case", size, Vector3.zero, new Color(0.2f, 0.2f, 0.22f));
			GameObject disc = Cylinder(root, "Disc", 0.118f, 0.002f, new Vector3(0f, 0f, 0.005f), Silver);
			disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			TextMesh label = Text(root, "Label", new Vector3(0f, 0f, 0.0065f), 0.004f, Color.black);
			label.text = "CD " + number;
			return size;
		}

		private static GameObject Box(Transform parent, string name, Vector3 size, Vector3 localPosition, Color color)
		{
			GameObject go = Primitive(PrimitiveType.Cube, parent, name, color);
			go.transform.localPosition = localPosition;
			go.transform.localScale = size;
			return go;
		}

		// Unity's cylinder is 1 wide and 2 tall along Y.
		private static GameObject Cylinder(Transform parent, string name, float diameter, float height, Vector3 localPosition, Color color)
		{
			GameObject go = Primitive(PrimitiveType.Cylinder, parent, name, color);
			go.transform.localPosition = localPosition;
			go.transform.localScale = new Vector3(diameter, height / 2f, diameter);
			return go;
		}

		private static GameObject Primitive(PrimitiveType type, Transform parent, string name, Color color)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			go.name = name;
			// One BoxCollider on the root covers the whole part.
			UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
			go.transform.SetParent(parent, false);
			// The primitive's default material always exists in a build, unlike
			// Shader.Find, which fails for shaders the game doesn't include.
			go.GetComponent<Renderer>().material.color = color;
			return go;
		}

		private static void Glow(GameObject go, Color color)
		{
			Material material = go.GetComponent<Renderer>().material;
			material.EnableKeyword("_EMISSION");
			material.SetColor("_EmissionColor", color);
		}

		private static TextMesh Text(Transform parent, string name, Vector3 localPosition, float characterSize, Color color)
		{
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent, false);
			go.transform.localPosition = localPosition;
			// TextMesh reads toward +Z; turn it to face out of the part's front.
			go.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			TextMesh text = go.AddComponent<TextMesh>();
			text.anchor = TextAnchor.MiddleCenter;
			text.alignment = TextAlignment.Center;
			text.fontSize = 64;
			text.characterSize = characterSize;
			text.color = color;
			Font f = GetFont();
			if (f != null)
			{
				text.font = f;
				go.GetComponent<MeshRenderer>().material = f.material;
			}
			return text;
		}

		private static Font GetFont()
		{
			if (font != null)
			{
				return font;
			}
			// The built-in font was renamed in Unity 2022.2.
			foreach (string name in new[] { "Arial.ttf", "LegacyRuntime.ttf" })
			{
				try
				{
					font = Resources.GetBuiltinResource<Font>(name);
				}
				catch (Exception)
				{
				}
				if (font != null)
				{
					break;
				}
			}
			return font;
		}
	}
}
