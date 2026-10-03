using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A7 RID: 423
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider))]
	public class WaveMakerSurface : MonoBehaviour
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x0008B23D File Offset: 0x0008943D
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x0008B245 File Offset: 0x00089445
		public float Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = Mathf.Clamp(value, 0.001f, float.MaxValue);
				this.Initialize();
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x0008B264 File Offset: 0x00089464
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x0008B26C File Offset: 0x0008946C
		public float Depth
		{
			get
			{
				return this._depth;
			}
			set
			{
				this._depth = Mathf.Clamp(value, 0.001f, float.MaxValue);
				this.Initialize();
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x0008B28B File Offset: 0x0008948B
		public float SampleSizeX
		{
			get
			{
				return this._sampleSizeX;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x0008B293 File Offset: 0x00089493
		public float SampleSizeZ
		{
			get
			{
				return this._sampleSizeZ;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000A4F RID: 2639 RVA: 0x0008B29B File Offset: 0x0008949B
		public int ResolutionX
		{
			get
			{
				return this._resolutionX;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000A50 RID: 2640 RVA: 0x0008B2A3 File Offset: 0x000894A3
		public int ResolutionZ
		{
			get
			{
				return this._resolutionZ;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x0008B2AB File Offset: 0x000894AB
		public int ResolutionXGhost
		{
			get
			{
				return this._resolutionXGhost;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x0008B2B3 File Offset: 0x000894B3
		public int ResolutionZGhost
		{
			get
			{
				return this._resolutionZGhost;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000A53 RID: 2643 RVA: 0x0008B2BB File Offset: 0x000894BB
		public bool IsAwake
		{
			get
			{
				return this._isAwake;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000A54 RID: 2644 RVA: 0x0008B2C3 File Offset: 0x000894C3
		public MeshRenderer Renderer
		{
			get
			{
				if (this._meshRenderer == null)
				{
					this._meshRenderer = base.GetComponent<MeshRenderer>();
				}
				return this._meshRenderer;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000A55 RID: 2645 RVA: 0x0008B2E5 File Offset: 0x000894E5
		public MeshFilter MeshFilter
		{
			get
			{
				if (this._meshFilter == null)
				{
					this._meshFilter = base.GetComponent<MeshFilter>();
				}
				return this._meshFilter;
			}
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x0008B308 File Offset: 0x00089508
		private void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.Descriptor == null || !this.Descriptor.IsInitialized)
			{
				Debug.LogError("WaveMaker - (" + base.gameObject.name + ") cannot be initialized. No Descriptor is attached in that gameobject or could not be initialized.");
				this._initialized = false;
				return;
			}
			this.Initialize();
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x0008B368 File Offset: 0x00089568
		private void FixedUpdate()
		{
			if (!this._initialized)
			{
				return;
			}
			this.UpdateAwakeStatus();
			this.UpdateCollidingObjectsInteraction();
			if (!this._isAwake)
			{
				return;
			}
			for (int i = 0; i < this.substeps; i++)
			{
				this.UpdateDataGrid();
			}
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0008B3AA File Offset: 0x000895AA
		private void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			if (!this._isAwake)
			{
				return;
			}
			this.UpdateMesh();
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x0008B3C4 File Offset: 0x000895C4
		private void OnDestroy()
		{
			this.Uninitialize();
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x0008B3CC File Offset: 0x000895CC
		private void OnCollisionEnter(Collision collision)
		{
			this.DetectCollisionStart(collision.collider);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x0008B3DA File Offset: 0x000895DA
		private void OnTriggerEnter(Collider other)
		{
			this.DetectCollisionStart(other);
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0008B3E4 File Offset: 0x000895E4
		private void DetectCollisionStart(Collider other)
		{
			WaveMakerInteractor component = other.GetComponent<WaveMakerInteractor>();
			if (component == null)
			{
				return;
			}
			this._interactorsDetectedColliders.Add(component.GetComponent<Collider>());
			this._interactorsDetected.Add(component);
			component.UpdateVelocities();
			this.UpdateVelocities();
			if (this.showLogMessages)
			{
				Debug.Log(string.Concat(new object[]
				{
					"WaveMaker - Interactor detected: ",
					component.gameObject.name,
					" . Interactors detected now : ",
					this._interactorsDetected.Count
				}));
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0008B474 File Offset: 0x00089674
		private void OnCollisionExit(Collision collision)
		{
			this.DetectCollisionEnd(collision.collider);
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0008B482 File Offset: 0x00089682
		private void OnTriggerExit(Collider other)
		{
			this.DetectCollisionEnd(other);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0008B48C File Offset: 0x0008968C
		private void DetectCollisionEnd(Collider other)
		{
			WaveMakerInteractor component = other.GetComponent<WaveMakerInteractor>();
			if (component == null)
			{
				return;
			}
			int index = this._interactorsDetected.IndexOf(component);
			this._interactorsDetected.RemoveAt(index);
			this._interactorsDetectedColliders.RemoveAt(index);
			if (this.showLogMessages)
			{
				Debug.Log(string.Concat(new object[]
				{
					"WaveMaker - Stop detecting interactor : ",
					component.gameObject.name,
					" . Interactors detected now : ",
					this._interactorsDetected.Count
				}));
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0008B518 File Offset: 0x00089718
		private void OnValidate()
		{
			if (this.substeps < 1)
			{
				this.substeps = 1;
			}
			if (this.propagationSpeed < 0f)
			{
				this.propagationSpeed = 0f;
			}
			if (this.verticalPushScale < 0f)
			{
				this.verticalPushScale = 0f;
			}
			if (this.horizontalPushScale < 0f)
			{
				this.horizontalPushScale = 0f;
			}
			if (this.interactorSpeedClamp < 0f)
			{
				this.interactorSpeedClamp = 0f;
			}
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x0008B595 File Offset: 0x00089795
		private void UpdateAwakeStatus()
		{
			this._isAwake = (this._cineticEnergy > this._sleepThreshold);
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0008B5AC File Offset: 0x000897AC
		private void UpdateCollidingObjectsInteraction()
		{
			if (this._interactorsDetected.Count <= 0)
			{
				return;
			}
			this.UpdateVelocities();
			float fixedDeltaTime = Time.fixedDeltaTime;
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			Quaternion rotation = base.transform.worldToLocalMatrix.rotation;
			for (int i = 0; i < this._interactorsDetected.Count; i++)
			{
				WaveMakerInteractor waveMakerInteractor = this._interactorsDetected[i];
				waveMakerInteractor.UpdateVelocities();
				Matrix4x4 worldToLocalMatrix = waveMakerInteractor.transform.worldToLocalMatrix;
				Collider collider = this._interactorsDetectedColliders[i];
				Bounds bounds = WaveMakerUtils.TransformBounds(collider.bounds, base.transform.worldToLocalMatrix);
				int num;
				int num2;
				this.GetNearestSample(bounds.min.x, bounds.min.z, out num, out num2);
				int num3;
				int num4;
				this.GetNearestSample(bounds.max.x, bounds.max.z, out num3, out num4);
				if (bounds.min.x > 0f)
				{
					num++;
				}
				if (bounds.min.z > 0f)
				{
					num2++;
				}
				Vector3 centerOfMass = waveMakerInteractor.CenterOfMass;
				Vector3 position = base.transform.position;
				for (int j = num; j <= num3; j++)
				{
					for (int k = num2; k <= num4; k++)
					{
						if (!this.FixedGridRef[k * this._resolutionX + j])
						{
							Vector3 positionFromSample = this.GetPositionFromSample(j, k, false, false, false);
							Vector3 point = localToWorldMatrix.MultiplyPoint(positionFromSample);
							if (WaveMakerUtils.IsPointInsideCollider(collider, point))
							{
								worldToLocalMatrix.MultiplyPoint(point);
								Vector3 a = WaveMakerUtils.VelocityAtPoint(point, centerOfMass, waveMakerInteractor.AngularVelocity, waveMakerInteractor.LinearVelocity);
								Vector3 b = WaveMakerUtils.VelocityAtPoint(point, position, this._angularVelocity, this._linearVelocity);
								Vector3 vector = rotation * (a - b);
								float magnitude = vector.magnitude;
								if (magnitude >= this._sleepThreshold)
								{
									if (magnitude > this.interactorSpeedClamp)
									{
										vector.Normalize();
										vector *= this.interactorSpeedClamp;
									}
									this.SetHeight_FullCheck(j, k, vector.y * this.verticalPushScale * fixedDeltaTime, true);
									float num5 = (vector.x > 0f) ? vector.x : (-vector.x);
									float num6 = (vector.z > 0f) ? vector.z : (-vector.z);
									int num7 = (vector.x > 0f) ? 1 : -1;
									int num8 = (vector.z > 0f) ? 1 : -1;
									vector.y = 0f;
									num7 = ((num5 < this.speedThreshold * fixedDeltaTime) ? 0 : num7);
									num8 = ((num6 < this.speedThreshold * fixedDeltaTime) ? 0 : num8);
									if (num7 != 0 || num8 != 0)
									{
										int num9 = j + num7;
										int num10 = k + num8;
										if (num9 >= 0 && num9 < this._resolutionX && num10 >= 0 && num10 < this._resolutionZ && !this.FixedGridRef[num9 * this._resolutionX + num10])
										{
											this.SetHeight_FullCheck(num9, num10, vector.magnitude * this.horizontalPushScale * fixedDeltaTime, true);
										}
										int num11 = j - num7;
										int num12 = k - num8;
										if (num11 >= 0 && num11 < this._resolutionX && num12 >= 0 && num12 < this._resolutionZ && !this.FixedGridRef[num11 * this._resolutionX + num12])
										{
											this.SetHeight_FullCheck(num11, num12, -vector.magnitude * this.horizontalPushScale * fixedDeltaTime, true);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x0008B954 File Offset: 0x00089B54
		private void GenerateDataGrids()
		{
			this.heights = new float[this._resolutionXGhost, this._resolutionZGhost];
			this.velocities = new float[this._resolutionX, this._resolutionZ];
			this.accelerations = new float[this._resolutionX, this._resolutionZ];
			for (int i = 0; i < this._resolutionXGhost; i++)
			{
				for (int j = 0; j < this._resolutionZGhost; j++)
				{
					this.heights[i, j] = 0f;
					if (i < this._resolutionX && j < this._resolutionZ)
					{
						this.velocities[i, j] = 0f;
						this.accelerations[i, j] = 0f;
					}
				}
			}
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x0008BA10 File Offset: 0x00089C10
		private void GenerateMesh()
		{
			if (this._meshFilter == null)
			{
				return;
			}
			Object.DestroyImmediate(this.mesh);
			this.mesh = new Mesh();
			int num = this._resolutionX - 1;
			int num2 = this._resolutionZ - 1;
			int num3 = this._resolutionX * this._resolutionZ;
			this.vertices = new Vector3[num3];
			Vector3[] array = new Vector3[num3];
			Vector2[] array2 = new Vector2[num3];
			int[] array3 = new int[6 * num * num2];
			float num4 = 1f / (float)num;
			float num5 = 1f / (float)num2;
			this._sampleSizeX = this.Width / (float)num;
			this._sampleSizeZ = this.Depth / (float)num2;
			int num6 = 0;
			for (int i = 0; i < this._resolutionZ; i++)
			{
				for (int j = 0; j < this._resolutionX; j++)
				{
					int num7 = i * this._resolutionX + j;
					this.vertices[num7] = new Vector3((float)j * this._sampleSizeX, 0f, (float)i * this._sampleSizeZ);
					array[num7] = Vector3.up;
					array2[num7] = new Vector2((float)j * num4, (float)i * num5);
					if (j != this._resolutionX - 1 && i != this._resolutionZ - 1)
					{
						int num8 = num7 + 1;
						int num9 = num7 + this._resolutionX;
						int num10 = num9 + 1;
						array3[num6++] = num7;
						array3[num6++] = num10;
						array3[num6++] = num8;
						array3[num6++] = num7;
						array3[num6++] = num9;
						array3[num6++] = num10;
					}
				}
			}
			this.mesh.vertices = this.vertices;
			this.mesh.normals = array;
			this.mesh.uv = array2;
			this.mesh.triangles = array3;
			this._meshFilter.sharedMesh = this.mesh;
			this.UpdateMesh();
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x0008BC1C File Offset: 0x00089E1C
		private void UpdateDataGrid()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this._cineticEnergy = 0f;
			float num = this.propagationSpeed * this.propagationSpeed / (this._sampleSizeX * this._sampleSizeZ);
			float num2 = Time.fixedDeltaTime / (float)this.substeps;
			float num3 = (1f - this.waveSmoothness) * Mathf.Min(this.SampleSizeX, this.SampleSizeZ);
			float num4 = this.propagationSpeed * num2;
			for (int i = 1; i <= this._resolutionX; i++)
			{
				for (int j = 1; j <= this._resolutionZ; j++)
				{
					float num5 = (this.heights[i - 1, j] + this.heights[i + 1, j] + this.heights[i, j - 1] + this.heights[i, j + 1]) / 4f;
					this.accelerations[i - 1, j - 1] = num5 - this.heights[i, j];
				}
			}
			for (int k = 0; k < this._resolutionXGhost; k++)
			{
				for (int l = 0; l < this._resolutionZGhost; l++)
				{
					if (k > 0 && l > 0 && k <= this._resolutionX && l <= this._resolutionZ)
					{
						int i = k - 1;
						int j = l - 1;
						float num6 = this.accelerations[i, j];
						float num7 = 0f;
						if (num6 > num3)
						{
							num7 += num6 - num3;
						}
						if (num6 < -num3)
						{
							num7 += num6 + num3;
						}
						num6 -= num7;
						this.velocities[i, j] += num2 * (num * num6 - this.velocities[i, j] * this.damping);
						this._cineticEnergy += this.velocities[i, j] * this.velocities[i, j] * 0.5f;
						this.SetHeight_FullCheck(i, j, num2 * this.velocities[i, j] + num7, true);
					}
					else
					{
						int num8 = k;
						int num9 = l;
						float num10 = this._sampleSizeX;
						if (k == 0)
						{
							num8++;
						}
						else if (k == this._resolutionX + 1)
						{
							num8--;
						}
						if (l == 0)
						{
							num9++;
							num10 = this._sampleSizeZ;
						}
						else if (l == this._resolutionZ + 1)
						{
							num9--;
							num10 = this._sampleSizeZ;
						}
						this.heights[k, l] = (num4 * this.heights[num8, num9] + this.heights[k, l] * num10) / (num10 + num4);
					}
				}
			}
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x0008BEE8 File Offset: 0x0008A0E8
		private void UpdateVelocities()
		{
			this._linearVelocity = (base.transform.position - this._lastPosition) / Time.fixedDeltaTime;
			this._lastPosition = base.transform.position;
			this._angularVelocity = WaveMakerUtils.GetAngularVelocity(this._lastRotation, base.transform.rotation);
			this._lastRotation = base.transform.rotation;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x0008BF59 File Offset: 0x0008A159
		private void UpdateMesh()
		{
			if (!Application.isPlaying || this.mesh == null)
			{
				return;
			}
			this.mesh.vertices = this.vertices;
			this.mesh.RecalculateNormals();
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0008BF90 File Offset: 0x0008A190
		public unsafe bool Initialize()
		{
			if (this.Descriptor == null)
			{
				Debug.LogError("WaveMaker - (" + base.gameObject.name + ") no descriptor attached: Cannot initialize");
				this._initialized = false;
				return false;
			}
			this.FixedGridRef = *this.Descriptor.FixedGridRef;
			if (this.showLogMessages)
			{
				Debug.Log("WaveMaker - (" + base.gameObject.name + ") initializing.");
			}
			this._meshFilter = base.GetComponent<MeshFilter>();
			this._meshRenderer = base.GetComponent<MeshRenderer>();
			this._collider = base.GetComponent<BoxCollider>();
			if (base.GetComponents<Collider>().Length > 1)
			{
				Debug.LogWarning("WaveMaker - (" + base.gameObject.name + ") There must be only one BoxCollider. Disabling the rest");
				Collider[] components = base.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
				this._collider.enabled = true;
			}
			this._resolutionX = this.Descriptor.ResolutionX;
			this._resolutionZ = this.Descriptor.ResolutionZ;
			this._resolutionXGhost = this._resolutionX + 2;
			this._resolutionZGhost = this._resolutionZ + 2;
			this._lastPosition = base.transform.position;
			this._lastRotation = base.transform.rotation;
			this.GenerateDataGrids();
			this.GenerateMesh();
			this._collider.center = new Vector3(this._width / 2f, 0f, this._depth / 2f);
			this._collider.size = new Vector3(this._width, 0.01f, this._depth);
			this._initialized = true;
			return true;
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x0008C146 File Offset: 0x0008A346
		public bool InitializeIfDescriptorChanged()
		{
			return !(this.Descriptor == null) && ((this._resolutionX == this.Descriptor.ResolutionX && this._resolutionZ == this.Descriptor.ResolutionZ) || this.Initialize());
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x0008C186 File Offset: 0x0008A386
		public float GetHeight(int sampleX, int sampleZ)
		{
			return this.heights[sampleX + 1, sampleZ + 1];
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0008C199 File Offset: 0x0008A399
		public float GetHeightIncludeGhostCells(int sampleX, int sampleZ)
		{
			return this.heights[sampleX, sampleZ];
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x0008C1A8 File Offset: 0x0008A3A8
		public float SetHeight(int sampleX, int sampleZ, float height, bool offset = false)
		{
			sampleX++;
			sampleZ++;
			if (offset)
			{
				this.heights[sampleX, sampleZ] += height;
			}
			else
			{
				this.heights[sampleX, sampleZ] = height;
			}
			this.vertices[(sampleZ - 1) * this._resolutionX + (sampleX - 1)].y = this.heights[sampleX, sampleZ];
			return this.heights[sampleX, sampleZ];
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x0008C220 File Offset: 0x0008A420
		public void SetHeight_FullCheck(int sampleX, int sampleZ, float height, bool offset = false)
		{
			if (sampleX >= this._resolutionX || sampleZ >= this._resolutionZ || sampleX < 0 || sampleZ < 0)
			{
				return;
			}
			if (this.FixedGridRef[sampleZ * this._resolutionX + sampleX])
			{
				return;
			}
			height = this.SetHeight(sampleX, sampleZ, height, offset);
			if (height > this._sleepThreshold || height < this._sleepThreshold)
			{
				this._isAwake = true;
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x0008C284 File Offset: 0x0008A484
		public Vector3 GetPositionFromSample(int sampleX, int sampleZ, bool worldSpace = false, bool includeHeight = false, bool includeGhostCells = false)
		{
			if (!includeGhostCells && (sampleX >= this._resolutionX || sampleZ >= this._resolutionZ || sampleX < 0 || sampleZ < 0))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Sample index is out of range : ",
					sampleX,
					" - ",
					sampleZ
				}));
			}
			if (includeGhostCells && (sampleX >= this.ResolutionXGhost || sampleZ >= this.ResolutionZGhost || sampleX < 0 || sampleZ < 0))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Sample index is out of range (ghost cells) : ",
					sampleX,
					" - ",
					sampleZ
				}));
			}
			Vector3 vector = new Vector3((float)sampleX * this._sampleSizeX, 0f, (float)sampleZ * this._sampleSizeZ);
			if (includeGhostCells)
			{
				vector.x -= this._sampleSizeX;
				vector.z -= this._sampleSizeZ;
				if (includeHeight)
				{
					vector.y = this.GetHeightIncludeGhostCells(sampleX, sampleZ);
				}
			}
			else if (includeHeight)
			{
				vector.y = this.GetHeight(sampleX, sampleZ);
			}
			if (worldSpace)
			{
				vector = base.transform.TransformPoint(vector);
			}
			return vector;
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0008C3B0 File Offset: 0x0008A5B0
		public void GetNearestSample(float posX, float posZ, out int sampleX, out int sampleZ)
		{
			if (posX < 0f)
			{
				posX = 0f;
			}
			else if (posX > this._width)
			{
				posX = this._width;
			}
			sampleX = Mathf.FloorToInt(posX / this._sampleSizeX);
			if (posZ < 0f)
			{
				posZ = 0f;
			}
			else if (posZ > this._depth)
			{
				posZ = this._depth;
			}
			sampleZ = Mathf.FloorToInt(posZ / this._sampleSizeZ);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0008C420 File Offset: 0x0008A620
		public void FixCollisions(int layer)
		{
			Quaternion rotation = base.transform.rotation;
			Vector3 halfExtents = new Vector3(this._sampleSizeX / 2f, this.collisionDetectionHeight * 2f, this._sampleSizeZ / 2f);
			LayerMask mask = 1 << layer;
			bool flag = layer == base.gameObject.layer;
			for (int i = 0; i < this._resolutionX; i++)
			{
				for (int j = 0; j < this._resolutionZ; j++)
				{
					if (Physics.OverlapBox(this.GetPositionFromSample(i, j, true, false, false), halfExtents, rotation, mask).Length > (flag ? 1 : 0))
					{
						this.Descriptor.SetFixed(i, j, true);
					}
				}
			}
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x0008C4DE File Offset: 0x0008A6DE
		public bool CheckStabilityCondition()
		{
			return Time.fixedDeltaTime / (float)this.substeps < Mathf.Min(this._sampleSizeX, this._sampleSizeZ) / this.propagationSpeed;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x0008C508 File Offset: 0x0008A708
		public void Uninitialize()
		{
			if (this._meshFilter != null)
			{
				this._meshFilter.sharedMesh = null;
			}
			if (this.mesh != null)
			{
				Object.DestroyImmediate(this.mesh);
			}
			this._initialized = false;
			if (this.showLogMessages)
			{
				Debug.Log("WaveMaker - (" + base.gameObject.name + ") uninitialized");
			}
		}

		// Token: 0x04001C47 RID: 7239
		public WaveMakerDescriptor Descriptor;

		// Token: 0x04001C48 RID: 7240
		[Tooltip("More sub executions of fixedUpdate means less efficiency, but allows for more Propagation Speed. Keep the value as near as 1 as possible.")]
		[Min(1f)]
		public int substeps = 1;

		// Token: 0x04001C49 RID: 7241
		[Tooltip("Higher damping makes waves have a short life")]
		[Range(0f, 10f)]
		public float damping = 3f;

		// Token: 0x04001C4A RID: 7242
		[Tooltip("How fast waves propagate on the surface. WARNING: High values can make it more unstable")]
		[Min(0f)]
		public float propagationSpeed = 6f;

		// Token: 0x04001C4B RID: 7243
		[Tooltip("Scales the vertical velocity of objects interacting that generates vertical waves")]
		[Min(0f)]
		public float verticalPushScale = 1f;

		// Token: 0x04001C4C RID: 7244
		[Tooltip("Scales the horizontal velocity of objects interacting that generates side waves")]
		[Min(0f)]
		public float horizontalPushScale = 1f;

		// Token: 0x04001C4D RID: 7245
		[Tooltip("Make waves smoother with this parameter")]
		[Range(0f, 1f)]
		public float waveSmoothness;

		// Token: 0x04001C4E RID: 7246
		[Tooltip("Clamp the speed of any interactor that affects this surface to this value to avoid too fast objects to affect the surface too much")]
		[Min(0f)]
		public float interactorSpeedClamp = 100f;

		// Token: 0x04001C4F RID: 7247
		[Tooltip("How much over the plane the collision will be tested")]
		[HideInInspector]
		[Range(0f, 10f)]
		public float collisionDetectionHeight = 0.1f;

		// Token: 0x04001C50 RID: 7248
		[Tooltip("Show more information on what is happening to this component")]
		public bool showLogMessages;

		// Token: 0x04001C51 RID: 7249
		[Tooltip("Red rays on intersection with interactors, yellow rays for a test")]
		public bool drawInteractionDebugRays;

		// Token: 0x04001C52 RID: 7250
		[Tooltip("Draw the mesh grid generated to test resolution and size")]
		public bool drawGrid;

		// Token: 0x04001C53 RID: 7251
		private bool _initialized;

		// Token: 0x04001C54 RID: 7252
		private MeshRenderer _meshRenderer;

		// Token: 0x04001C55 RID: 7253
		private MeshFilter _meshFilter;

		// Token: 0x04001C56 RID: 7254
		private BoxCollider _collider;

		// Token: 0x04001C57 RID: 7255
		private Mesh mesh;

		// Token: 0x04001C58 RID: 7256
		private Vector3[] vertices;

		// Token: 0x04001C59 RID: 7257
		private float[,] heights;

		// Token: 0x04001C5A RID: 7258
		private float[,] velocities;

		// Token: 0x04001C5B RID: 7259
		private float[,] accelerations;

		// Token: 0x04001C5C RID: 7260
		private bool[] FixedGridRef;

		// Token: 0x04001C5D RID: 7261
		[HideInInspector]
		[NonSerialized]
		private int _resolutionX = 10;

		// Token: 0x04001C5E RID: 7262
		[HideInInspector]
		[NonSerialized]
		private int _resolutionZ = 10;

		// Token: 0x04001C5F RID: 7263
		[HideInInspector]
		[NonSerialized]
		private int _resolutionXGhost = 12;

		// Token: 0x04001C60 RID: 7264
		[HideInInspector]
		[NonSerialized]
		private int _resolutionZGhost = 12;

		// Token: 0x04001C61 RID: 7265
		[SerializeField]
		[HideInInspector]
		private float _width = 10f;

		// Token: 0x04001C62 RID: 7266
		[SerializeField]
		[HideInInspector]
		private float _depth = 10f;

		// Token: 0x04001C63 RID: 7267
		[SerializeField]
		private float _sampleSizeX;

		// Token: 0x04001C64 RID: 7268
		[SerializeField]
		private float _sampleSizeZ;

		// Token: 0x04001C65 RID: 7269
		private float speedThreshold = 0.001f;

		// Token: 0x04001C66 RID: 7270
		private Vector3 _linearVelocity = Vector3.zero;

		// Token: 0x04001C67 RID: 7271
		private Vector3 _angularVelocity = Vector3.zero;

		// Token: 0x04001C68 RID: 7272
		private Vector3 _lastPosition = Vector3.zero;

		// Token: 0x04001C69 RID: 7273
		private Quaternion _lastRotation = Quaternion.identity;

		// Token: 0x04001C6A RID: 7274
		private List<WaveMakerInteractor> _interactorsDetected = new List<WaveMakerInteractor>();

		// Token: 0x04001C6B RID: 7275
		private List<Collider> _interactorsDetectedColliders = new List<Collider>();

		// Token: 0x04001C6C RID: 7276
		private bool _isAwake;

		// Token: 0x04001C6D RID: 7277
		private float _cineticEnergy;

		// Token: 0x04001C6E RID: 7278
		private float _sleepThreshold = 0.001f;
	}
}
