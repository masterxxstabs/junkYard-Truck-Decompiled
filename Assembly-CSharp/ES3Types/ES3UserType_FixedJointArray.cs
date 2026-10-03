using System;
using UnityEngine;

namespace ES3Types
{
	// Token: 0x020002E5 RID: 741
	public class ES3UserType_FixedJointArray : ES3ArrayType
	{
		// Token: 0x060013C7 RID: 5063 RVA: 0x000CD338 File Offset: 0x000CB538
		public ES3UserType_FixedJointArray() : base(typeof(FixedJoint[]), ES3UserType_FixedJoint.Instance)
		{
			ES3UserType_FixedJointArray.Instance = this;
		}

		// Token: 0x04002451 RID: 9297
		public static ES3Type Instance;
	}
}
