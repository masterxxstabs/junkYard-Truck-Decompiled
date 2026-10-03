using System;

namespace ES3Types
{
	// Token: 0x020002E3 RID: 739
	public class ES3UserType_FiremanArray : ES3ArrayType
	{
		// Token: 0x060013C2 RID: 5058 RVA: 0x000CD254 File Offset: 0x000CB454
		public ES3UserType_FiremanArray() : base(typeof(Fireman[]), ES3UserType_Fireman.Instance)
		{
			ES3UserType_FiremanArray.Instance = this;
		}

		// Token: 0x0400244F RID: 9295
		public static ES3Type Instance;
	}
}
