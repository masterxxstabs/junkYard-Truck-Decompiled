using System;
using UnityEngine;

namespace ES3Types
{
	// Token: 0x02000303 RID: 771
	public class ES3UserType_TransformArray : ES3ArrayType
	{
		// Token: 0x06001412 RID: 5138 RVA: 0x000D475C File Offset: 0x000D295C
		public ES3UserType_TransformArray() : base(typeof(Transform[]), ES3UserType_Transform.Instance)
		{
			ES3UserType_TransformArray.Instance = this;
		}

		// Token: 0x0400246F RID: 9327
		public static ES3Type Instance;
	}
}
