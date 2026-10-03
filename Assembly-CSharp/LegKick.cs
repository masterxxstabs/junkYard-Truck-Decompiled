using System;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class LegKick : MonoBehaviour
{
	// Token: 0x060005C7 RID: 1479 RVA: 0x000475DB File Offset: 0x000457DB
	public void KickOpen()
	{
		this.anim.Play();
		this.aSource.clip = this.clip[0];
		this.aSource.Play();
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00047607 File Offset: 0x00045807
	public void KickClose()
	{
		this.anim.Play();
		this.aSource.clip = this.clip[1];
		this.aSource.Play();
	}

	// Token: 0x04000C89 RID: 3209
	public Animation anim;

	// Token: 0x04000C8A RID: 3210
	public AudioSource aSource;

	// Token: 0x04000C8B RID: 3211
	public AudioClip[] clip;
}
