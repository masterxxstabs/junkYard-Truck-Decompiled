using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class RainDrop : MonoBehaviour
{
	// Token: 0x06000499 RID: 1177 RVA: 0x0002FE77 File Offset: 0x0002E077
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		Object.Destroy(base.gameObject, 10f);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0002FE98 File Offset: 0x0002E098
	public void OnCollisionEnter(Collision col)
	{
		if (this.audioSource.isPlaying)
		{
			return;
		}
		this.audioSource.volume = col.relativeVelocity.magnitude * 0.04f;
		this.audioSource.pitch = Random.Range(0.2f, 1.2f);
		this.audioSource.Play();
	}

	// Token: 0x0400095C RID: 2396
	private AudioSource audioSource;
}
