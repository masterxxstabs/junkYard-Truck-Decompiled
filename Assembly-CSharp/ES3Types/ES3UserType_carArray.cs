using System;

namespace ES3Types
{
	// Token: 0x02000307 RID: 775
	public class ES3UserType_carArray : ES3ArrayType
	{
		// Token: 0x0600141C RID: 5148 RVA: 0x000D7C00 File Offset: 0x000D5E00
		public ES3UserType_carArray() : base(typeof(car[]), ES3UserType_car.Instance)
		{
			ES3UserType_carArray.Instance = this;
		}

		// Token: 0x04002473 RID: 9331
		public static ES3Type Instance;
	}
}
