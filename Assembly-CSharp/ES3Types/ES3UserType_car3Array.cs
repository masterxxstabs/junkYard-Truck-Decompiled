using System;

namespace ES3Types
{
	// Token: 0x02000309 RID: 777
	public class ES3UserType_car3Array : ES3ArrayType
	{
		// Token: 0x06001421 RID: 5153 RVA: 0x000D9494 File Offset: 0x000D7694
		public ES3UserType_car3Array() : base(typeof(car3[]), ES3UserType_car3.Instance)
		{
			ES3UserType_car3Array.Instance = this;
		}

		// Token: 0x04002475 RID: 9333
		public static ES3Type Instance;
	}
}
