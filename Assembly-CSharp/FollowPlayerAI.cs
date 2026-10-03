using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class FollowPlayerAI : MonoBehaviour
{
	// Token: 0x060000AA RID: 170 RVA: 0x00002188 File Offset: 0x00000388
	public void Start()
	{
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00002188 File Offset: 0x00000388
	private void FixedUpdate()
	{
	}

	// Token: 0x040001CA RID: 458
	private Transform cameraTarget;

	// Token: 0x040001CB RID: 459
	[HideInInspector]
	public float sSpeed = 10f;

	// Token: 0x040001CC RID: 460
	[HideInInspector]
	public Vector3 dist;

	// Token: 0x040001CD RID: 461
	public Transform lookTarget;

	// Token: 0x040001CE RID: 462
	public Transform[] camerasPlaces;

	// Token: 0x040001CF RID: 463
	private int i;
}
