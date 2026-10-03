using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x02000351 RID: 849
	public class uTireWorldSpaceSynchronizer : MonoBehaviour
	{
		// Token: 0x060015BF RID: 5567 RVA: 0x000E1C8A File Offset: 0x000DFE8A
		private void Start()
		{
			this.synchronize();
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x000E1C94 File Offset: 0x000DFE94
		[ContextMenu("Synchronize uTireWorldSpaceBehaviour scripts")]
		public void synchronize()
		{
			if (this.templateBehavior == null)
			{
				Debug.LogWarning("(uTireWorldSpaceSynchronizer)No templateBehavior was asigned.", this);
				return;
			}
			if (this.uTireWorldSpace.Count == 0)
			{
				this.uTireWorldSpace = (from utire in base.GetComponentsInChildren<uTireWorldSpaceBehaviour>()
				where utire != this.templateBehavior
				select utire).ToList<uTireWorldSpaceBehaviour>();
			}
			foreach (uTireWorldSpaceBehaviour uTireWorldSpaceBehaviour in this.uTireWorldSpace)
			{
				uTireWorldSpaceBehaviour.pasteComponentData(this.templateBehavior);
			}
		}

		// Token: 0x0400265A RID: 9818
		[HideInInspector]
		[Tooltip("The settings of this behavior will be copied")]
		public uTireWorldSpaceBehaviour templateBehavior;

		// Token: 0x0400265B RID: 9819
		public List<uTireWorldSpaceBehaviour> uTireWorldSpace = new List<uTireWorldSpaceBehaviour>();
	}
}
