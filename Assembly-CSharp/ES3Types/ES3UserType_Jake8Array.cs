using System;

namespace ES3Types
{
	// Token: 0x020002ED RID: 749
	public class ES3UserType_Jake8Array : ES3ArrayType
	{
		// Token: 0x060013DB RID: 5083 RVA: 0x000D1EE8 File Offset: 0x000D00E8
		public ES3UserType_Jake8Array() : base(typeof(Jake8[]), ES3UserType_Jake8.Instance)
		{
			ES3UserType_Jake8Array.Instance = this;
		}

		// Token: 0x04002459 RID: 9305
		public static ES3Type Instance;
	}
}
