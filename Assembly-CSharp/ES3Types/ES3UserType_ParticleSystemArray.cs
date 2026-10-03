using System;
using UnityEngine;

namespace ES3Types
{
	// Token: 0x020002F9 RID: 761
	public class ES3UserType_ParticleSystemArray : ES3ArrayType
	{
		// Token: 0x060013F9 RID: 5113 RVA: 0x000D30F4 File Offset: 0x000D12F4
		public ES3UserType_ParticleSystemArray() : base(typeof(ParticleSystem[]), ES3UserType_ParticleSystem.Instance)
		{
			ES3UserType_ParticleSystemArray.Instance = this;
		}

		// Token: 0x04002465 RID: 9317
		public static ES3Type Instance;
	}
}
