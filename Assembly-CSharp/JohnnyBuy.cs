using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class JohnnyBuy : MonoBehaviour
{
	// Token: 0x0600057D RID: 1405 RVA: 0x00044EDA File Offset: 0x000430DA
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00044EE8 File Offset: 0x000430E8
	public bool IsBusy()
	{
		return this.aSources[0].isPlaying || this.aSources[1].isPlaying || this.aSources[2].isPlaying || this.aSources[3].isPlaying || this.aSources[4].isPlaying;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x00044F44 File Offset: 0x00043144
	public void JohnnyInteract()
	{
		if (!this.IsBusy() && (this.dialogueNum < 3 || this.dialogueNum == 4))
		{
			this.johnnybusy = true;
			this.aSources[this.dialogueNum].Play();
			if (this.dialogueNum == 0)
			{
				this.dialogue = "You here about the truck? You picked the right day.";
				this.interactor.Subtitle(this.dialogue, this.aSources[this.dialogueNum].clip);
			}
			if (this.dialogueNum == 1)
			{
				this.dialogue = "These Diamondbacks are actually really, really durable trucks! That's why they're still around...heh. They're from a time when trucks were made of metal.";
				this.interactor.Subtitle(this.dialogue, this.aSources[this.dialogueNum].clip);
			}
			if (this.dialogueNum == 2)
			{
				this.dialogue = "This one's engine is in better condition...With a little fixing it can be up and running.";
				this.interactor.Subtitle(this.dialogue, this.aSources[this.dialogueNum].clip);
			}
			if (this.dialogueNum == 4)
			{
				this.dialogue = "These trucks use really common parts, so I'll always have a lot of shit here that's compatable. And if you can't find what you want, check back in a day or two.";
				this.interactor.Subtitle(this.dialogue, this.aSources[this.dialogueNum].clip);
			}
			if (this.dialogueNum < 5)
			{
				this.dialogueNum++;
			}
		}
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0004507C File Offset: 0x0004327C
	public void JohnnyInteract2()
	{
		if (!this.IsBusy())
		{
			this.johnnybusy = true;
			this.aSources[this.dialogueNum].Play();
			this.dialogue = "That's the one you want? ...k... I'll have it delivered to your place tomorrow morning.";
			this.interactor.Subtitle(this.dialogue, this.aSources[this.dialogueNum].clip);
			this.dialogueNum++;
		}
	}

	// Token: 0x04000BDF RID: 3039
	public int dialogueNum;

	// Token: 0x04000BE0 RID: 3040
	private bool johnnybusy = true;

	// Token: 0x04000BE1 RID: 3041
	public AudioSource[] aSources;

	// Token: 0x04000BE2 RID: 3042
	private string dialogue;

	// Token: 0x04000BE3 RID: 3043
	public Interactor interactor;
}
