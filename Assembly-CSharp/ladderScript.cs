using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x0200017B RID: 379
public class ladderScript : MonoBehaviour
{
	// Token: 0x06000948 RID: 2376 RVA: 0x0007DCC7 File Offset: 0x0007BEC7
	private void Start()
	{
		this.FPSInput = base.GetComponent<FirstPersonController>();
		this.inside = false;
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x0007DCDC File Offset: 0x0007BEDC
	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Ladder")
		{
			this.FPSInput.enabled = false;
			this.inside = !this.inside;
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0007DD10 File Offset: 0x0007BF10
	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Ladder")
		{
			this.FPSInput.enabled = true;
			this.inside = !this.inside;
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0007DD44 File Offset: 0x0007BF44
	private void Update()
	{
		if (this.inside && Input.GetKey(this.cr.Forward))
		{
			this.chController.transform.position += Vector3.up / this.speedUpDown;
		}
		if (this.inside && Input.GetKey(this.cr.Backward))
		{
			this.chController.transform.position += Vector3.down / this.speedUpDown;
		}
	}

	// Token: 0x040018DF RID: 6367
	public Transform chController;

	// Token: 0x040018E0 RID: 6368
	private bool inside;

	// Token: 0x040018E1 RID: 6369
	private float speedUpDown = 20f;

	// Token: 0x040018E2 RID: 6370
	public FirstPersonController FPSInput;

	// Token: 0x040018E3 RID: 6371
	public ControlRef cr;
}
