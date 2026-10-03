using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class CreeperMovement : MonoBehaviour
{
	// Token: 0x06000140 RID: 320 RVA: 0x0000EE2C File Offset: 0x0000D02C
	private void Start()
	{
		this.aSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000EE3C File Offset: 0x0000D03C
	private void FixedUpdate()
	{
		if (this.controlled)
		{
			if (!this.blocksEnabled)
			{
				this.blocksEnabled = true;
				this.creeperBlocks.SetActive(true);
			}
			this.translation = this.cr.Vert * this.speed * Time.deltaTime;
			this.straffe = this.cr.Horiz * this.speed * Time.deltaTime;
			base.transform.position += this.FPSController.transform.forward * this.translation;
			base.transform.Translate(this.straffe, 0f, 0f);
			if (this.translation != 0f || this.straffe != 0f)
			{
				if (!this.playing)
				{
					base.StartCoroutine("playCastor");
				}
				if (this.playing)
				{
					this.aSource.volume = Mathf.Abs(this.translation) + Mathf.Abs(this.straffe);
					return;
				}
			}
		}
		else if (this.blocksEnabled)
		{
			this.blocksEnabled = false;
			this.creeperBlocks.SetActive(false);
		}
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000EF6B File Offset: 0x0000D16B
	private IEnumerator playCastor()
	{
		this.playing = true;
		this.aSource.Play();
		yield return new WaitForSeconds(this.aSource.clip.length);
		this.playing = false;
		yield break;
	}

	// Token: 0x04000384 RID: 900
	public float speed = 0.5f;

	// Token: 0x04000385 RID: 901
	private float translation;

	// Token: 0x04000386 RID: 902
	private float straffe;

	// Token: 0x04000387 RID: 903
	private AudioSource aSource;

	// Token: 0x04000388 RID: 904
	private bool playing;

	// Token: 0x04000389 RID: 905
	public bool controlled;

	// Token: 0x0400038A RID: 906
	public GameObject FPSController;

	// Token: 0x0400038B RID: 907
	public GameObject dismountSphere;

	// Token: 0x0400038C RID: 908
	public GameObject creeperBlocks;

	// Token: 0x0400038D RID: 909
	private bool blocksEnabled;

	// Token: 0x0400038E RID: 910
	public ControlRef cr;
}
