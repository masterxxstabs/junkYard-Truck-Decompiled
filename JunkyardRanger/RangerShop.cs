using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JunkyardRanger
{
	// Sold in JunkAuto (the mods' shared shop list) and in the Junkyard Terminal's
	// Parts Store (ComputerPartStore) when it's installed. What's sold is a dormant
	// marker: the shop puts it where it delivers, and it orders a Ranger there.
	internal static class RangerShop
	{
		public const string Name = "Vehicle - 2004 Ford Ranger";
		private const string Category = "Truck Part";

		private static GameObject holder;
		private static GameObject marker;
		private static FieldInfo prefabField, nameField, priceField, categoryField;
		private static Type entryType;
		private static bool hooked;

		private static GameObject Marker()
		{
			if (marker == null)
			{
				holder = new GameObject("JunkyardRanger_Shop");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
				marker = new GameObject("Ranger delivery");
				marker.transform.SetParent(holder.transform, false);
				marker.AddComponent<RangerDelivery>();
			}
			return marker;
		}

		public static object[][] ShopItems()
		{
			return new[] { new object[] { Marker(), Name, RangerMod.Price, Category } };
		}

		public static void HookStore(HarmonyLib.Harmony harmony)
		{
			if (hooked)
			{
				return;
			}
			Type store = null;
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				store = a.GetType("ComputerFeatures.PartsStoreService", false);
				if (store != null)
				{
					break;
				}
			}
			if (store == null)
			{
				return;
			}
			MethodInfo getCatalog = store.GetMethod("GetCatalog", BindingFlags.Static | BindingFlags.Public);
			entryType = store.GetNestedType("PartEntry", BindingFlags.Public | BindingFlags.NonPublic);
			if (getCatalog == null || entryType == null)
			{
				return;
			}
			prefabField = entryType.GetField("prefab");
			nameField = entryType.GetField("displayName");
			priceField = entryType.GetField("price");
			categoryField = entryType.GetField("category");
			if (prefabField == null || nameField == null || priceField == null || categoryField == null)
			{
				return;
			}
			harmony.Patch(getCatalog, null, new HarmonyLib.HarmonyMethod(typeof(RangerShop).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)));
			hooked = true;
			RangerMod.Log("The Ranger is sold in the Parts Store.");
		}

		private static void Postfix(object __result)
		{
			IList list = __result as IList;
			if (list == null)
			{
				return;
			}
			try
			{
				foreach (object entry in list)
				{
					if ((string)nameField.GetValue(entry) == Name)
					{
						prefabField.SetValue(entry, Marker());
						priceField.SetValue(entry, RangerMod.Price);
						return;
					}
				}
				object e = Activator.CreateInstance(entryType);
				prefabField.SetValue(e, Marker());
				nameField.SetValue(e, Name);
				priceField.SetValue(e, RangerMod.Price);
				categoryField.SetValue(e, Category);
				list.Add(e);
			}
			catch (Exception ex)
			{
				RangerMod.Log("Couldn't list the Ranger in the Parts Store: " + ex.Message);
			}
		}
	}

	// What a shop delivers: orders a Ranger where it lands.
	public class RangerDelivery : MonoBehaviour
	{
		private void Start()
		{
			RangerBuilder.Order(transform.position);
			Destroy(gameObject);
		}
	}
}
