using System;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001B4 RID: 436
	public class Sinoid : MonoBehaviour
	{
		// Token: 0x06000AA8 RID: 2728 RVA: 0x0008DC15 File Offset: 0x0008BE15
		private void Start()
		{
			this.startPos = base.transform.position;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0008DC28 File Offset: 0x0008BE28
		private void Update()
		{
			this.accuTime += Time.deltaTime;
			base.transform.position = this.startPos + Vector3.up * this.Amplitude * Mathf.Sin(this.accuTime * 2f * 3.1415927f * this.SineFreq);
			base.transform.Rotate((Vector3.up + Vector3.forward) * this.AngularVelocity * Time.deltaTime);
		}

		// Token: 0x04001CB9 RID: 7353
		public float AngularVelocity = 2f;

		// Token: 0x04001CBA RID: 7354
		public float SineFreq = 0.2f;

		// Token: 0x04001CBB RID: 7355
		public float Amplitude = 0.25f;

		// Token: 0x04001CBC RID: 7356
		private float accuTime;

		// Token: 0x04001CBD RID: 7357
		private Vector3 startPos;
	}
}
