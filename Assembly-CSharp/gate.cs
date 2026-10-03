using System;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class gate : MonoBehaviour
{
	// Token: 0x06000984 RID: 2436 RVA: 0x0007FD3E File Offset: 0x0007DF3E
	private void Start()
	{
		this.anim = base.GetComponent<Animation>();
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x0007FD4C File Offset: 0x0007DF4C
	public void SetOpen()
	{
		if (!this.isOpen)
		{
			this.anim.Play("Open");
			base.GetComponent<AudioSource>().Play();
		}
		else
		{
			this.anim.Play("Close");
			base.GetComponent<AudioSource>().Play();
		}
		this.isOpen = !this.isOpen;
	}

	// Token: 0x0400196D RID: 6509
	private Animation anim;

	// Token: 0x0400196E RID: 6510
	private bool soundEnable;

	// Token: 0x0400196F RID: 6511
	private bool isOpen = true;
}
