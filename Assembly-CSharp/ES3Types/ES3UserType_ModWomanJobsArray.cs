using System;

namespace ES3Types
{
	// Token: 0x020002F5 RID: 757
	public class ES3UserType_ModWomanJobsArray : ES3ArrayType
	{
		// Token: 0x060013EF RID: 5103 RVA: 0x000D2A2C File Offset: 0x000D0C2C
		public ES3UserType_ModWomanJobsArray() : base(typeof(ModWomanJobs[]), ES3UserType_ModWomanJobs.Instance)
		{
			ES3UserType_ModWomanJobsArray.Instance = this;
		}

		// Token: 0x04002461 RID: 9313
		public static ES3Type Instance;
	}
}
