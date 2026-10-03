using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000338 RID: 824
	public class WaypointCircuit : MonoBehaviour
	{
		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06001511 RID: 5393 RVA: 0x000DEBDF File Offset: 0x000DCDDF
		// (set) Token: 0x06001512 RID: 5394 RVA: 0x000DEBE7 File Offset: 0x000DCDE7
		public float Length { get; private set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06001513 RID: 5395 RVA: 0x000DEBF0 File Offset: 0x000DCDF0
		public Transform[] Waypoints
		{
			get
			{
				return this.waypointList.items;
			}
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x000DEBFD File Offset: 0x000DCDFD
		private void Awake()
		{
			if (this.Waypoints.Length > 1)
			{
				this.CachePositionsAndDistances();
			}
			this.numPoints = this.Waypoints.Length;
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x000DEC20 File Offset: 0x000DCE20
		public WaypointCircuit.RoutePoint GetRoutePoint(float dist)
		{
			Vector3 routePosition = this.GetRoutePosition(dist);
			return new WaypointCircuit.RoutePoint(routePosition, (this.GetRoutePosition(dist + 0.1f) - routePosition).normalized);
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x000DEC58 File Offset: 0x000DCE58
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

		// Token: 0x06001517 RID: 5399 RVA: 0x000DEE00 File Offset: 0x000DD000
		private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
		{
			return 0.5f * (2f * p1 + (-p0 + p2) * i + (2f * p0 - 5f * p1 + 4f * p2 - p3) * i * i + (-p0 + 3f * p1 - 3f * p2 + p3) * i * i * i);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x000DEEC8 File Offset: 0x000DD0C8
		public int ClosestWaypointNum(Transform from)
		{
			Transform transform = this.Waypoints[0];
			float num = Vector3.Distance(from.position, transform.position);
			foreach (Transform transform2 in this.Waypoints)
			{
				float num2 = Vector3.Distance(from.position, transform2.position);
				if (num2 < num)
				{
					transform = transform2;
					num = num2;
				}
			}
			return transform.GetSiblingIndex();
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x000DEF30 File Offset: 0x000DD130
		public void AddWaypointsFromChildren()
		{
			Transform[] array = new Transform[this.transform.childCount];
			for (int i = 0; i < this.transform.childCount; i++)
			{
				array[i] = this.transform.GetChild(i);
			}
			this.waypointList.items = new Transform[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				this.waypointList.items[j] = array[j];
			}
			base.transform.name = "WaypointCircuit";
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x000DEFB8 File Offset: 0x000DD1B8
		private void CachePositionsAndDistances()
		{
			this.points = new Vector3[this.Waypoints.Length + 1];
			this.distances = new float[this.Waypoints.Length + 1];
			float num = 0f;
			for (int i = 0; i < this.points.Length; i++)
			{
				Transform transform = this.Waypoints[i % this.Waypoints.Length];
				Transform transform2 = this.Waypoints[(i + 1) % this.Waypoints.Length];
				if (transform != null && transform2 != null)
				{
					Vector3 position = transform.position;
					Vector3 position2 = transform2.position;
					this.points[i] = this.Waypoints[i % this.Waypoints.Length].position;
					this.distances[i] = num;
					num += (position - position2).magnitude;
				}
			}
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x000DF096 File Offset: 0x000DD296
		private void OnDrawGizmos()
		{
			this.DrawGizmos(false);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x000DF09F File Offset: 0x000DD29F
		private void OnDrawGizmosSelected()
		{
			this.DrawGizmos(true);
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x000DF0A8 File Offset: 0x000DD2A8
		private void DrawGizmos(bool selected)
		{
			this.waypointList.circuit = this;
			if (this.Waypoints.Length > 1)
			{
				this.numPoints = this.Waypoints.Length;
				this.CachePositionsAndDistances();
				this.Length = this.distances[this.distances.Length - 1];
				Gizmos.color = (selected ? Color.yellow : Color.yellow);
				Vector3 from = this.Waypoints[0].position;
				if (this.smoothRoute)
				{
					for (float num = 0f; num < this.Length; num += this.Length / this.editorVisualisationSubsteps)
					{
						Vector3 routePosition = this.GetRoutePosition(num + 1f);
						Gizmos.DrawLine(from, routePosition);
						from = routePosition;
					}
					Gizmos.DrawLine(from, this.Waypoints[0].position);
				}
				else
				{
					for (int i = 0; i < this.Waypoints.Length; i++)
					{
						Vector3 position = this.Waypoints[(i + 1) % this.Waypoints.Length].position;
						Gizmos.DrawLine(from, position);
						from = position;
					}
				}
			}
			foreach (Transform transform in this.Waypoints)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawSphere(transform.position, 1f);
			}
		}

		// Token: 0x040025BB RID: 9659
		public WaypointCircuit.WaypointList waypointList = new WaypointCircuit.WaypointList();

		// Token: 0x040025BC RID: 9660
		[SerializeField]
		private bool smoothRoute = true;

		// Token: 0x040025BD RID: 9661
		private int numPoints;

		// Token: 0x040025BE RID: 9662
		private Vector3[] points;

		// Token: 0x040025BF RID: 9663
		private float[] distances;

		// Token: 0x040025C0 RID: 9664
		[Range(100f, 500f)]
		public float editorVisualisationSubsteps = 100f;

		// Token: 0x040025C2 RID: 9666
		private int p0n;

		// Token: 0x040025C3 RID: 9667
		private int p1n;

		// Token: 0x040025C4 RID: 9668
		private int p2n;

		// Token: 0x040025C5 RID: 9669
		private int p3n;

		// Token: 0x040025C6 RID: 9670
		private float i;

		// Token: 0x040025C7 RID: 9671
		private Vector3 P0;

		// Token: 0x040025C8 RID: 9672
		private Vector3 P1;

		// Token: 0x040025C9 RID: 9673
		private Vector3 P2;

		// Token: 0x040025CA RID: 9674
		private Vector3 P3;

		// Token: 0x020004F1 RID: 1265
		[Serializable]
		public class WaypointList
		{
			// Token: 0x04002CDC RID: 11484
			public WaypointCircuit circuit;

			// Token: 0x04002CDD RID: 11485
			public Transform[] items = new Transform[0];
		}

		// Token: 0x020004F2 RID: 1266
		public struct RoutePoint
		{
			// Token: 0x06001B8B RID: 7051 RVA: 0x000F933F File Offset: 0x000F753F
			public RoutePoint(Vector3 position, Vector3 direction)
			{
				this.position = position;
				this.direction = direction;
			}

			// Token: 0x04002CDE RID: 11486
			public Vector3 position;

			// Token: 0x04002CDF RID: 11487
			public Vector3 direction;
		}
	}
}
