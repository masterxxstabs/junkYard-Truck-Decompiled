using System;
using ES3Internal;

namespace ES3Types
{
	// Token: 0x020002E1 RID: 737
	public class ES3UserType_ES3PrefabArray : ES3ArrayType
	{
		// Token: 0x060013BD RID: 5053 RVA: 0x000CCEB0 File Offset: 0x000CB0B0
		public ES3UserType_ES3PrefabArray() : base(typeof(ES3Prefab[]), ES3UserType_ES3Prefab.Instance)
		{
			ES3UserType_ES3PrefabArray.Instance = this;
		}

		// Token: 0x0400244D RID: 9293
		public static ES3Type Instance;
	}
}
