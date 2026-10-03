using System;
using UnityEngine;

namespace ES3Types
{
	// Token: 0x020002F3 RID: 755
	public class ES3UserType_MeshRendererArray : ES3ArrayType
	{
		// Token: 0x060013EA RID: 5098 RVA: 0x000D23EC File Offset: 0x000D05EC
		public ES3UserType_MeshRendererArray() : base(typeof(MeshRenderer[]), ES3UserType_MeshRenderer.Instance)
		{
			ES3UserType_MeshRendererArray.Instance = this;
		}

		// Token: 0x0400245F RID: 9311
		public static ES3Type Instance;
	}
}
