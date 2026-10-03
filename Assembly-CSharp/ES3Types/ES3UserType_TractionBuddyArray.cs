using System;

namespace ES3Types
{
	// Token: 0x02000301 RID: 769
	public class ES3UserType_TractionBuddyArray : ES3ArrayType
	{
		// Token: 0x0600140D RID: 5133 RVA: 0x000D4600 File Offset: 0x000D2800
		public ES3UserType_TractionBuddyArray() : base(typeof(TractionBuddy[]), ES3UserType_TractionBuddy.Instance)
		{
			ES3UserType_TractionBuddyArray.Instance = this;
		}

		// Token: 0x0400246D RID: 9325
		public static ES3Type Instance;
	}
}
