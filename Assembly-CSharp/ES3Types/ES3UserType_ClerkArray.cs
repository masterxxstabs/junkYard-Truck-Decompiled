using System;

namespace ES3Types
{
	// Token: 0x020002D9 RID: 729
	public class ES3UserType_ClerkArray : ES3ArrayType
	{
		// Token: 0x060013A9 RID: 5033 RVA: 0x000CCB88 File Offset: 0x000CAD88
		public ES3UserType_ClerkArray() : base(typeof(Clerk[]), ES3UserType_Clerk.Instance)
		{
			ES3UserType_ClerkArray.Instance = this;
		}

		// Token: 0x04002445 RID: 9285
		public static ES3Type Instance;
	}
}
