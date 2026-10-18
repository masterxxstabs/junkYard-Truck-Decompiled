using System;
using System.Collections.Generic;

namespace TruckPartsQOL
{
	// The shared shop list our mods publish to: AppDomain data
	// "JunkyardTruckMods.ShopProviders", a List<Func<object[][]>> where each item
	// is { GameObject prefab, string name, float price, string category }. Shops
	// (JunkAuto) read it; only mscorlib types, so no mod needs another's DLL.
	internal static class ShopProviders
	{
		private const string Key = "JunkyardTruckMods.ShopProviders";

		public static void Register(Func<object[][]> provider)
		{
			lock (typeof(AppDomain))
			{
				List<Func<object[][]>> list = AppDomain.CurrentDomain.GetData(Key) as List<Func<object[][]>>;
				if (list == null)
				{
					list = new List<Func<object[][]>>();
					AppDomain.CurrentDomain.SetData(Key, list);
				}
				list.Add(provider);
			}
		}
	}
}
