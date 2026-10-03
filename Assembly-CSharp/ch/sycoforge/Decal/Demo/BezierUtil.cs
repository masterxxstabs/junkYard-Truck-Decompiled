using System;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001B0 RID: 432
	public static class BezierUtil
	{
		// Token: 0x06000A9B RID: 2715 RVA: 0x0008D5CC File Offset: 0x0008B7CC
		public static List<Vector3> InterpolatePath(List<Vector3> path, int segments, float radius, float angleThreshold)
		{
			if (path.Count >= 3)
			{
				List<Vector3> list = new List<Vector3>();
				int num = path.Count - 1;
				list.Add(path[0]);
				int num2 = 0;
				for (int i = 2; i < path.Count; i++)
				{
					Vector3 b = path[i - 2];
					Vector3 vector = path[i - 1];
					Vector3 a = path[i];
					Vector3 vector2 = vector - b;
					Vector3 vector3 = a - vector;
					if (Mathf.Abs(Vector3.Angle(vector2, vector3)) > angleThreshold)
					{
						float num3 = vector2.magnitude;
						float num4 = vector3.magnitude;
						vector2.Normalize();
						vector3.Normalize();
						num3 = Mathf.Min(num3 * 0.5f, radius);
						num4 = Mathf.Min(num4 * 0.5f, radius);
						Vector3 a2 = vector - vector2 * num3;
						Vector3 a3 = vector;
						Vector3 a4 = vector + vector3 * num4;
						for (int j = 0; j < segments; j++)
						{
							float num5 = (float)j / ((float)segments - 1f);
							float num6 = 1f - num5;
							Vector3 item = num6 * num6 * a2 + 2f * num6 * num5 * a3 + num5 * num5 * a4;
							list.Add(item);
						}
						num2 = i;
					}
				}
				if (num2 <= num)
				{
					list.Add(path[num]);
				}
				return list;
			}
			return path;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x0008D744 File Offset: 0x0008B944
		public static Vector3[] GetBezierApproximation(Vector3[] controlPoints, int outputSegmentCount)
		{
			Vector3[] array = new Vector3[outputSegmentCount + 1];
			for (int i = 0; i < outputSegmentCount; i++)
			{
				float t = (float)i / (float)outputSegmentCount;
				array[i] = BezierUtil.GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
			}
			return array;
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x0008D780 File Offset: 0x0008B980
		public static Vector3 GetBezierPoint(float t, Vector3[] controlPoints, int index, int count)
		{
			if (count == 1)
			{
				return controlPoints[index];
			}
			Vector3 bezierPoint = BezierUtil.GetBezierPoint(t, controlPoints, index - 1, count - 1);
			Vector3 bezierPoint2 = BezierUtil.GetBezierPoint(t, controlPoints, index, count - 1);
			Vector3 bezierPoint3 = BezierUtil.GetBezierPoint(t, controlPoints, index + 1, count - 1);
			return (1f - t) * (1f - t) * bezierPoint + 2f * (1f - t) * t * bezierPoint2 + t * t * bezierPoint3;
		}
	}
}
