using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class ImpactDeformable : MonoBehaviour
{
	// Token: 0x1400000C RID: 12
	// (add) Token: 0x060004B0 RID: 1200 RVA: 0x000303E0 File Offset: 0x0002E5E0
	// (remove) Token: 0x060004B1 RID: 1201 RVA: 0x00030418 File Offset: 0x0002E618
	public event Action<ImpactDeformable, Vector3, Vector3> OnDeformForce;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x060004B2 RID: 1202 RVA: 0x00030450 File Offset: 0x0002E650
	// (remove) Token: 0x060004B3 RID: 1203 RVA: 0x00030488 File Offset: 0x0002E688
	public event Action<ImpactDeformable> OnDeform;

	// Token: 0x060004B4 RID: 1204 RVA: 0x000304C0 File Offset: 0x0002E6C0
	private void Awake()
	{
		ImpactDeformable impactDeformable = this.FindMaster();
		if (impactDeformable != null && !this.OverrideMaster)
		{
			this.RecalculateNormals = impactDeformable.RecalculateNormals;
			this.Hardness = impactDeformable.Hardness;
			this.MaxVertexMov = impactDeformable.MaxVertexMov;
			this.RandomFactorDeformation = impactDeformable.RandomFactorDeformation;
			this.DeformationsScale = impactDeformable.DeformationsScale;
			this.DeformMeshCollider = impactDeformable.DeformMeshCollider;
		}
		if (this.MeshFilter == null)
		{
			this.MeshFilter = base.GetComponent<MeshFilter>();
		}
		this.UpdateMeshFilter();
		this.meshCollider = base.GetComponent<MeshCollider>();
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0003055C File Offset: 0x0002E75C
	public ImpactDeformable FindMaster()
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			return null;
		}
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			ImpactDeformable component = parent.GetComponent<ImpactDeformable>();
			Rigidbody component2 = parent.GetComponent<Rigidbody>();
			if (component && component2)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x000305B8 File Offset: 0x0002E7B8
	private bool UpdateMeshFilter()
	{
		if (this.deformedMeshFilter == this.MeshFilter)
		{
			return this.deformedMesh != null;
		}
		this.deformedMeshFilter = this.MeshFilter;
		if (this.deformedMeshFilter == null)
		{
			this.meshCache = null;
			this.deformedMesh = null;
			this.deformedVertices = null;
			this.deformedNormals = null;
			return false;
		}
		this.meshCache = MeshCache.GetMeshCache(this.deformedMeshFilter.sharedMesh);
		this.deformedMesh = this.deformedMeshFilter.mesh;
		this.deformedVertices = this.deformedMesh.vertices;
		this.deformedNormals = this.deformedMesh.normals;
		return true;
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x00030667 File Offset: 0x0002E867
	private void OnCollisionEnter(Collision col)
	{
		this.ProcessCollision(col);
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00030667 File Offset: 0x0002E867
	private void OnCollisionStay(Collision col)
	{
		this.ProcessCollision(col);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00030670 File Offset: 0x0002E870
	private void ProcessCollision(Collision col)
	{
		for (int i = 0; i < col.contacts.Length; i++)
		{
			(col.contacts[i].thisCollider.GetComponent<ImpactDeformable>() ?? this).ProcessContactPoint(col.contacts[i].point, col.relativeVelocity, col.contacts[i].normal);
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x000306D8 File Offset: 0x0002E8D8
	private void ProcessContactPoint(Vector3 point, Vector3 relativeVelocity, Vector3 normal)
	{
		if (!this.UpdateMeshFilter())
		{
			return;
		}
		if ((double)(relativeVelocity.sqrMagnitude / Mathf.Max(this.Hardness, 0.01f)) < 0.25)
		{
			return;
		}
		float num = Vector3.Dot(relativeVelocity.normalized, normal);
		if (num <= 0f)
		{
			return;
		}
		Vector3 force = num * relativeVelocity.magnitude * normal * 0.02f;
		this.Deform(point, force);
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0003074C File Offset: 0x0002E94C
	public void Deform(Vector3 point, Vector3 force)
	{
		if (!this.UpdateMeshFilter())
		{
			return;
		}
		point = this.MeshFilter.transform.InverseTransformPoint(point);
		if (this.Hardness < 0.01f)
		{
			this.Hardness = 0.01f;
		}
		this.RandomFactorDeformation = Mathf.Clamp01(this.RandomFactorDeformation);
		force = this.MeshFilter.transform.InverseTransformDirection(force) * (1f / this.Hardness);
		if (this.MeshFilter.transform.localScale != Vector3.one)
		{
			Vector3 localScale = this.MeshFilter.transform.localScale;
			force.Scale(new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z));
		}
		float num = force.magnitude;
		if (this.MaxDeformationRadius > 0f)
		{
			num = Mathf.Min(num, this.MaxDeformationRadius);
		}
		if (num < 0.025f)
		{
			return;
		}
		num *= num;
		this.deformedVerticesIndex.Clear();
		int i = this.deformedVertices.Length - 1;
		bool flag = false;
		Vector3 scale = this.MeshFilter.transform.InverseTransformVector(this.DeformationsScale);
		while (i > 0)
		{
			int num2 = i;
			float sqrMagnitude = (this.deformedVertices[num2] - point).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				flag = true;
				Vector3 vector = force * (1f - sqrMagnitude / num);
				if (this.RandomFactorDeformation > 0f)
				{
					vector = vector * (1f - this.RandomFactorDeformation) + vector * Random.value * this.RandomFactorDeformation;
				}
				if (this.DeformationsScale != Vector3.one)
				{
					vector.Scale(scale);
				}
				this.deformedVertices[num2] += vector;
				if (this.LimitDeformationToMeshBounds && !this.meshCache.Bounds.Contains(this.deformedVertices[num2]))
				{
					this.deformedVertices[num2] = this.meshCache.Bounds.ClosestPoint(this.deformedVertices[num2]);
				}
				if (this.MaxVertexMov > 0f)
				{
					Vector3 a = this.deformedVertices[num2] - this.meshCache.Vertices[num2];
					if (a.sqrMagnitude > this.MaxVertexMov * this.MaxVertexMov)
					{
						this.deformedVertices[num2] = this.meshCache.Vertices[num2] + a * (this.MaxVertexMov / a.magnitude);
					}
				}
				this.deformedVerticesIndex.Add(num2);
			}
			i--;
		}
		if (flag)
		{
			if (Application.isEditor)
			{
				this.ImmediateApplyChanges(false);
			}
			else
			{
				this.ApplyChangesToMesh();
			}
			if (this.OnDeformForce != null)
			{
				this.OnDeformForce(this, point, force);
			}
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00030A4D File Offset: 0x0002EC4D
	private void ApplyChangesToMesh()
	{
		if (this.applyingChanges)
		{
			return;
		}
		this.applyingChanges = true;
		base.StartCoroutine(this.DeferredApplyChanges());
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00030A6C File Offset: 0x0002EC6C
	private IEnumerator DeferredApplyChanges()
	{
		yield return new WaitForEndOfFrame();
		this.applyingChanges = false;
		this.ImmediateApplyChanges(false);
		yield break;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00030A7C File Offset: 0x0002EC7C
	private void ImmediateApplyChanges(bool applyDeformedNormals = false)
	{
		this.structuralDamage = -1f;
		this.deformedMesh.MarkDynamic();
		this.deformedMesh.vertices = this.deformedVertices;
		this.deformedMesh.RecalculateBounds();
		if (this.DeformMeshCollider)
		{
			if (this.meshCollider != null)
			{
				this.meshCollider.sharedMesh = null;
				this.meshCollider.sharedMesh = this.deformedMesh;
				this.meshCollider.sharedMesh.UploadMeshData(false);
			}
		}
		else if (this.meshCollider != null && this.meshCollider.sharedMesh == this.deformedMesh)
		{
			this.meshCollider.sharedMesh = null;
			Mesh mesh = new Mesh();
			mesh.vertices = this.meshCache.Vertices;
			mesh.triangles = this.meshCache.Triangles;
			this.meshCollider.sharedMesh = mesh;
		}
		if (this.RecalculateNormals)
		{
			if (!applyDeformedNormals)
			{
				this.RecalcNormalsForDeformedVertices();
			}
			else
			{
				this.deformedMesh.normals = this.deformedNormals;
			}
		}
		this.deformedMesh.UploadMeshData(false);
		if (this.OnDeform != null)
		{
			this.OnDeform(this);
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00030BB0 File Offset: 0x0002EDB0
	private void RecalcNormalsForDeformedVertices()
	{
		List<int> list = this.meshCache.FindConnectedVertices(this.deformedVerticesIndex);
		this.deformedMesh.RecalculateNormals();
		Vector3[] normals = this.deformedMesh.normals;
		foreach (int num in list)
		{
			this.deformedNormals[num] = normals[num];
		}
		this.deformedMesh.normals = this.deformedNormals;
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060004C0 RID: 1216 RVA: 0x00030C44 File Offset: 0x0002EE44
	public float StructuralDamage
	{
		get
		{
			if (this.structuralDamage == -1f)
			{
				if (this.deformedMesh == null)
				{
					return 0f;
				}
				Vector3 a = Vector3.zero;
				for (int i = 0; i < this.deformedVertices.Length; i++)
				{
					Vector3 vector = this.deformedVertices[i];
					Vector3 vector2 = this.meshCache.Vertices[i];
					a += new Vector3(Mathf.Abs(vector.x - vector2.x), Mathf.Abs(vector.y - vector2.y), Mathf.Abs(vector.z - vector2.z));
				}
				a /= (float)this.deformedVertices.Length;
				a.Scale(this.meshCache.SizeFactor);
				this.structuralDamage = a.magnitude;
			}
			return this.structuralDamage;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00030D24 File Offset: 0x0002EF24
	public float AverageStructuralDamage
	{
		get
		{
			return (from i in base.transform.GetComponentsInChildren<ImpactDeformable>()
			where i.MeshFilter != null
			select i.StructuralDamage).DefaultIfEmpty(0f).Average();
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x00030D94 File Offset: 0x0002EF94
	public void Repair(float percentual, Vector3? point = null, float? radius = null)
	{
		if (!this.UpdateMeshFilter())
		{
			return;
		}
		bool flag = false;
		this.deformedVerticesIndex.Clear();
		Vector3 b = Vector3.zero;
		float num = 0f;
		if (point != null && radius != null)
		{
			num = radius.Value;
			num *= num;
			b = this.MeshFilter.transform.InverseTransformPoint(point.Value);
		}
		for (int i = 0; i < this.deformedVertices.Length; i++)
		{
			this.deformedNormals[i] = Vector3.Lerp(this.deformedNormals[i], this.meshCache.Normals[i], percentual);
			if (this.deformedVertices[i] - this.meshCache.Vertices[i] != Vector3.zero && (num <= 0f || (this.deformedVertices[i] - b).sqrMagnitude < num))
			{
				this.deformedVertices[i] = Vector3.Lerp(this.deformedVertices[i], this.meshCache.Vertices[i], percentual);
				flag = true;
			}
		}
		if (flag)
		{
			this.ImmediateApplyChanges(true);
		}
	}

	// Token: 0x0400096D RID: 2413
	public float Hardness = 1f;

	// Token: 0x0400096E RID: 2414
	public float MaxDeformationRadius;

	// Token: 0x0400096F RID: 2415
	public float MaxVertexMov;

	// Token: 0x04000970 RID: 2416
	public float RandomFactorDeformation;

	// Token: 0x04000971 RID: 2417
	public Vector3 DeformationsScale = Vector3.one;

	// Token: 0x04000972 RID: 2418
	public bool DeformMeshCollider;

	// Token: 0x04000973 RID: 2419
	public bool LimitDeformationToMeshBounds = true;

	// Token: 0x04000974 RID: 2420
	public bool RecalculateNormals = true;

	// Token: 0x04000975 RID: 2421
	[HideInInspector]
	public ImpactDeformable Master;

	// Token: 0x04000976 RID: 2422
	[HideInInspector]
	public bool OverrideMaster;

	// Token: 0x04000979 RID: 2425
	[HideInInspector]
	public MeshFilter MeshFilter;

	// Token: 0x0400097A RID: 2426
	private MeshFilter deformedMeshFilter;

	// Token: 0x0400097B RID: 2427
	private Mesh deformedMesh;

	// Token: 0x0400097C RID: 2428
	private MeshCache meshCache;

	// Token: 0x0400097D RID: 2429
	private Vector3[] deformedVertices;

	// Token: 0x0400097E RID: 2430
	private Vector3[] deformedNormals;

	// Token: 0x0400097F RID: 2431
	private MeshCollider meshCollider;

	// Token: 0x04000980 RID: 2432
	private List<int> deformedVerticesIndex = new List<int>();

	// Token: 0x04000981 RID: 2433
	private float structuralDamage;

	// Token: 0x04000982 RID: 2434
	private bool applyingChanges;
}
