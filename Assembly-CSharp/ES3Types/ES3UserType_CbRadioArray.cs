using System;

namespace ES3Types
{
	// Token: 0x020002D7 RID: 727
	public class ES3UserType_CbRadioArray : ES3ArrayType
	{
		// Token: 0x060013A4 RID: 5028 RVA: 0x000CCA9C File Offset: 0x000CAC9C
		public ES3UserType_CbRadioArray() : base(typeof(CbRadio[]), ES3UserType_CbRadio.Instance)
		{
			ES3UserType_CbRadioArray.Instance = this;
		}

		// Token: 0x04002443 RID: 9283
		public static ES3Type Instance;
	}
}
