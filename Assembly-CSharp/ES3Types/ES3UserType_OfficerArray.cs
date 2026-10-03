using System;

namespace ES3Types
{
	// Token: 0x020002F7 RID: 759
	public class ES3UserType_OfficerArray : ES3ArrayType
	{
		// Token: 0x060013F4 RID: 5108 RVA: 0x000D304C File Offset: 0x000D124C
		public ES3UserType_OfficerArray() : base(typeof(Officer[]), ES3UserType_Officer.Instance)
		{
			ES3UserType_OfficerArray.Instance = this;
		}

		// Token: 0x04002463 RID: 9315
		public static ES3Type Instance;
	}
}
