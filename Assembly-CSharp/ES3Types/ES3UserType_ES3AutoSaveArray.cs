using System;

namespace ES3Types
{
	// Token: 0x020002DF RID: 735
	public class ES3UserType_ES3AutoSaveArray : ES3ArrayType
	{
		// Token: 0x060013B8 RID: 5048 RVA: 0x000CCE08 File Offset: 0x000CB008
		public ES3UserType_ES3AutoSaveArray() : base(typeof(ES3AutoSave[]), ES3UserType_ES3AutoSave.Instance)
		{
			ES3UserType_ES3AutoSaveArray.Instance = this;
		}

		// Token: 0x0400244B RID: 9291
		public static ES3Type Instance;
	}
}
