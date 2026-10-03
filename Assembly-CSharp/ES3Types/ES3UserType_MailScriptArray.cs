using System;

namespace ES3Types
{
	// Token: 0x020002F1 RID: 753
	public class ES3UserType_MailScriptArray : ES3ArrayType
	{
		// Token: 0x060013E5 RID: 5093 RVA: 0x000D2300 File Offset: 0x000D0500
		public ES3UserType_MailScriptArray() : base(typeof(MailScript[]), ES3UserType_MailScript.Instance)
		{
			ES3UserType_MailScriptArray.Instance = this;
		}

		// Token: 0x0400245D RID: 9309
		public static ES3Type Instance;
	}
}
