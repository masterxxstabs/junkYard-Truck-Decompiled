using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class DemoCarMotion : MonoBehaviour
{
	// Token: 0x0600015C RID: 348 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
	private void FixedUpdate()
	{
		this.lPulley.Rotate(Vector3.forward * 1000f * Time.deltaTime);
		this.dFan.Rotate(Vector3.forward * 1000f * Time.deltaTime);
		this.rPulley.Rotate(Vector3.forward * 1000f * Time.deltaTime);
		this.engine.Rotate(Vector3.forward * 0.5f * Mathf.Sin(Time.time * 50f));
	}

	// Token: 0x040003B2 RID: 946
	public Transform lPulley;

	// Token: 0x040003B3 RID: 947
	public Transform rPulley;

	// Token: 0x040003B4 RID: 948
	public Transform dFan;

	// Token: 0x040003B5 RID: 949
	public GameObject emitter1;

	// Token: 0x040003B6 RID: 950
	public GameObject flame1;

	// Token: 0x040003B7 RID: 951
	public Transform engine;
}
