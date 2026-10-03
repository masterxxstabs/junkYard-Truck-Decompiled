using System;

namespace ES3Types
{
	// Token: 0x020002FD RID: 765
	public class ES3UserType_RadarDetectorArray : ES3ArrayType
	{
		// Token: 0x06001403 RID: 5123 RVA: 0x000D3EA8 File Offset: 0x000D20A8
		public ES3UserType_RadarDetectorArray() : base(typeof(RadarDetector[]), ES3UserType_RadarDetector.Instance)
		{
			ES3UserType_RadarDetectorArray.Instance = this;
		}

		// Token: 0x04002469 RID: 9321
		public static ES3Type Instance;
	}
}
