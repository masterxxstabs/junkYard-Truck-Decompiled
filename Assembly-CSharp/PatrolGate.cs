using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class PatrolGate : MonoBehaviour
{
	// Token: 0x06000714 RID: 1812 RVA: 0x0005B0B3 File Offset: 0x000592B3
	public void Open()
	{
		if (!this.isOpen)
		{
			this.anim.Play("Open");
			base.GetComponent<AudioSource>().Play();
			this.isOpen = true;
		}
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0005B0E0 File Offset: 0x000592E0
	public void Close()
	{
		if (this.isOpen)
		{
			this.anim.Play("Close");
			base.GetComponent<AudioSource>().Play();
			this.isOpen = false;
		}
	}

	// Token: 0x04000FED RID: 4077
	public Animation anim;

	// Token: 0x04000FEE RID: 4078
	private bool isOpen;
}
