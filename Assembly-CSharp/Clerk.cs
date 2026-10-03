using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class Clerk : MonoBehaviour
{
	// Token: 0x06000135 RID: 309 RVA: 0x0000E93F File Offset: 0x0000CB3F
	private void Start()
	{
		if (this.jobNum == 1)
		{
			this.lostkeys.SetActive(true);
		}
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000E958 File Offset: 0x0000CB58
	public void Interact()
	{
		if (this.jobNum == 1)
		{
			if (Vector3.Distance(base.transform.position, this.lostkeys.transform.position) < 2.5f)
			{
				this.jobNum = 2;
				this.Say(5);
				this.lostkeys.SetActive(false);
				this.mc.CompleteMission(69);
				return;
			}
		}
		else if (this.jobNum == 0)
		{
			this.Say(4);
			this.mc.ActivateMission(69);
			this.jobNum = 1;
			this.lostkeys.SetActive(true);
			this.mc.ActivateMission(69);
			this.anim.Play("clerkjob2", 0, 0f);
		}
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000EA0F File Offset: 0x0000CC0F
	public void Entering()
	{
		if (Random.Range(0, 6) == 0 && this.jobNum > 1)
		{
			this.Say(3);
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000EA2A File Offset: 0x0000CC2A
	public void Exiting()
	{
		if (Random.Range(0, 6) == 0 && this.jobNum > 1)
		{
			this.Say(2);
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000EA45 File Offset: 0x0000CC45
	public void Say(int voiceNum)
	{
		if (!this.aSource.isPlaying)
		{
			this.aSource.clip = this.clip[voiceNum];
			this.aSource.Play();
		}
	}

	// Token: 0x04000370 RID: 880
	public int jobNum;

	// Token: 0x04000371 RID: 881
	public AudioSource aSource;

	// Token: 0x04000372 RID: 882
	public AudioClip[] clip;

	// Token: 0x04000373 RID: 883
	public MissionController mc;

	// Token: 0x04000374 RID: 884
	public GameObject lostkeys;

	// Token: 0x04000375 RID: 885
	public Animator anim;
}
