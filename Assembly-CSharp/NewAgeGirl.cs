using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class NewAgeGirl : MonoBehaviour
{
	// Token: 0x060006CE RID: 1742 RVA: 0x00057A4A File Offset: 0x00055C4A
	private void Start()
	{
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00057A58 File Offset: 0x00055C58
	public void Interact()
	{
		if (!this.girlBusy)
		{
			this.aSources[this.dialogueNum].Play();
			this.girlBusy = true;
			base.StartCoroutine(this.StayBusy());
			this.dialogueNum++;
			if (this.dialogueNum > 6)
			{
				this.dialogueNum = 0;
			}
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00057AB4 File Offset: 0x00055CB4
	public void Bye()
	{
		this.ranNum2 = Random.Range(1, 3);
		if (this.ranNum2 == 2 && !this.girlBusy)
		{
			this.girlBusy = true;
			base.StartCoroutine(this.StayBusy());
			this.ranNum = Random.Range(11, 13);
			this.aSources[this.ranNum].Play();
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00057B14 File Offset: 0x00055D14
	public void Hello()
	{
		this.ranNum2 = Random.Range(1, 3);
		if (this.ranNum2 == 2 && !this.girlBusy && !this.helloBusy)
		{
			this.girlBusy = true;
			this.helloBusy = true;
			base.StartCoroutine(this.StayBusy());
			base.StartCoroutine(this.StayBusy2());
			this.ranNum = Random.Range(13, 15);
			this.aSources[this.ranNum].Play();
		}
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00057B90 File Offset: 0x00055D90
	public void StillHere()
	{
		if (!this.girlBusy)
		{
			this.girlBusy = true;
			base.StartCoroutine(this.StayBusy());
			this.aSources[15].Play();
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00057BBC File Offset: 0x00055DBC
	private IEnumerator StayBusy()
	{
		yield return new WaitForSeconds(7f);
		this.girlBusy = false;
		yield break;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00057BCB File Offset: 0x00055DCB
	private IEnumerator StayBusy2()
	{
		yield return new WaitForSeconds(180f);
		this.helloBusy = false;
		yield break;
	}

	// Token: 0x04000F06 RID: 3846
	public AudioSource[] aSources;

	// Token: 0x04000F07 RID: 3847
	public bool girlBusy;

	// Token: 0x04000F08 RID: 3848
	public bool helloBusy;

	// Token: 0x04000F09 RID: 3849
	public Transform audioLoc;

	// Token: 0x04000F0A RID: 3850
	private int ranNum;

	// Token: 0x04000F0B RID: 3851
	private int ranNum2;

	// Token: 0x04000F0C RID: 3852
	private int dialogueNum;
}
