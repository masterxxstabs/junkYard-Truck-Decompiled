using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class MeshCollisionScriptAI : MonoBehaviour
{
	// Token: 0x060000AD RID: 173 RVA: 0x000095E2 File Offset: 0x000077E2
	private void Start()
	{
		this.sqrDemRange = this.demolutionRange * this.demolutionRange;
		if (this.loseAftCollisions == 0)
		{
			this.fixedMesh = true;
		}
		else
		{
			this.fixedMesh = false;
		}
		this.collisionParticlesPrefab = Resources.Load<Transform>("CollisionParticles");
	}

	// Token: 0x060000AE RID: 174 RVA: 0x0000961F File Offset: 0x0000781F
	private void FixedUpdate()
	{
		if (this.collisionHappened)
		{
			this.CollisionCalculator();
		}
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00009630 File Offset: 0x00007830
	public void OnTriggerEnter(Collider objCollided)
	{
		if (objCollided)
		{
			this.loseAftCollisions--;
			if (!this.fixedMesh && this.loseAftCollisions <= 0)
			{
				base.transform.parent = null;
				base.StartCoroutine(this.LoseObjectCoroutine());
			}
			this.collisionHappened = true;
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00009684 File Offset: 0x00007884
	private void CollisionCalculator()
	{
		if (this.hitPoint != null)
		{
			if (this.collisionParticlesON && this.collisionParticles == null)
			{
				this.collisionParticles = Object.Instantiate<Transform>(this.collisionParticlesPrefab);
				this.collisionParticles.GetComponent<ParticleSystem>().Emit(10);
				this.collisionParticles.transform.position = this.hitPoint.contacts[0].point;
				Object.Destroy(this.collisionParticles.gameObject, 5f);
			}
			Vector3 a = this.hitPoint.relativeVelocity;
			a *= this.yForceDamp;
			Vector3 vector = base.transform.position - this.hitPoint.contacts[0].point;
			float num = a.magnitude * Vector3.Dot(this.hitPoint.contacts[0].normal, vector.normalized);
			this.OnMeshForce(this.hitPoint.contacts[0].point, Mathf.Clamp01(num / this.maxCollisionStrength));
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x000097A4 File Offset: 0x000079A4
	public IEnumerator LoseObjectCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		base.transform.gameObject.GetComponent<MeshCollider>().isTrigger = false;
		if (base.transform.gameObject.GetComponent<Rigidbody>() == null)
		{
			base.transform.gameObject.AddComponent<Rigidbody>();
		}
		base.transform.gameObject.GetComponent<Rigidbody>().mass = 50f;
		yield break;
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x000097B4 File Offset: 0x000079B4
	public void OnMeshForce(Vector3 originPos, float force)
	{
		force = Mathf.Clamp01(force);
		Vector3[] vertices = this.meshFilter.mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 point = Vector3.Scale(vertices[i], base.transform.localScale);
			Vector3 vector = this.meshFilter.transform.position + this.meshFilter.transform.rotation * point;
			Vector3 a = vector - originPos;
			Vector3 b = base.transform.position - vector;
			b.y = 0f;
			if (a.sqrMagnitude < this.sqrDemRange)
			{
				float num = Mathf.Clamp01(a.sqrMagnitude / this.sqrDemRange);
				float d = force * (1f - num) * this.maxMoveDelta;
				Vector3 point2 = Vector3.Slerp(a, b, this.impactDirManipulator).normalized * d;
				vertices[i] += Quaternion.Inverse(base.transform.rotation) * point2;
			}
		}
		this.meshFilter.mesh.vertices = vertices;
		this.meshFilter.mesh.RecalculateBounds();
		this.hitPoint = null;
		this.collisionHappened = false;
	}

	// Token: 0x040001D0 RID: 464
	[HideInInspector]
	public float maxCollisionStrength = 50f;

	// Token: 0x040001D1 RID: 465
	[HideInInspector]
	public float demolutionRange = 100f;

	// Token: 0x040001D2 RID: 466
	[HideInInspector]
	public MeshFilter meshFilter;

	// Token: 0x040001D3 RID: 467
	private float maxMoveDelta = 1.5f;

	// Token: 0x040001D4 RID: 468
	private float yForceDamp = 1f;

	// Token: 0x040001D5 RID: 469
	private float impactDirManipulator = 0.5f;

	// Token: 0x040001D6 RID: 470
	private float sqrDemRange;

	// Token: 0x040001D7 RID: 471
	[HideInInspector]
	public Collision hitPoint;

	// Token: 0x040001D8 RID: 472
	[HideInInspector]
	public bool collisionHappened;

	// Token: 0x040001D9 RID: 473
	[HideInInspector]
	public int loseAftCollisions;

	// Token: 0x040001DA RID: 474
	private bool fixedMesh = true;

	// Token: 0x040001DB RID: 475
	[HideInInspector]
	public bool collisionParticlesON;

	// Token: 0x040001DC RID: 476
	private Transform collisionParticles;

	// Token: 0x040001DD RID: 477
	private Transform collisionParticlesPrefab;
}
