using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class Blender : MonoBehaviour
{
	// Token: 0x06000058 RID: 88 RVA: 0x00004B93 File Offset: 0x00002D93
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.rend.material = this.material1;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00004BB4 File Offset: 0x00002DB4
	private void Update()
	{
		float t = Mathf.PingPong(Time.time, this.duration) / this.duration;
		this.rend.material.Lerp(this.material1, this.material2, t);
	}

	// Token: 0x040000DF RID: 223
	public Material material1;

	// Token: 0x040000E0 RID: 224
	public Material material2;

	// Token: 0x040000E1 RID: 225
	private float duration = 2f;

	// Token: 0x040000E2 RID: 226
	private Renderer rend;
}
