using System;

namespace ES3Types
{
	// Token: 0x0200030B RID: 779
	public class ES3UserType_jiggsArray : ES3ArrayType
	{
		// Token: 0x06001426 RID: 5158 RVA: 0x000D969C File Offset: 0x000D789C
		public ES3UserType_jiggsArray() : base(typeof(jiggs[]), ES3UserType_jiggs.Instance)
		{
			ES3UserType_jiggsArray.Instance = this;
		}

		// Token: 0x04002477 RID: 9335
		public static ES3Type Instance;
	}
}
