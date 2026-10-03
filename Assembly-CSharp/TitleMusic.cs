using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class TitleMusic : MonoBehaviour
{
	// Token: 0x06000874 RID: 2164 RVA: 0x0006E878 File Offset: 0x0006CA78
	private void Start()
	{
		this.aSource[0].time = Random.value * this.aSource[0].clip.length;
		this.aSource[0].Play();
		base.StartCoroutine(this.Crow());
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0006E8C4 File Offset: 0x0006CAC4
	private IEnumerator Crow()
	{
		for (;;)
		{
			this.aSource[1].Play();
			this.crowwait = Random.Range(20, 55);
			yield return new WaitForSeconds((float)this.crowwait);
		}
		yield break;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00002188 File Offset: 0x00000388
	private void Update()
	{
	}

	// Token: 0x0400138F RID: 5007
	public AudioSource[] aSource;

	// Token: 0x04001390 RID: 5008
	private int crowwait = 6;
}
