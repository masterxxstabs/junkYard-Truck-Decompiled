using System;
using UnityEngine;

namespace ES3Types
{
	// Token: 0x020002FF RID: 767
	public class ES3UserType_RigidbodyArray : ES3ArrayType
	{
		// Token: 0x06001408 RID: 5128 RVA: 0x000D44E0 File Offset: 0x000D26E0
		public ES3UserType_RigidbodyArray() : base(typeof(Rigidbody[]), ES3UserType_Rigidbody.Instance)
		{
			ES3UserType_RigidbodyArray.Instance = this;
		}

		// Token: 0x0400246B RID: 9323
		public static ES3Type Instance;
	}
}
