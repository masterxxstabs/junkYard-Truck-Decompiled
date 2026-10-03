using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class Headtrack : MonoBehaviour
{
	// Token: 0x06000463 RID: 1123 RVA: 0x0002F332 File Offset: 0x0002D532
	private void Start()
	{
		this.LastFace = this.headBone.transform.rotation;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0002F34A File Offset: 0x0002D54A
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "FPSController")
		{
			this.inProximity = true;
			base.StartCoroutine(this.StopLook());
		}
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0002F377 File Offset: 0x0002D577
	private void OnTriggerExit(Collider other)
	{
		this.inProximity = false;
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0002F380 File Offset: 0x0002D580
	private IEnumerator StopLook()
	{
		yield return new WaitForSeconds(this.disinterested);
		this.inProximity = false;
		this.headBone.transform.rotation = this.LastFace;
		yield break;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0002F390 File Offset: 0x0002D590
	private void Update()
	{
		if (this.inProximity)
		{
			if (!this.reverseLook)
			{
				this.headBone.transform.LookAt(2f * this.headBone.transform.position - this.playerCam.position);
				return;
			}
			if (this.offset == 0f)
			{
				this.headBone.transform.LookAt(this.playerCam.position);
				this.headBone.transform.rotation *= Quaternion.Euler(-15f, -30f, 0f);
				return;
			}
			this.headBone.transform.LookAt(this.playerCam.position);
			this.headBone.transform.rotation *= Quaternion.Euler(-20f, -115f, 0f);
		}
	}

	// Token: 0x04000931 RID: 2353
	public GameObject headBone;

	// Token: 0x04000932 RID: 2354
	public Transform playerCam;

	// Token: 0x04000933 RID: 2355
	public bool inProximity;

	// Token: 0x04000934 RID: 2356
	private Quaternion LastFace;

	// Token: 0x04000935 RID: 2357
	public bool reverseLook;

	// Token: 0x04000936 RID: 2358
	public float disinterested;

	// Token: 0x04000937 RID: 2359
	public float offset;
}
