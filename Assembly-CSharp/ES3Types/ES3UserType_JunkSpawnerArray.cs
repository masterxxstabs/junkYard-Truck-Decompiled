using System;

namespace ES3Types
{
	// Token: 0x020002EF RID: 751
	public class ES3UserType_JunkSpawnerArray : ES3ArrayType
	{
		// Token: 0x060013E0 RID: 5088 RVA: 0x000D2210 File Offset: 0x000D0410
		public ES3UserType_JunkSpawnerArray() : base(typeof(JunkSpawner[]), ES3UserType_JunkSpawner.Instance)
		{
			ES3UserType_JunkSpawnerArray.Instance = this;
		}

		// Token: 0x0400245B RID: 9307
		public static ES3Type Instance;
	}
}
