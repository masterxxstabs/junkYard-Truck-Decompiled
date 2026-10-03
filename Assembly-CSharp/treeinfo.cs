using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class treeinfo : MonoBehaviour
{
	// Token: 0x06000A1C RID: 2588 RVA: 0x0008A495 File Offset: 0x00088695
	private void Start()
	{
		this.shaft.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.aSources = base.GetComponents<AudioSource>();
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0008A4C8 File Offset: 0x000886C8
	private void Update()
	{
		if (this.shaft != null)
		{
			if (this.shaft.transform.localEulerAngles.x > 5f || this.shaft.transform.localEulerAngles.z > 5f || this.shaft.transform.localEulerAngles.x < -5f || this.shaft.transform.localEulerAngles.z < -5f)
			{
				this.falling = true;
				if (!this.aSources[0].isPlaying && !this.aSources[1].isPlaying && !this.dead && !this.isMissionTree)
				{
					this.aSources[0].Play();
					base.StartCoroutine(this.killSound());
				}
			}
			if (this.shaft.transform.localEulerAngles.x > 50f && this.shaft.transform.localEulerAngles.x < 120f && !this.dead && !this.isMissionTree)
			{
				this.aSources[1].Play();
				this.aSources[0].Stop();
				this.dead = true;
				this.sectionscut = 0;
			}
			if (this.shaft.transform.localEulerAngles.z > 50f && this.shaft.transform.localEulerAngles.z < 120f && !this.dead && !this.isMissionTree)
			{
				this.aSources[1].Play();
				this.aSources[0].Stop();
				this.dead = true;
				this.sectionscut = 0;
			}
		}
		else
		{
			this.dead = true;
		}
		if (this.dead)
		{
			base.StartCoroutine(this.killStump());
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0008A6A3 File Offset: 0x000888A3
	private IEnumerator killSound()
	{
		yield return new WaitForSeconds(5f);
		this.aSources[0].Stop();
		this.dead = true;
		if (base.transform.parent.name == "cuttableTreeXLM")
		{
			GameObject.Find("businessman").GetComponent<Businessman>().TreeCutUpdate();
		}
		yield break;
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0008A6B2 File Offset: 0x000888B2
	public IEnumerator killStump()
	{
		yield return new WaitForSeconds(5f);
		base.enabled = false;
		yield break;
	}

	// Token: 0x04001C03 RID: 7171
	public int health = 1000;

	// Token: 0x04001C04 RID: 7172
	public GameObject shaft;

	// Token: 0x04001C05 RID: 7173
	public bool falling;

	// Token: 0x04001C06 RID: 7174
	public bool dead;

	// Token: 0x04001C07 RID: 7175
	public int sectionscut;

	// Token: 0x04001C08 RID: 7176
	private AudioSource[] aSources;

	// Token: 0x04001C09 RID: 7177
	public bool isMissionTree;
}
