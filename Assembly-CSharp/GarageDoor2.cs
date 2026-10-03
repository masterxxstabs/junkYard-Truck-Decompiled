using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class GarageDoor2 : MonoBehaviour
{
	// Token: 0x06000870 RID: 2160 RVA: 0x0006E823 File Offset: 0x0006CA23
	private void Start()
	{
		this.doorAnim = base.GetComponent<Animation>();
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0006E831 File Offset: 0x0006CA31
	private void OnTriggerEnter(Collider other)
	{
		if (!this.opened)
		{
			this.doorAnim.Play("Open");
			this.opened = true;
		}
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0006E853 File Offset: 0x0006CA53
	private void OnTriggerExit(Collider other)
	{
		if (this.opened)
		{
			this.doorAnim.Play("Close");
			this.opened = false;
		}
	}

	// Token: 0x0400138D RID: 5005
	private Animation doorAnim;

	// Token: 0x0400138E RID: 5006
	private bool opened;
}
