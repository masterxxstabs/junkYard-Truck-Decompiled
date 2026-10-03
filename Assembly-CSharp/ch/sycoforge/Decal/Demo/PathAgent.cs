using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ch.sycoforge.Decal.Demo
{
	// Token: 0x020001B2 RID: 434
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(LineRenderer))]
	public class PathAgent : MonoBehaviour
	{
		// Token: 0x06000AA0 RID: 2720 RVA: 0x0008D90C File Offset: 0x0008BB0C
		private void Start()
		{
			this.TargetAimDecal.gameObject.SetActive(false);
			this.agent = base.GetComponent<NavMeshAgent>();
			this.lineRenderer = base.GetComponent<LineRenderer>();
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0008D938 File Offset: 0x0008BB38
		private void Update()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			this.CreatePath(ray);
			this.SetTarget(ray);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0008D964 File Offset: 0x0008BB64
		private void SetTarget(Ray mouseRay)
		{
			RaycastHit raycastHit;
			if (Input.GetMouseButtonUp(0) && Physics.Raycast(mouseRay, out raycastHit, 50f))
			{
				this.agent.SetDestination(raycastHit.point);
				EasyDecal.ProjectAt(this.TargetPointDecalPrefab, raycastHit.collider.gameObject, raycastHit.point + this.decalOffset, Quaternion.identity);
			}
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0008D9CC File Offset: 0x0008BBCC
		private void CreatePath(Ray mouseRay)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(mouseRay, out raycastHit, 50f))
			{
				Vector3 position = base.transform.position;
				Vector3 point = raycastHit.point;
				this.path.Clear();
				NavMeshPath navMeshPath = new NavMeshPath();
				if (NavMesh.CalculatePath(position, point, -1, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
				{
					int num = navMeshPath.corners.Length;
					Vector3 a = base.transform.up;
					for (int i = 0; i < num; i++)
					{
						RaycastHit raycastHit2;
						if (i > 0 && this.NormalPathOffset > 0f && Physics.Raycast(navMeshPath.corners[i], Vector3.down, out raycastHit2, this.NormalPathOffset * 10f))
						{
							a = raycastHit.normal;
						}
						Vector3 item = navMeshPath.corners[i] + a * this.NormalPathOffset;
						this.path.Add(item);
					}
					Vector3[] array = BezierUtil.InterpolatePath(this.path, 10, this.Radius, this.AngleThreshold).ToArray();
					this.lineRenderer.SetVertexCount(array.Length);
					this.lineRenderer.SetPositions(array);
					this.TargetAimDecal.gameObject.SetActive(true);
					this.TargetAimDecal.gameObject.transform.position = navMeshPath.corners[num - 1] + this.decalOffset;
					return;
				}
			}
			this.TargetAimDecal.gameObject.SetActive(false);
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0008DB4C File Offset: 0x0008BD4C
		private void OnDrawGizmos()
		{
			if (this.DrawGizmos)
			{
				Gizmos.color = Color.red;
				foreach (Vector3 center in this.path)
				{
					Gizmos.DrawSphere(center, 0.05f);
				}
			}
		}

		// Token: 0x04001CAC RID: 7340
		public float PathThickness = 1f;

		// Token: 0x04001CAD RID: 7341
		[Tooltip("Distance from the ground.")]
		public float NormalPathOffset;

		// Token: 0x04001CAE RID: 7342
		[Tooltip("Max radius between segments.")]
		[Range(0.001f, 0.5f)]
		public float Radius = 0.25f;

		// Token: 0x04001CAF RID: 7343
		[Tooltip("Discard segments when their angle is smaller than this value.")]
		public float AngleThreshold = 5f;

		// Token: 0x04001CB0 RID: 7344
		public bool DrawGizmos;

		// Token: 0x04001CB1 RID: 7345
		public EasyDecal TargetAimDecal;

		// Token: 0x04001CB2 RID: 7346
		public GameObject TargetPointDecalPrefab;

		// Token: 0x04001CB3 RID: 7347
		private List<Vector3> path = new List<Vector3>();

		// Token: 0x04001CB4 RID: 7348
		private NavMeshAgent agent;

		// Token: 0x04001CB5 RID: 7349
		private LineRenderer lineRenderer;

		// Token: 0x04001CB6 RID: 7350
		private Vector3 decalOffset = Vector3.up * 0.5f;

		// Token: 0x04001CB7 RID: 7351
		private const int MAXDISTANCE = 50;
	}
}
