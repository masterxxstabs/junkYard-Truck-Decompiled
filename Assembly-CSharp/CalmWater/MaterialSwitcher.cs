using System;
using UnityEngine;

namespace CalmWater
{
	// Token: 0x0200032D RID: 813
	public class MaterialSwitcher : MonoBehaviour
	{
		// Token: 0x060014CD RID: 5325 RVA: 0x000DCE48 File Offset: 0x000DB048
		private void Start()
		{
			this.m = this.WaterPlane.GetComponent<MirrorReflection>();
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x000DCE5B File Offset: 0x000DB05B
		public void SetDX11Mat()
		{
			this.WaterPlane.material = this.DX11Mat;
			this.m.setMaterial();
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x000DCE79 File Offset: 0x000DB079
		public void SetClassicMat()
		{
			this.WaterPlane.material = this.ClassicMat;
			this.m.setMaterial();
		}

		// Token: 0x04002543 RID: 9539
		public MeshRenderer WaterPlane;

		// Token: 0x04002544 RID: 9540
		public Material ClassicMat;

		// Token: 0x04002545 RID: 9541
		public Material DX11Mat;

		// Token: 0x04002546 RID: 9542
		private MirrorReflection m;
	}
}
