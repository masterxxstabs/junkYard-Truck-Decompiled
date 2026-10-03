using System;

namespace ES3Types
{
	// Token: 0x020002DD RID: 733
	public class ES3UserType_DiagnosticArray : ES3ArrayType
	{
		// Token: 0x060013B3 RID: 5043 RVA: 0x000CCD60 File Offset: 0x000CAF60
		public ES3UserType_DiagnosticArray() : base(typeof(Diagnostic[]), ES3UserType_Diagnostic.Instance)
		{
			ES3UserType_DiagnosticArray.Instance = this;
		}

		// Token: 0x04002449 RID: 9289
		public static ES3Type Instance;
	}
}
