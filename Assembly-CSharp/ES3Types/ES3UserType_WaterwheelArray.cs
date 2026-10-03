using System;

namespace ES3Types
{
	// Token: 0x02000305 RID: 773
	public class ES3UserType_WaterwheelArray : ES3ArrayType
	{
		// Token: 0x06001417 RID: 5143 RVA: 0x000D4948 File Offset: 0x000D2B48
		public ES3UserType_WaterwheelArray() : base(typeof(Waterwheel[]), ES3UserType_Waterwheel.Instance)
		{
			ES3UserType_WaterwheelArray.Instance = this;
		}

		// Token: 0x04002471 RID: 9329
		public static ES3Type Instance;
	}
}
