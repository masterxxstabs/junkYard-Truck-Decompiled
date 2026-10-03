using System;

namespace ES3Types
{
	// Token: 0x020002EB RID: 747
	public class ES3UserType_JakeArray : ES3ArrayType
	{
		// Token: 0x060013D6 RID: 5078 RVA: 0x000D1A70 File Offset: 0x000CFC70
		public ES3UserType_JakeArray() : base(typeof(Jake[]), ES3UserType_Jake.Instance)
		{
			ES3UserType_JakeArray.Instance = this;
		}

		// Token: 0x04002457 RID: 9303
		public static ES3Type Instance;
	}
}
