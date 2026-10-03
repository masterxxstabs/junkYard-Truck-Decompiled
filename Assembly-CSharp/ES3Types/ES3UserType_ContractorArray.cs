using System;

namespace ES3Types
{
	// Token: 0x020002DB RID: 731
	public class ES3UserType_ContractorArray : ES3ArrayType
	{
		// Token: 0x060013AE RID: 5038 RVA: 0x000CCC74 File Offset: 0x000CAE74
		public ES3UserType_ContractorArray() : base(typeof(Contractor[]), ES3UserType_Contractor.Instance)
		{
			ES3UserType_ContractorArray.Instance = this;
		}

		// Token: 0x04002447 RID: 9287
		public static ES3Type Instance;
	}
}
