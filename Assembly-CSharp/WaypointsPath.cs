using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class WaypointsPath : MonoBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060000B6 RID: 182 RVA: 0x00009970 File Offset: 0x00007B70
	// (set) Token: 0x060000B7 RID: 183 RVA: 0x00009978 File Offset: 0x00007B78
	public float Length { get; private set; }

	// Token: 0x060000B8 RID: 184 RVA: 0x00009981 File Offset: 0x00007B81
	private void Awake()
	{
		if (this.nodes.Count > 1)
		{
			this.CachePositionsAndDistances();
		}
		this.numPoints = this.nodes.Count;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x000099A8 File Offset: 0x00007BA8
	public WaypointsPath.RoutePoint GetRoutePoint(float dist)
	{
		Vector3 routePosition = this.GetRoutePosition(dist);
		return new WaypointsPath.RoutePoint(routePosition, (this.GetRoutePosition(dist + 0.1f) - routePosition).normalized);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000099E0 File Offset: 0x00007BE0
	public Vector3 GetRoutePosition(float dist)
	{
		int num = 0;
		if (this.Length == 0f)
		{
			this.Length = this.distances[this.distances.Length - 1];
		}
		dist = Mathf.Repeat(dist, this.Length);
		while (this.distances[num] < dist)
		{
			num++;
		}
		this.p1n = (num - 1 + this.numPoints) % this.numPoints;
		this.p2n = num;
		this.i = Mathf.InverseLerp(this.distances[this.p1n], this.distances[this.p2n], dist);
		if (this.smoothRoute)
		{
			this.p0n = (num - 2 + this.numPoints) % this.numPoints;
			this.p3n = (num + 1) % this.numPoints;
			this.p2n %= this.numPoints;
			this.P0 = this.points[this.p0n];
			this.P1 = this.points[this.p1n];
			this.P2 = this.points[this.p2n];
			this.P3 = this.points[this.p3n];
			return this.CatmullRom(this.P0, this.P1, this.P2, this.P3, this.i);
		}
		this.p1n = (num - 1 + this.numPoints) % this.numPoints;
		this.p2n = num;
		return Vector3.Lerp(this.points[this.p1n], this.points[this.p2n], this.i);
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00009B88 File Offset: 0x00007D88
	private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
	{
		return 0.5f * (2f * p1 + (-p0 + p2) * i + (2f * p0 - 5f * p1 + 4f * p2 - p3) * i * i + (-p0 + 3f * p1 - 3f * p2 + p3) * i * i * i);
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00009C50 File Offset: 0x00007E50
	private void CachePositionsAndDistances()
	{
		this.points = new Vector3[this.nodes.Count + 1];
		this.distances = new float[this.nodes.Count + 1];
		float num = 0f;
		for (int i = 0; i < this.points.Length; i++)
		{
			Transform transform = this.nodes[i % this.nodes.Count];
			Transform transform2 = this.nodes[(i + 1) % this.nodes.Count];
			if (transform != null && transform2 != null)
			{
				Vector3 position = transform.position;
				Vector3 position2 = transform2.position;
				this.points[i] = this.nodes[i % this.nodes.Count].position;
				this.distances[i] = num;
				num += (position - position2).magnitude;
			}
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00009D49 File Offset: 0x00007F49
	private void OnDrawGizmos()
	{
		this.DrawGizmos(false);
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00009D52 File Offset: 0x00007F52
	private void OnDrawGizmosSelected()
	{
		this.DrawGizmos(true);
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00009D5C File Offset: 0x00007F5C
	private void DrawGizmos(bool selected)
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.nodes = new List<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != base.transform)
			{
				this.nodes.Add(componentsInChildren[i]);
			}
		}
		if (this.nodes.Count > 1)
		{
			this.numPoints = this.nodes.Count;
			this.CachePositionsAndDistances();
			this.Length = this.distances[this.distances.Length - 1];
			Gizmos.color = (selected ? Color.yellow : new Color(1f, 1f, 0f, 0.5f));
			Vector3 from = this.nodes[0].position;
			if (this.smoothRoute)
			{
				for (float num = 0f; num < this.Length; num += this.Length / this.editorVisualisationSubsteps)
				{
					Vector3 routePosition = this.GetRoutePosition(num + 1f);
					Gizmos.DrawLine(from, routePosition);
					from = routePosition;
				}
				Gizmos.DrawLine(from, this.nodes[0].position);
				return;
			}
			for (int j = 0; j < this.nodes.Count; j++)
			{
				Vector3 position = this.nodes[(j + 1) % this.nodes.Count].position;
				Gizmos.DrawLine(from, position);
				from = position;
			}
		}
	}

	// Token: 0x040001DF RID: 479
	[HideInInspector]
	public List<Transform> nodes = new List<Transform>();

	// Token: 0x040001E0 RID: 480
	private int numPoints;

	// Token: 0x040001E1 RID: 481
	private Vector3[] points;

	// Token: 0x040001E2 RID: 482
	private float[] distances;

	// Token: 0x040001E4 RID: 484
	[SerializeField]
	private bool smoothRoute = true;

	// Token: 0x040001E5 RID: 485
	[Range(40f, 200f)]
	public float editorVisualisationSubsteps = 100f;

	// Token: 0x040001E6 RID: 486
	private int p0n;

	// Token: 0x040001E7 RID: 487
	private int p1n;

	// Token: 0x040001E8 RID: 488
	private int p2n;

	// Token: 0x040001E9 RID: 489
	private int p3n;

	// Token: 0x040001EA RID: 490
	private float i;

	// Token: 0x040001EB RID: 491
	private Vector3 P0;

	// Token: 0x040001EC RID: 492
	private Vector3 P1;

	// Token: 0x040001ED RID: 493
	private Vector3 P2;

	// Token: 0x040001EE RID: 494
	private Vector3 P3;

	// Token: 0x02000361 RID: 865
	public struct RoutePoint
	{
		// Token: 0x060015F7 RID: 5623 RVA: 0x000E263C File Offset: 0x000E083C
		public RoutePoint(Vector3 position, Vector3 direction)
		{
			this.position = position;
			this.direction = direction;
		}

		// Token: 0x040026A4 RID: 9892
		public Vector3 position;

		// Token: 0x040026A5 RID: 9893
		public Vector3 direction;
	}
}
