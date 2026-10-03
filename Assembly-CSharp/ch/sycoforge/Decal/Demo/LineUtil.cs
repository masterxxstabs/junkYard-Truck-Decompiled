using System;
using System.Collections.Generic;
using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001B1 RID: 433
	public static class LineUtil
	{
		// Token: 0x06000A9E RID: 2718 RVA: 0x0008D800 File Offset: 0x0008BA00
		public static void DrawPath(float thickness, Material material, List<Vector3> path)
		{
			if (path == null || (path != null && path.Count < 2))
			{
				return;
			}
			if (thickness <= Mathf.Epsilon)
			{
				GL.Begin(1);
			}
			else
			{
				GL.Begin(7);
			}
			material.SetPass(0);
			GL.Color(Color.blue);
			Vector3 start = path[0];
			for (int i = 1; i < path.Count; i++)
			{
				Vector3 vector = path[i];
				LineUtil.DrawLine(thickness, start, vector);
				start = vector;
			}
			GL.End();
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0008D878 File Offset: 0x0008BA78
		private static void DrawLine(float thickness, Vector3 start, Vector3 end)
		{
			if (thickness <= Mathf.Epsilon)
			{
				GL.Vertex(start);
				GL.Vertex(end);
				return;
			}
			Camera main = Camera.main;
			Vector3 normalized = (end - start).normalized;
			Vector3 b = Vector3.Cross((start - main.transform.position).normalized, normalized) * (thickness / 2f);
			GL.Vertex(start - b);
			GL.Vertex(start + b);
			GL.Vertex(end + b);
			GL.Vertex(end - b);
		}
	}
}
