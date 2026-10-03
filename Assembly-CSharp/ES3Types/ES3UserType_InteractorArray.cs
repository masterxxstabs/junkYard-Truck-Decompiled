using System;

namespace ES3Types
{
	// Token: 0x020002E7 RID: 743
	public class ES3UserType_InteractorArray : ES3ArrayType
	{
		// Token: 0x060013CC RID: 5068 RVA: 0x000D07E8 File Offset: 0x000CE9E8
		public ES3UserType_InteractorArray() : base(typeof(Interactor[]), ES3UserType_Interactor.Instance)
		{
			ES3UserType_InteractorArray.Instance = this;
		}

		// Token: 0x04002453 RID: 9299
		public static ES3Type Instance;
	}
}
