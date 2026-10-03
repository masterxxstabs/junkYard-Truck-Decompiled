using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class TireDeformation : MonoBehaviour
{
	// Token: 0x0600086A RID: 2154 RVA: 0x0006E65C File Offset: 0x0006C85C
	private void Update()
	{
		for (int i = 0; i < this.tires.Count; i++)
		{
			WheelHit wheelHit = default(WheelHit);
			bool groundHit = this.tires[i].wheelCollider.GetGroundHit(out wheelHit);
			float y = 0f;
			float x = 0f;
			if (groundHit)
			{
				y = 1f - Mathf.Clamp01((-this.tires[i].wheelCollider.transform.InverseTransformPoint(wheelHit.point).y - this.tires[i].wheelCollider.radius) / this.tires[i].wheelCollider.suspensionDistance);
				x = Mathf.Clamp(wheelHit.sidewaysSlip, -1f, 1f);
			}
			this.tires[i].wheelMaterial.SetVector("_SlideDirPressure", new Vector4(x, y, 0f, 0f));
		}
	}

	// Token: 0x04001387 RID: 4999
	public List<TireDeformation.Tire> tires;

	// Token: 0x02000425 RID: 1061
	[Serializable]
	public class Tire
	{
		// Token: 0x04002957 RID: 10583
		public WheelCollider wheelCollider;

		// Token: 0x04002958 RID: 10584
		public Material wheelMaterial;
	}
}
