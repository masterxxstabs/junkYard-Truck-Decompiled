using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class Spline : MonoBehaviour
{
	// Token: 0x06000824 RID: 2084 RVA: 0x0006C7A4 File Offset: 0x0006A9A4
	private void Start()
	{
		this.splineCount = base.transform.childCount;
		this.splinePoint = new Vector3[this.splineCount];
		for (int i = 0; i < this.splineCount; i++)
		{
			this.splinePoint[i] = base.transform.GetChild(i).position;
		}
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0006C804 File Offset: 0x0006AA04
	public Vector3 WhereOnSpline(Vector3 pos)
	{
		int closestSplinePoint = this.GetClosestSplinePoint(pos);
		if (closestSplinePoint == 0)
		{
			return this.splineSegment(this.splinePoint[0], this.splinePoint[1], pos);
		}
		if (closestSplinePoint == this.splineCount - 1)
		{
			return this.splineSegment(this.splinePoint[this.splineCount - 1], this.splinePoint[this.splineCount - 2], pos);
		}
		Vector3 vector = this.splineSegment(this.splinePoint[closestSplinePoint - 1], this.splinePoint[closestSplinePoint], pos);
		Vector3 vector2 = this.splineSegment(this.splinePoint[closestSplinePoint + 1], this.splinePoint[closestSplinePoint], pos);
		if ((pos - vector).sqrMagnitude <= (pos - vector2).sqrMagnitude)
		{
			return vector;
		}
		return vector2;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0006C8DC File Offset: 0x0006AADC
	private int GetClosestSplinePoint(Vector3 pos)
	{
		int result = -1;
		float num = 0f;
		for (int i = 0; i < this.splineCount; i++)
		{
			float sqrMagnitude = (this.splinePoint[i] - pos).sqrMagnitude;
			if (num == 0f || sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0006C930 File Offset: 0x0006AB30
	public Vector3 splineSegment(Vector3 v1, Vector3 v2, Vector3 pos)
	{
		Vector3 rhs = pos - v1;
		Vector3 normalized = (v2 - v1).normalized;
		float num = Vector3.Dot(normalized, rhs);
		if (num < 0f)
		{
			return v1;
		}
		if (num * num > (v2 - v1).sqrMagnitude)
		{
			return v2;
		}
		Vector3 b = normalized * num;
		return v1 + b;
	}

	// Token: 0x04001307 RID: 4871
	private Vector3[] splinePoint;

	// Token: 0x04001308 RID: 4872
	private int splineCount;

	// Token: 0x04001309 RID: 4873
	public bool debug_drawSpline = true;
}
