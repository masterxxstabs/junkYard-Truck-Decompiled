using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class BezierScript : MonoBehaviour
{
	// Token: 0x06000054 RID: 84 RVA: 0x00004AAD File Offset: 0x00002CAD
	private void Start()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004ABB File Offset: 0x00002CBB
	private void Update()
	{
		this.DrawQuadraticBezierCurve(this.p0.position, this.p1.position, this.p2.position);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00004AE4 File Offset: 0x00002CE4
	private void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
	{
		this.lineRenderer.positionCount = 200;
		float num = 0f;
		Vector3 position = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < this.lineRenderer.positionCount; i++)
		{
			position = (1f - num) * (1f - num) * point0 + 2f * (1f - num) * num * point1 + num * num * point2;
			this.lineRenderer.SetPosition(i, position);
			num += 1f / (float)this.lineRenderer.positionCount;
		}
	}

	// Token: 0x040000DB RID: 219
	private LineRenderer lineRenderer;

	// Token: 0x040000DC RID: 220
	public Transform p0;

	// Token: 0x040000DD RID: 221
	public Transform p1;

	// Token: 0x040000DE RID: 222
	public Transform p2;
}
