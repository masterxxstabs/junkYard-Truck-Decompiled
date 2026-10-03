using System;

namespace ES3Types
{
	// Token: 0x020002E9 RID: 745
	public class ES3UserType_InventoryItemsArray : ES3ArrayType
	{
		// Token: 0x060013D1 RID: 5073 RVA: 0x000D0CB4 File Offset: 0x000CEEB4
		public ES3UserType_InventoryItemsArray() : base(typeof(InventoryItems[]), ES3UserType_InventoryItems.Instance)
		{
			ES3UserType_InventoryItemsArray.Instance = this;
		}

		// Token: 0x04002455 RID: 9301
		public static ES3Type Instance;
	}
}
