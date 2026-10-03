using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class ModernGarageOpener : MonoBehaviour
{
	// Token: 0x06000679 RID: 1657 RVA: 0x0004DC92 File Offset: 0x0004BE92
	private void Start()
	{
		if (this.isUp)
		{
			this.anim.Play("Garagedoor2X1");
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0004DCB0 File Offset: 0x0004BEB0
	public void UseDoor()
	{
		if (this.atm.ownsHouse && !this.isLifting)
		{
			this.isLifting = true;
			if (!this.isUp)
			{
				this.anim.Play("Garagedoor2X1");
				this.isUp = true;
			}
			else
			{
				this.anim.Play("Garagedoor2X2");
				this.isUp = false;
			}
			base.StartCoroutine(this.EnableLift());
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0004DD20 File Offset: 0x0004BF20
	private IEnumerator EnableLift()
	{
		this.asource.Play();
		yield return new WaitForSeconds(3f);
		this.isLifting = false;
		yield break;
	}

	// Token: 0x04000D9C RID: 3484
	public Animation anim;

	// Token: 0x04000D9D RID: 3485
	public bool isUp;

	// Token: 0x04000D9E RID: 3486
	private bool isLifting;

	// Token: 0x04000D9F RID: 3487
	public Atm atm;

	// Token: 0x04000DA0 RID: 3488
	public AudioSource asource;
}
