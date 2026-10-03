using System;

namespace ES3Types
{
	// Token: 0x0200030D RID: 781
	public class ES3UserType_vaultringArray : ES3ArrayType
	{
		// Token: 0x0600142B RID: 5163 RVA: 0x000D9788 File Offset: 0x000D7988
		public ES3UserType_vaultringArray() : base(typeof(vaultring[]), ES3UserType_vaultring.Instance)
		{
			ES3UserType_vaultringArray.Instance = this;
		}

		// Token: 0x04002479 RID: 9337
		public static ES3Type Instance;
	}
}
