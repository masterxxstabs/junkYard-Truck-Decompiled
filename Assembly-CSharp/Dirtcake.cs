using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class Dirtcake : MonoBehaviour
{
	// Token: 0x06000181 RID: 385 RVA: 0x00010EF0 File Offset: 0x0000F0F0
	private void Start()
	{
		this.rock1.SetActive(false);
		this.rock2.SetActive(false);
		this.rock3.SetActive(false);
		this.rock4.SetActive(false);
		this.rn = Random.Range(1, 6);
		switch (this.rn)
		{
		case 1:
			this.rock1.SetActive(true);
			break;
		case 2:
			this.rock2.SetActive(true);
			break;
		case 3:
			this.rock3.SetActive(true);
			break;
		case 4:
			this.rock4.SetActive(true);
			break;
		}
		if (this.rn < 5)
		{
			this.sphere.enabled = true;
			return;
		}
		this.sphere.enabled = false;
	}

	// Token: 0x04000466 RID: 1126
	public GameObject rock1;

	// Token: 0x04000467 RID: 1127
	public GameObject rock2;

	// Token: 0x04000468 RID: 1128
	public GameObject rock3;

	// Token: 0x04000469 RID: 1129
	public GameObject rock4;

	// Token: 0x0400046A RID: 1130
	public SphereCollider sphere;

	// Token: 0x0400046B RID: 1131
	private int rn;
}
