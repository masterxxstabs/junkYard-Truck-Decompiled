using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ControllerSupport
{
	// The game's input object (ControlRef: your key bindings, and the axes the
	// vehicles drive from) and the player (Interactor), found by reflection so this
	// mod doesn't depend on a particular game build.
	internal static class Game
	{
		private static Type controlRefType;
		private static Type interactorType;
		private static FieldInfo drivingCar;
		private static FieldInfo person;
		private static readonly Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

		public static object Controls;
		private static Object interactor;
		private static float nextControls;
		private static float nextInteractor;

		public static Type ControlRefType
		{
			get
			{
				if (controlRefType == null)
				{
					controlRefType = AccessTools.TypeByName("ControlRef");
				}
				return controlRefType;
			}
		}

		public static Type InteractorType
		{
			get
			{
				if (interactorType == null)
				{
					interactorType = AccessTools.TypeByName("Interactor");
					if (interactorType != null)
					{
						drivingCar = AccessTools.Field(interactorType, "drivingCar");
						person = AccessTools.Field(interactorType, "person");
					}
				}
				return interactorType;
			}
		}

		private static FieldInfo Field(string name)
		{
			FieldInfo f;
			if (!fields.TryGetValue(name, out f))
			{
				f = ControlRefType != null ? AccessTools.Field(ControlRefType, name) : null;
				fields[name] = f;
			}
			return f;
		}

		private static object ControlsNow()
		{
			Object o = Controls as Object;
			if (o == null && ControlRefType != null && Time.unscaledTime >= nextControls)
			{
				nextControls = Time.unscaledTime + 1f;
				Controls = Object.FindObjectOfType(ControlRefType);
			}
			return Controls as Object != null ? Controls : null;
		}

		// The key the game uses for one of its controls (Jump, Interact, Item2...).
		public static KeyCode Key(string control)
		{
			object c = ControlsNow();
			FieldInfo f = Field(control);
			if (c == null || f == null || f.FieldType != typeof(KeyCode))
			{
				return KeyCode.None;
			}
			return (KeyCode)f.GetValue(c);
		}

		public static float GetFloat(object controls, string name)
		{
			FieldInfo f = Field(name);
			return f != null ? (float)f.GetValue(controls) : 0f;
		}

		public static void SetFloat(object controls, string name, float value)
		{
			FieldInfo f = Field(name);
			if (f != null)
			{
				f.SetValue(controls, value);
			}
		}

		public static bool GetBool(object controls, string name)
		{
			FieldInfo f = Field(name);
			return f != null && (bool)f.GetValue(controls);
		}

		private static Object Interactor()
		{
			if (interactor == null && InteractorType != null && Time.unscaledTime >= nextInteractor)
			{
				nextInteractor = Time.unscaledTime + 1f;
				interactor = Object.FindObjectOfType(InteractorType);
			}
			return interactor;
		}

		// In a vehicle (truck, car, golf cart, dirt bike...).
		public static bool Driving()
		{
			Object i = Interactor();
			return i != null && drivingCar != null && (bool)drivingCar.GetValue(i);
		}

		// How fast the vehicle you're in is going forward (m/s, negative backwards):
		// along the seat you sit in, which faces the way the vehicle drives.
		public static float ForwardSpeed()
		{
			Object i = Interactor();
			if (i == null || person == null)
			{
				return 0f;
			}
			GameObject p = person.GetValue(i) as GameObject;
			if (p == null || p.transform.parent == null)
			{
				return 0f;
			}
			Transform seat = p.transform.parent;
			Rigidbody rb = seat.GetComponentInParent<Rigidbody>();
			if (rb == null)
			{
				return 0f;
			}
			return Vector3.Dot(rb.velocity, seat.forward);
		}
	}
}
