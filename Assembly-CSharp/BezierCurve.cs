using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012C RID: 300
public static class BezierCurve
{
	// Token: 0x060007C7 RID: 1991 RVA: 0x00063C38 File Offset: 0x00061E38
	public static void GetBezierCurve(Vector3 A, Vector3 B, Vector3 C, Vector3 D, List<Vector3> allRopeSections)
	{
		float num = 0.1f;
		allRopeSections.Clear();
		for (float num2 = 0f; num2 <= 1f; num2 += num)
		{
			Vector3 item = BezierCurve.DeCasteljausAlgorithm(A, B, C, D, num2);
			allRopeSections.Add(item);
		}
		allRopeSections.Add(D);
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00063C84 File Offset: 0x00061E84
	private static Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
	{
		float d = 1f - t;
		Vector3 a = d * A + t * B;
		Vector3 a2 = d * B + t * C;
		Vector3 a3 = d * C + t * D;
		Vector3 a4 = d * a + t * a2;
		Vector3 a5 = d * a2 + t * a3;
		return d * a4 + t * a5;
	}
}
