using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EngineCloner
{
	// The game wires each vehicle to ONE specific engine block per type when the
	// level loads: car.enginescriptv8, GearBox.engineScriptV8, AudioControl,
	// FluidHandler, DrainOil, Diagnostic, Interactor.v8, plus GameObject fields for
	// parts inside the block (engineFan, engineCrank, ...). A cloned block mounted
	// in a truck is never consulted: the truck keeps asking the original, which is
	// not in a truck, so it reports canRun = false and the truck won't start.
	//
	// Swap() exchanges every such reference between two blocks of the same type, so
	// the game treats the other block as "its" engine. Blocks made by Instantiate
	// have identical hierarchies, so every part and component is matched by its
	// position in the hierarchy.
	internal static class ReferenceSwapper
	{
		private const BindingFlags InstanceFields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private const BindingFlags StaticFields = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private static readonly Dictionary<Type, List<FieldInfo>> fieldCache = new Dictionary<Type, List<FieldInfo>>();

		public static int Swap(GameObject a, GameObject b)
		{
			Dictionary<Object, Object> map = new Dictionary<Object, Object>();
			MapHierarchy(a.transform, b.transform, map);
			int swapped = 0;
			foreach (MonoBehaviour behaviour in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
			{
				if (behaviour == null || !behaviour.gameObject.scene.IsValid())
				{
					continue; // destroyed, or a prefab asset rather than a scene object
				}
				Transform t = behaviour.transform;
				// The blocks' own scripts already point at their own parts.
				if (t.IsChildOf(a.transform) || t.IsChildOf(b.transform))
				{
					continue;
				}
				foreach (FieldInfo field in ObjectFields(behaviour.GetType()))
				{
					swapped += SwapField(field, behaviour, map);
				}
			}
			foreach (FieldInfo field in GameStaticFields())
			{
				swapped += SwapField(field, null, map);
			}
			return swapped;
		}

		// Map both directions: a's parts -> b's parts and b's parts -> a's parts.
		private static void MapHierarchy(Transform a, Transform b, Dictionary<Object, Object> map)
		{
			Pair(a.gameObject, b.gameObject, map);
			Component[] ca = a.GetComponents<Component>();
			Component[] cb = b.GetComponents<Component>();
			for (int i = 0; i < ca.Length && i < cb.Length; i++)
			{
				if (ca[i] != null && cb[i] != null && ca[i].GetType() == cb[i].GetType())
				{
					Pair(ca[i], cb[i], map);
				}
			}
			for (int i = 0; i < a.childCount && i < b.childCount; i++)
			{
				Transform childA = a.GetChild(i);
				Transform childB = b.GetChild(i);
				if (childA.name == childB.name)
				{
					MapHierarchy(childA, childB, map);
				}
			}
		}

		private static void Pair(Object x, Object y, Dictionary<Object, Object> map)
		{
			map[x] = y;
			map[y] = x;
		}

		private static int SwapField(FieldInfo field, object owner, Dictionary<Object, Object> map)
		{
			Type type = field.FieldType;
			object value;
			try
			{
				value = field.GetValue(owner);
			}
			catch
			{
				return 0;
			}
			if (value == null)
			{
				return 0;
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				Object replacement;
				if (map.TryGetValue((Object)value, out replacement) && type.IsInstanceOfType(replacement))
				{
					field.SetValue(owner, replacement);
					return 1;
				}
				return 0;
			}
			// Arrays and List<T> of Unity objects, e.g. GameObject[] or List<Transform>.
			IList list = value as IList;
			if (list == null)
			{
				return 0;
			}
			Type element = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
			int swapped = 0;
			for (int i = 0; i < list.Count; i++)
			{
				Object item = list[i] as Object;
				Object replacement;
				if (!ReferenceEquals(item, null) && map.TryGetValue(item, out replacement) && element.IsInstanceOfType(replacement))
				{
					list[i] = replacement;
					swapped++;
				}
			}
			return swapped;
		}

		// Instance fields that can hold Unity objects, including inherited private ones.
		private static List<FieldInfo> ObjectFields(Type type)
		{
			List<FieldInfo> fields;
			if (fieldCache.TryGetValue(type, out fields))
			{
				return fields;
			}
			fields = new List<FieldInfo>();
			for (Type t = type; t != null && t != typeof(MonoBehaviour); t = t.BaseType)
			{
				foreach (FieldInfo field in t.GetFields(InstanceFields))
				{
					if (CanHoldObject(field.FieldType))
					{
						fields.Add(field);
					}
				}
			}
			fieldCache[type] = fields;
			return fields;
		}

		private static List<FieldInfo> staticFields;

		private static List<FieldInfo> GameStaticFields()
		{
			if (staticFields != null)
			{
				return staticFields;
			}
			staticFields = new List<FieldInfo>();
			Type[] types;
			try
			{
				types = typeof(engine).Assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types;
			}
			foreach (Type type in types)
			{
				if (type == null || type.ContainsGenericParameters)
				{
					continue;
				}
				foreach (FieldInfo field in type.GetFields(StaticFields))
				{
					if (!field.IsLiteral && CanHoldObject(field.FieldType))
					{
						staticFields.Add(field);
					}
				}
			}
			return staticFields;
		}

		private static bool CanHoldObject(Type type)
		{
			if (typeof(Object).IsAssignableFrom(type))
			{
				return true;
			}
			if (type.IsArray)
			{
				return typeof(Object).IsAssignableFrom(type.GetElementType());
			}
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && typeof(Object).IsAssignableFrom(type.GetGenericArguments()[0]);
		}
	}
}
