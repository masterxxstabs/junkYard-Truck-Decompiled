using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x02000015 RID: 21
public class Benchpress : MonoBehaviour
{
	// Token: 0x0600004F RID: 79 RVA: 0x00004970 File Offset: 0x00002B70
	private void Start()
	{
		this.UpdatePlates();
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00004978 File Offset: 0x00002B78
	public void UpdatePlates()
	{
		if (this.curr.strength > 13)
		{
			this.plateL2.SetActive(true);
			this.plateR2.SetActive(true);
		}
		if (this.curr.strength > 26)
		{
			this.plateL3.SetActive(true);
			this.plateR3.SetActive(true);
		}
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000049D4 File Offset: 0x00002BD4
	public void Bench()
	{
		if (!this.curr.tired)
		{
			this.fpc.canMove = false;
			this.fps.position = this.pressLoc.position;
			this.anim.Play();
			this.curr.tired = true;
			this.fpc.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0f, 0f, 0f);
			this.fpc.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(-35f, 0f, 0f);
			this.curr.strength++;
			base.StartCoroutine(this.StartBench());
			this.aSource.Play();
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00004A9E File Offset: 0x00002C9E
	private IEnumerator StartBench()
	{
		yield return new WaitForSeconds(5f);
		this.fpc.canMove = true;
		this.fps.position = this.finishedLoc.position;
		this.curr.water -= 5f;
		if (this.curr.water < 0f)
		{
			this.curr.water = 0f;
		}
		if (this.curr.strength < 13)
		{
			this.curr.sleep -= 5f;
			if (this.curr.sleep < 0f)
			{
				this.curr.sleep = 0f;
			}
		}
		else
		{
			this.curr.sleep += 5f;
		}
		yield break;
	}

	// Token: 0x040000D0 RID: 208
	public GameObject plateR2;

	// Token: 0x040000D1 RID: 209
	public GameObject plateR3;

	// Token: 0x040000D2 RID: 210
	public GameObject plateL2;

	// Token: 0x040000D3 RID: 211
	public GameObject plateL3;

	// Token: 0x040000D4 RID: 212
	public Currency curr;

	// Token: 0x040000D5 RID: 213
	public Animation anim;

	// Token: 0x040000D6 RID: 214
	public Transform fps;

	// Token: 0x040000D7 RID: 215
	public Transform pressLoc;

	// Token: 0x040000D8 RID: 216
	public Transform finishedLoc;

	// Token: 0x040000D9 RID: 217
	public FirstPersonController fpc;

	// Token: 0x040000DA RID: 218
	public AudioSource aSource;
}
