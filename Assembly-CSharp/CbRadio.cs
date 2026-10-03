using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class CbRadio : MonoBehaviour
{
	// Token: 0x0600012F RID: 303 RVA: 0x0000E855 File Offset: 0x0000CA55
	private void Start()
	{
		this.busy = false;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0000E85E File Offset: 0x0000CA5E
	public void PlayClip()
	{
		if (this.renderer.enabled)
		{
			this.aSource.clip = this.clip[this.interactor.radioNum];
			this.aSource.Play();
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000E898 File Offset: 0x0000CA98
	private void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer >= 10f)
		{
			this.currTime = EnviroSkyMgr.instance.GetTimeOfDay();
			this.timer = 0f;
			if (this.currTime > 0.2f && this.currTime < 0.4f && !this.busy)
			{
				this.PlayClip();
				this.renderer.material.EnableKeyword("_EMISSION");
				this.busy = true;
				base.StartCoroutine(this.UnBusy());
			}
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000E930 File Offset: 0x0000CB30
	private IEnumerator UnBusy()
	{
		yield return new WaitForSeconds(this.aSource.clip.length);
		this.renderer.material.DisableKeyword("_EMISSION");
		yield return new WaitForSeconds(20f);
		this.interactor.radioNum++;
		if (this.interactor.radioNum > 5)
		{
			this.interactor.radioNum = 0;
		}
		this.busy = false;
		yield break;
	}

	// Token: 0x0400035A RID: 858
	public AudioSource aSource;

	// Token: 0x0400035B RID: 859
	public AudioClip[] clip;

	// Token: 0x0400035C RID: 860
	public Interactor interactor;

	// Token: 0x0400035D RID: 861
	private float currTime;

	// Token: 0x0400035E RID: 862
	private float timer;

	// Token: 0x0400035F RID: 863
	private bool busy;

	// Token: 0x04000360 RID: 864
	public Renderer renderer;
}
