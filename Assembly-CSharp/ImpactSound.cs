using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class ImpactSound : MonoBehaviour
{
	// Token: 0x060004C4 RID: 1220 RVA: 0x00030F08 File Offset: 0x0002F108
	private void Start()
	{
		if (base.gameObject.name.Contains("bottlescrap") || base.gameObject.name.Contains("beercan") || base.gameObject.name.Contains("energycanEmpty") || base.gameObject.name.Contains("beefareeno_empty") || base.gameObject.name.Contains("cram_empty") || base.gameObject.name.Contains("cancrushed"))
		{
			Object.Destroy(base.gameObject, 120f);
		}
		if (base.gameObject.name.Contains("beerbottles") && base.gameObject.GetComponent<Beercase>().bottles < 1)
		{
			base.enabled = false;
		}
		if (base.gameObject.name.Contains("beercancrushed"))
		{
			this.audioSource.clip = this.clipx[Random.Range(7, 10)];
			this.audioSource.Play();
			this.isCrushClipPlaying = true;
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00031024 File Offset: 0x0002F224
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > this.impactThreshold && (!this.audioSource.isPlaying || this.isCrushClipPlaying))
		{
			bool flag = false;
			if (base.gameObject.name.Contains("bottlescrap") && Random.Range(0, 2) == 0)
			{
				flag = true;
			}
			if (!flag)
			{
				this.audioSource.clip = this.clipx[Random.Range(0, 7)];
				this.audioSource.Play();
				this.isCrushClipPlaying = false;
				return;
			}
			this.ShatterBottle();
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x000310B8 File Offset: 0x0002F2B8
	private void ShatterBottle()
	{
		if (base.GetComponent<MeshRenderer>().enabled)
		{
			GameObject obj = Object.Instantiate<GameObject>(this.frag1, base.transform.position, base.transform.rotation);
			Object obj2 = Object.Instantiate<GameObject>(this.frag2, base.transform.position, base.transform.rotation);
			this.audioSource.clip = this.clipx[Random.Range(7, 10)];
			this.audioSource.Play();
			Object.Destroy(obj, 170f);
			Object.Destroy(obj2, 170f);
			base.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	// Token: 0x04000983 RID: 2435
	public float impactThreshold = 1f;

	// Token: 0x04000984 RID: 2436
	public AudioSource audioSource;

	// Token: 0x04000985 RID: 2437
	public AudioClip[] clipx;

	// Token: 0x04000986 RID: 2438
	public GameObject frag1;

	// Token: 0x04000987 RID: 2439
	public GameObject frag2;

	// Token: 0x04000988 RID: 2440
	private bool isCrushClipPlaying;
}
