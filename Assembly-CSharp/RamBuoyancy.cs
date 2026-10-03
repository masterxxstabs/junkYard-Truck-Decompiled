using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000106 RID: 262
[RequireComponent(typeof(Rigidbody))]
public class RamBuoyancy : MonoBehaviour
{
	// Token: 0x060006AA RID: 1706 RVA: 0x00051CEC File Offset: 0x0004FEEC
	private void Start()
	{
		this.rigidbody = base.GetComponent<Rigidbody>();
		if (RamBuoyancy.ramSplines == null)
		{
			RamBuoyancy.ramSplines = Object.FindObjectsOfType<RamSpline>();
		}
		if (RamBuoyancy.lakePolygons == null)
		{
			RamBuoyancy.lakePolygons = Object.FindObjectsOfType<LakePolygon>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.collider == null)
		{
			Debug.LogError("Buoyancy doesn't have collider");
			base.enabled = false;
			return;
		}
		Vector3 size = this.collider.bounds.size;
		Vector3 min = this.collider.bounds.min;
		Vector3 vector = new Vector3(size.x / (float)this.pointsInAxis, size.y / (float)this.pointsInAxis, size.z / (float)this.pointsInAxis);
		for (int i = 0; i <= this.pointsInAxis; i++)
		{
			for (int j = 0; j <= this.pointsInAxis; j++)
			{
				for (int k = 0; k <= this.pointsInAxis; k++)
				{
					Vector3 vector2 = new Vector3(min.x + (float)i * vector.x, min.y + (float)j * vector.y, min.z + (float)k * vector.z);
					if (Vector3.Distance(this.collider.ClosestPoint(vector2), vector2) < 1E-45f)
					{
						this.vertices.Add(base.transform.InverseTransformPoint(vector2));
					}
				}
			}
		}
		this.verticesMatrix = new Vector3[this.vertices.Count];
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00051E88 File Offset: 0x00050088
	private void FixedUpdate()
	{
		this.WaterPhysics();
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00051E90 File Offset: 0x00050090
	public void WaterPhysics()
	{
		Ray ray = default(Ray);
		ray.direction = Vector3.up;
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		this.lowestPoint = this.vertices[0];
		float num = float.MaxValue;
		for (int i = 0; i < this.vertices.Count; i++)
		{
			this.verticesMatrix[i] = localToWorldMatrix.MultiplyPoint3x4(this.vertices[i]);
			if (num > this.verticesMatrix[i].y)
			{
				this.lowestPoint = this.verticesMatrix[i];
				num = this.lowestPoint.y;
			}
		}
		ray.origin = this.lowestPoint;
		this.center = Vector3.zero;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 100f, this.layer))
		{
			Mathf.Max(this.collider.bounds.size.x, this.collider.bounds.size.z);
			int num2 = 0;
			Vector3 velocity = this.rigidbody.velocity;
			Vector3 normalized = velocity.normalized;
			num = raycastHit.point.y;
			for (int j = 0; j < this.verticesMatrix.Length; j++)
			{
				if (this.verticesMatrix[j].y <= num)
				{
					this.center += this.verticesMatrix[j];
					num2++;
				}
			}
			this.center /= (float)num2;
			this.rigidbody.AddForceAtPosition(Vector3.up * this.buoyancy * (num - this.center.y), this.center);
			this.rigidbody.AddForce(velocity * -1f * this.viscosity);
			if (velocity.magnitude > 0.01f)
			{
				Vector3 normalized2 = Vector3.Cross(velocity, new Vector3(1f, 1f, 1f)).normalized;
				Vector3 normalized3 = Vector3.Cross(velocity, normalized2).normalized;
				Vector3 a = velocity.normalized * 10f;
				foreach (Vector3 b in this.verticesMatrix)
				{
					Vector3 origin = a + b;
					Ray ray2 = new Ray(origin, -normalized);
					RaycastHit raycastHit2;
					if (this.collider.Raycast(ray2, out raycastHit2, 50f))
					{
						Vector3 pointVelocity = this.rigidbody.GetPointVelocity(raycastHit2.point);
						this.rigidbody.AddForceAtPosition(-pointVelocity * this.viscosityAngular, raycastHit2.point);
						if (this.debug)
						{
							Debug.DrawRay(raycastHit2.point, -pointVelocity * this.viscosityAngular, Color.red, 0.1f);
						}
					}
				}
			}
			RamSpline component = raycastHit.collider.GetComponent<RamSpline>();
			LakePolygon component2 = raycastHit.collider.GetComponent<LakePolygon>();
			if (component != null)
			{
				Mesh sharedMesh = component.meshfilter.sharedMesh;
				int num3 = sharedMesh.triangles[raycastHit.triangleIndex * 3];
				Vector3 vector = component.verticeDirection[num3];
				Vector2 vector2 = sharedMesh.uv4[num3];
				vector = vector * vector2.y - new Vector3(vector.z, vector.y, -vector.x) * vector2.x;
				this.rigidbody.AddForce(new Vector3(vector.x, 0f, vector.z) * component.floatSpeed);
				if (this.debug)
				{
					Debug.DrawRay(this.center, Vector3.up * this.buoyancy * (num - this.center.y) * 5f, Color.blue);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * -1f * this.viscosity * 5f, Color.magenta);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * 5f, Color.grey);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, this.rigidbody.angularVelocity * 5f, Color.black);
				}
			}
			else if (component2 != null)
			{
				Mesh sharedMesh2 = component2.meshfilter.sharedMesh;
				int num4 = sharedMesh2.triangles[raycastHit.triangleIndex * 3];
				Vector2 vector3 = -sharedMesh2.uv4[num4];
				Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.y);
				this.rigidbody.AddForce(new Vector3(vector4.x, 0f, vector4.z) * component2.floatSpeed);
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position + Vector3.up, vector4 * 5f, Color.red);
				}
				if (this.debug)
				{
					Debug.DrawRay(this.center, Vector3.up * this.buoyancy * (num - this.center.y) * 5f, Color.blue);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * -1f * this.viscosity * 5f, Color.magenta);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, velocity * 5f, Color.grey);
				}
				if (this.debug)
				{
					Debug.DrawRay(base.transform.position, this.rigidbody.angularVelocity * 5f, Color.black);
				}
			}
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00052518 File Offset: 0x00050718
	private void OnDrawGizmosSelected()
	{
		if (!this.debug)
		{
			return;
		}
		if (this.collider != null && this.verticesMatrix != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			foreach (Vector3 vector in this.verticesMatrix)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(vector, 0.08f);
			}
		}
		Vector3 vector2 = this.lowestPoint;
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(this.lowestPoint, 0.08f);
		Vector3 vector3 = this.center;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.center, 0.08f);
	}

	// Token: 0x04000E45 RID: 3653
	public float buoyancy = 30f;

	// Token: 0x04000E46 RID: 3654
	public float viscosity = 2f;

	// Token: 0x04000E47 RID: 3655
	public float viscosityAngular = 0.4f;

	// Token: 0x04000E48 RID: 3656
	public LayerMask layer = 16;

	// Token: 0x04000E49 RID: 3657
	public Collider collider;

	// Token: 0x04000E4A RID: 3658
	[Range(2f, 10f)]
	public int pointsInAxis = 2;

	// Token: 0x04000E4B RID: 3659
	private Rigidbody rigidbody;

	// Token: 0x04000E4C RID: 3660
	private static RamSpline[] ramSplines;

	// Token: 0x04000E4D RID: 3661
	private static LakePolygon[] lakePolygons;

	// Token: 0x04000E4E RID: 3662
	private List<Vector3> vertices = new List<Vector3>();

	// Token: 0x04000E4F RID: 3663
	private Vector3[] verticesMatrix;

	// Token: 0x04000E50 RID: 3664
	private Vector3 lowestPoint;

	// Token: 0x04000E51 RID: 3665
	private Vector3 center = Vector3.zero;

	// Token: 0x04000E52 RID: 3666
	public bool debug;
}
