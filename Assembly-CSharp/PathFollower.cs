using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class PathFollower : MonoBehaviour
{
	// Token: 0x060007EB RID: 2027 RVA: 0x00066E1C File Offset: 0x0006501C
	private void OnDrawGizmos()
	{
		for (int i = 0; i < this.pathParent.childCount; i++)
		{
			Vector3 position = this.pathParent.GetChild(i).position;
			Vector3 position2 = this.pathParent.GetChild((i + 1) % this.pathParent.childCount).position;
			Gizmos.color = new Color(1f, 0f, 0f);
			Gizmos.DrawLine(position, position2);
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00066E8F File Offset: 0x0006508F
	private void Start()
	{
		this.index = 0;
		this.targetPoint = this.pathParent.GetChild(this.index);
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x00066EB0 File Offset: 0x000650B0
	private void Update()
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPoint.position, this.speed * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, this.targetPoint.position) < 0.1f)
		{
			this.index++;
			this.index %= this.pathParent.childCount;
			this.targetPoint = this.pathParent.GetChild(this.index);
		}
	}

	// Token: 0x0400126A RID: 4714
	public float speed = 3f;

	// Token: 0x0400126B RID: 4715
	public Transform pathParent;

	// Token: 0x0400126C RID: 4716
	private Transform targetPoint;

	// Token: 0x0400126D RID: 4717
	private int index;
}
