using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002AF RID: 687
	[Serializable]
	public class DamageHandler : VehicleComponent
	{
		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x000C51DE File Offset: 0x000C33DE
		// (set) Token: 0x06001248 RID: 4680 RVA: 0x000C51E6 File Offset: 0x000C33E6
		public float Damage { get; private set; }

		// Token: 0x06001249 RID: 4681 RVA: 0x000C51F0 File Offset: 0x000C33F0
		public override void Initialize()
		{
			foreach (MeshFilter meshFilter in this.vc.transform.GetComponentsInChildren<MeshFilter>())
			{
				if (!this._deformableMeshFilters.Contains(meshFilter))
				{
					this._deformableMeshFilters.Add(meshFilter);
					this._originalMeshes.Add(meshFilter.sharedMesh);
				}
			}
			this.initialized = true;
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x000C5254 File Offset: 0x000C3454
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this._collisionEvents.Count != 0)
			{
				DamageHandler.VehicleCollision vehicleCollision = this._collisionEvents.Peek();
				if (vehicleCollision.deformationQueue.Count == 0)
				{
					this._collisionEvents.Dequeue();
					if (this._collisionEvents.Count != 0)
					{
						vehicleCollision = this._collisionEvents.Peek();
					}
				}
				int num = 0;
				while (num < this.deformationVerticesPerFrame && vehicleCollision.deformationQueue.Count > 0)
				{
					MeshFilter meshFilter = vehicleCollision.deformationQueue.Dequeue();
					num += meshFilter.mesh.vertexCount;
					this.MeshDeform(vehicleCollision, meshFilter);
				}
			}
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x000C52F4 File Offset: 0x000C34F4
		public static Vector3 AverageCollisionNormal(ContactPoint[] contacts)
		{
			Vector3[] array = new Vector3[contacts.Length];
			int num = contacts.Length;
			for (int i = 0; i < num; i++)
			{
				array[i] = contacts[i].normal;
			}
			return DamageHandler.AveragePoint(array);
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x000C5334 File Offset: 0x000C3534
		public static Vector3 AverageCollisionPoint(ContactPoint[] contacts)
		{
			Vector3[] array = new Vector3[contacts.Length];
			int num = contacts.Length;
			for (int i = 0; i < num; i++)
			{
				array[i] = contacts[i].point;
			}
			return DamageHandler.AveragePoint(array);
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x000C5374 File Offset: 0x000C3574
		public bool Enqueue(Collision collision, float accelerationMagnitude)
		{
			for (int i = 0; i < this.collisionIgnoreTags.Count; i++)
			{
				string tag = this.collisionIgnoreTags[i];
				if (collision.collider.CompareTag(tag))
				{
					return false;
				}
			}
			DamageHandler.VehicleCollision vehicleCollision = new DamageHandler.VehicleCollision();
			vehicleCollision.collision = collision;
			vehicleCollision.decelerationMagnitude = accelerationMagnitude;
			Vector3 vector = DamageHandler.AverageCollisionPoint(collision.contacts);
			if (!this.visualOnly && this.damageIntensity > 0f)
			{
				this.damageIntensity = ((this.damageIntensity < 0f) ? 0f : ((this.damageIntensity > 0.99f) ? 0.99f : this.damageIntensity));
				float num = collision.impulse.magnitude / (Time.fixedDeltaTime * this.vc.mass * 10f) * this.damageIntensity * 0.002f;
				this.Damage += num;
				this.Damage = ((this.Damage < 0f) ? 0f : ((this.Damage > 1f) ? 1f : this.Damage));
				foreach (WheelComponent wheelComponent in this.vc.Wheels)
				{
					if (Vector3.Distance(vector, wheelComponent.wheelController.worldCenter) < wheelComponent.Radius * 1.5f)
					{
						wheelComponent.Damage += num;
					}
				}
				float magnitude = this.vc.vehicleDimensions.magnitude;
				if (Vector3.Distance(this.vc.WorldEnginePosition, vector) < magnitude * 0.25f)
				{
					this.vc.powertrain.engine.ComponentDamage += num;
				}
				if (Vector3.Distance(this.vc.WorldTransmissionPosition, vector) < magnitude * 0.25f)
				{
					this.vc.powertrain.transmission.ComponentDamage += num;
				}
			}
			if (!this.meshDeform)
			{
				return true;
			}
			foreach (MeshFilter meshFilter in this._deformableMeshFilters)
			{
				string tag2 = meshFilter.gameObject.tag;
				if (tag2 == null)
				{
					vehicleCollision.deformationQueue.Enqueue(meshFilter);
				}
				else
				{
					bool flag = false;
					for (int j = 0; j < this.deformationIgnoreTags.Count; j++)
					{
						if (tag2 == this.deformationIgnoreTags[j])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						vehicleCollision.deformationQueue.Enqueue(meshFilter);
					}
				}
			}
			this._collisionEvents.Enqueue(vehicleCollision);
			return true;
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x000C5658 File Offset: 0x000C3858
		public void HandleCollision(Collision collision)
		{
			if (!base.Active)
			{
				return;
			}
			if (Time.realtimeSinceStartup < this.lastCollisionTime + this.collisionTimeout)
			{
				return;
			}
			float num = collision.relativeVelocity.magnitude * 100f;
			if (num <= this.decelerationThreshold)
			{
				return;
			}
			if (!this.Enqueue(collision, num))
			{
				return;
			}
			this.OnCollision.Invoke(collision);
			this.lastCollision = collision;
			this.lastCollisionTime = Time.realtimeSinceStartup;
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x000C56CC File Offset: 0x000C38CC
		public void MeshDeform(DamageHandler.VehicleCollision collisionEvent, MeshFilter deformableMeshFilter)
		{
			foreach (ContactPoint contactPoint in collisionEvent.collision.contacts)
			{
				Vector3 point = contactPoint.point;
				Vector3 normal = contactPoint.normal;
				float num = Mathf.Clamp(collisionEvent.decelerationMagnitude * this.deformationStrength / 2000f, 0f, this.deformationRadius);
				Vector3[] vertices = deformableMeshFilter.mesh.vertices;
				int num2 = vertices.Length;
				for (int j = 0; j < num2; j++)
				{
					Vector3 vector = deformableMeshFilter.transform.TransformPoint(vertices[j]);
					float num3 = Mathf.Sqrt((point.x - vector.x) * (point.x - vector.x) + (point.z - vector.z) * (point.z - vector.z) + (point.y - vector.y) * (point.y - vector.y));
					num3 *= Random.Range(1f - this.deformationRandomness, 1f + this.deformationRandomness);
					if (num3 < num)
					{
						vector += normal * (num - num3);
						vertices[j] = deformableMeshFilter.transform.InverseTransformPoint(vector);
					}
				}
				deformableMeshFilter.mesh.vertices = vertices;
				deformableMeshFilter.mesh.RecalculateNormals();
				deformableMeshFilter.mesh.RecalculateTangents();
			}
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x000C5850 File Offset: 0x000C3A50
		public void Repair()
		{
			int count = this._deformableMeshFilters.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._originalMeshes[i] != null)
				{
					this._deformableMeshFilters[i].mesh = this._originalMeshes[i];
				}
			}
			foreach (PowertrainComponent powertrainComponent in this.vc.powertrain.solver.Components)
			{
				powertrainComponent.ComponentDamage = 0f;
			}
			foreach (WheelComponent wheelComponent in this.vc.Wheels)
			{
				wheelComponent.wheelController.Damage = 0f;
			}
			this.Damage = 0f;
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x000C5958 File Offset: 0x000C3B58
		private static Vector3 AveragePoint(Vector3[] points)
		{
			Vector3 a = Vector3.zero;
			int num = points.Length;
			for (int i = 0; i < num; i++)
			{
				a += points[i];
			}
			return a / (float)points.Length;
		}

		// Token: 0x040022B7 RID: 8887
		[Tooltip("Collisions with the objects that have a tag that is on this list will be ignored.\r\nCollision state will be changed but no processing will happen.")]
		public List<string> collisionIgnoreTags = new List<string>
		{
			"Wheel"
		};

		// Token: 0x040022B8 RID: 8888
		[Tooltip("Disable repeating collision until the 'collisionTimeout' time has passed. Used to prevent single collision triggering multiple times from minor bumps.")]
		public float collisionTimeout = 0.8f;

		// Token: 0x040022B9 RID: 8889
		[Tooltip("    How much new collisions add to the 'damage' value. Does not affect mesh deformation strength.")]
		public float damageIntensity = 1f;

		// Token: 0x040022BA RID: 8890
		[Tooltip("    Deceleration magnitude needed to trigger damage.")]
		public float decelerationThreshold = 500f;

		// Token: 0x040022BB RID: 8891
		[Tooltip("    Objects that have a tag that is on this list will not have their meshes deformed on collision.")]
		public List<string> deformationIgnoreTags = new List<string>
		{
			"Wheel"
		};

		// Token: 0x040022BC RID: 8892
		[Range(0f, 2f)]
		[Tooltip("    Radius is which vertices will be deformed.")]
		public float deformationRadius = 0.4f;

		// Token: 0x040022BD RID: 8893
		[Range(0.001f, 0.5f)]
		[Tooltip("    Adds noise to the mesh deformation. 0 will result in smooth mesh.")]
		public float deformationRandomness = 0.01f;

		// Token: 0x040022BE RID: 8894
		[Range(0.1f, 5f)]
		[Tooltip("    Determines how much vertices will be deformed for given collision strength.")]
		public float deformationStrength = 1f;

		// Token: 0x040022BF RID: 8895
		[Tooltip("Number of vertices that will be checked and eventually deformed per frame. Setting it to lower values will reduce or remove frame drops but will induce lag into mesh deformation as vehicle will be deformed over longer time span.")]
		public int deformationVerticesPerFrame = 8000;

		// Token: 0x040022C0 RID: 8896
		[Tooltip("    Should meshes be deformed upon collision?")]
		public bool meshDeform = true;

		// Token: 0x040022C1 RID: 8897
		[Tooltip("    Called when a collision happens.")]
		public DamageHandler.VehicleCollisionEvent OnCollision = new DamageHandler.VehicleCollisionEvent();

		// Token: 0x040022C2 RID: 8898
		public List<ParticleSystem> smokeParticleSystems = new List<ParticleSystem>();

		// Token: 0x040022C3 RID: 8899
		[Tooltip("    Should damage affect vehicle performance (steering, power, etc.)?")]
		public bool visualOnly;

		// Token: 0x040022C4 RID: 8900
		[Tooltip("Collision data for the latest collision. Null if no collision yet happened.")]
		public Collision lastCollision;

		// Token: 0x040022C5 RID: 8901
		[Tooltip("Time since startup to the latest collision.")]
		public float lastCollisionTime = -1f;

		// Token: 0x040022C6 RID: 8902
		private Queue<DamageHandler.VehicleCollision> _collisionEvents = new Queue<DamageHandler.VehicleCollision>();

		// Token: 0x040022C7 RID: 8903
		private List<MeshFilter> _deformableMeshFilters = new List<MeshFilter>();

		// Token: 0x040022C8 RID: 8904
		private List<Mesh> _originalMeshes = new List<Mesh>();

		// Token: 0x020004C6 RID: 1222
		public class VehicleCollision
		{
			// Token: 0x04002C25 RID: 11301
			[Tooltip("    Collision data for the collision event.")]
			public Collision collision;

			// Token: 0x04002C26 RID: 11302
			[Tooltip("    Magnitude of the decekeration vector at the moment of impact.")]
			public float decelerationMagnitude;

			// Token: 0x04002C27 RID: 11303
			[Tooltip("Queue of mesh filter components that are waiting for deformation.\r\nSome of the meshes might be queued for checking even if not deformed.")]
			public Queue<MeshFilter> deformationQueue = new Queue<MeshFilter>();
		}

		// Token: 0x020004C7 RID: 1223
		[Serializable]
		public class VehicleCollisionEvent : UnityEvent<Collision>
		{
		}
	}
}
