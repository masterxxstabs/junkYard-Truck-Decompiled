using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public class impactnoise : MonoBehaviour
{
	// Token: 0x06000935 RID: 2357 RVA: 0x0007D35F File Offset: 0x0007B55F
	private void Start()
	{
		this.audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0007D374 File Offset: 0x0007B574
	private void OnCollisionEnter(Collision collision)
	{
		this.audioSource.clip = this.audioClips[Random.Range(0, this.audioClips.Length)];
		this.audioSource.volume = 0.2f;
		if (!this.audioSource.isPlaying)
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x0400189C RID: 6300
	public AudioClip[] audioClips;

	// Token: 0x0400189D RID: 6301
	public AudioSource audioSource;
}
