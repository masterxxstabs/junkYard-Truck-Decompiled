using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class Radio : MonoBehaviour
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x0006302E File Offset: 0x0006122E
	private void Start()
	{
		if (!base.GetComponent<AudioSource>().playOnAwake)
		{
			base.GetComponent<AudioSource>().clip = this.soundtrack[Random.Range(0, this.soundtrack.Length)];
			base.GetComponent<AudioSource>().Play();
		}
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00063068 File Offset: 0x00061268
	public void Toggle()
	{
		this.off = !this.off;
		if (this.off)
		{
			base.GetComponent<AudioSource>().volume = 0f;
			return;
		}
		base.GetComponent<AudioSource>().volume = 0.5f;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x000630A4 File Offset: 0x000612A4
	private void Update()
	{
		if (!this.off && Time.time >= (float)this.songInterval)
		{
			this.skip = Random.Range(0, 3);
			if (this.skip == 0)
			{
				base.GetComponent<AudioSource>().clip = this.soundtrack[Random.Range(0, this.soundtrack.Length)];
				base.GetComponent<AudioSource>().Play();
				return;
			}
			this.songInterval = Mathf.FloorToInt(Time.time) + 60;
		}
	}

	// Token: 0x04001171 RID: 4465
	public AudioClip[] soundtrack;

	// Token: 0x04001172 RID: 4466
	private int songInterval;

	// Token: 0x04001173 RID: 4467
	private int skip;

	// Token: 0x04001174 RID: 4468
	public bool off;
}
