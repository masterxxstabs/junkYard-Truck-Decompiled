using System;

namespace ES3Types
{
	// Token: 0x020002FB RID: 763
	public class ES3UserType_PickUpArray : ES3ArrayType
	{
		// Token: 0x060013FE RID: 5118 RVA: 0x000D3DBC File Offset: 0x000D1FBC
		public ES3UserType_PickUpArray() : base(typeof(PickUp[]), ES3UserType_PickUp.Instance)
		{
			ES3UserType_PickUpArray.Instance = this;
		}

		// Token: 0x04002467 RID: 9319
		public static ES3Type Instance;
	}
}
