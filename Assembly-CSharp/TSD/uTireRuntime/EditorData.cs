using System;
using System.Linq;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034D RID: 845
	[Serializable]
	public class EditorData
	{
		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060015B6 RID: 5558 RVA: 0x000E1A01 File Offset: 0x000DFC01
		// (set) Token: 0x060015B7 RID: 5559 RVA: 0x000E1A09 File Offset: 0x000DFC09
		public int selectedMeshSide { get; set; }

		// Token: 0x060015B8 RID: 5560 RVA: 0x000E1A14 File Offset: 0x000DFC14
		public void Init(Bounds meshBounds)
		{
			float[] arr = new float[]
			{
				meshBounds.extents.x,
				meshBounds.extents.y,
				meshBounds.extents.z
			};
			this.selectedMeshSide = arr.Select((float axis, int index) => new
			{
				axis,
				index
			}).First(element => element.axis == Mathf.Max(arr)).index;
		}
	}
}
