using System;
using TSD.uTireSettings;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034C RID: 844
	[ExecuteInEditMode]
	public class uTireWorldSpaceBehaviour : MonoBehaviour
	{
		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060015A2 RID: 5538 RVA: 0x000E135B File Offset: 0x000DF55B
		// (set) Token: 0x060015A3 RID: 5539 RVA: 0x000E1363 File Offset: 0x000DF563
		public EditorData editorData { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060015A4 RID: 5540 RVA: 0x000E136C File Offset: 0x000DF56C
		// (set) Token: 0x060015A5 RID: 5541 RVA: 0x000E1375 File Offset: 0x000DF575
		public float rayCountEditor
		{
			get
			{
				return (float)this.rayCount;
			}
			set
			{
				if (value == (float)this.rayCount)
				{
					return;
				}
				this.rayCount = Mathf.Clamp((int)value, 1, 64);
				this.vectorPositions = new Vector4[uTireWorldSpaceBehaviour.shaderArraySize];
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060015A6 RID: 5542 RVA: 0x000E13A2 File Offset: 0x000DF5A2
		// (set) Token: 0x060015A7 RID: 5543 RVA: 0x000E13AB File Offset: 0x000DF5AB
		public float rayRowCountEditor
		{
			get
			{
				return (float)this.rayRowCount;
			}
			set
			{
				if (value != (float)this.rayRowCount)
				{
					this.rayRowCount = (int)value;
					this.vectorPositions = new Vector4[uTireWorldSpaceBehaviour.shaderArraySize];
					return;
				}
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060015A8 RID: 5544 RVA: 0x000E13D0 File Offset: 0x000DF5D0
		private static Transform dummyTransform
		{
			get
			{
				if (uTireWorldSpaceBehaviour._dummyTransform == null)
				{
					GameObject gameObject = GameObject.Find("uTire3DCollisionDummyTransform");
					if (gameObject == null)
					{
						uTireWorldSpaceBehaviour._dummyTransform = new GameObject("uTire3DCollisionDummyTransform").transform;
					}
					else
					{
						uTireWorldSpaceBehaviour._dummyTransform = gameObject.transform;
					}
				}
				return uTireWorldSpaceBehaviour._dummyTransform;
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060015A9 RID: 5545 RVA: 0x000E1424 File Offset: 0x000DF624
		// (set) Token: 0x060015AA RID: 5546 RVA: 0x000E142C File Offset: 0x000DF62C
		public float uniformScale { get; private set; }

		// Token: 0x060015AB RID: 5547 RVA: 0x000E1435 File Offset: 0x000DF635
		private void Awake()
		{
			if (this.rayCount == -1 || this.rayRowCount == -1)
			{
				this.setDefaultSettings();
			}
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x000E1450 File Offset: 0x000DF650
		[ContextMenu("start")]
		private void Start()
		{
			this.debugData = new uTireWorldSpaceDebugData[uTireWorldSpaceBehaviour.shaderArraySize];
			this.renderer = base.GetComponent<Renderer>();
			this.vectorPositions = new Vector4[uTireWorldSpaceBehaviour.shaderArraySize];
			this.materialProperty = new MaterialPropertyBlock();
			this.propID_positionsArray = Shader.PropertyToID("positionsArray");
			this.propID_distanceCheckStrength = Shader.PropertyToID("_distanceCheckStrength");
			this.propID_distanceCheckMultiplier = Shader.PropertyToID("_distanceCheckMultiplier");
			Bounds bounds = new Bounds(base.transform.position, Vector3.zero);
			bounds.Encapsulate(this.renderer.bounds);
			this.originOffset = base.transform.InverseTransformPoint(bounds.center);
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x000A22BF File Offset: 0x000A04BF
		public virtual void CalculateRaycasts(out Vector4 hitData, Vector3 startPos, Vector3 dir, float scaledRayLength)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060015AE RID: 5550 RVA: 0x000E1508 File Offset: 0x000DF708
		private void Update()
		{
			this.uniformScale = base.transform.lossyScale.x;
			float num = this.rayLength * this.uniformScale;
			uTireWorldSpaceBehaviour.dummyTransform.position = base.transform.position + base.transform.TransformVector(this.originOffset);
			uTireWorldSpaceBehaviour.dummyTransform.right = base.transform.right;
			Vector3 a = new Vector3(-this.castDirectionMax.x, this.castDirectionMax.y, this.castDirectionMax.z) * this.uniformScale;
			for (float num2 = 0f; num2 < (float)this.rayRowCount; num2 += 1f)
			{
				float t = num2 / (float)(this.rayRowCount - 1);
				if (this.rayRowCount == 1)
				{
					t = 0.5f;
				}
				Vector3 a2 = uTireWorldSpaceBehaviour.dummyTransform.TransformDirection(this.offsetPos * this.uniformScale);
				Vector3 b = Vector3.Lerp(a2, -a2, t);
				Vector3 point = Vector3.Lerp(a, this.castDirectionMax, t);
				for (float num3 = 0f; num3 < (float)this.rayCount; num3 += 1f)
				{
					Quaternion rotation = Quaternion.AngleAxis(this.angle * ((num3 + 1f) / (float)this.rayCount) + this.offsetAngle, -Vector3.right);
					Vector3 direction = rotation * point;
					Vector3 normalized = uTireWorldSpaceBehaviour.dummyTransform.TransformDirection(rotation * Vector3.up).normalized;
					Vector3 dir = uTireWorldSpaceBehaviour.dummyTransform.TransformDirection(direction).normalized * num;
					Vector3 startPos = uTireWorldSpaceBehaviour.dummyTransform.position + b + normalized * this.radius * this.uniformScale;
					Vector4 vector;
					this.CalculateRaycasts(out vector, startPos, dir, num);
					int num4 = Mathf.Clamp((int)num2 * this.rayCount + (int)num3, 0, uTireWorldSpaceBehaviour.shaderArraySize);
					this.vectorPositions[num4] = Vector4.Lerp(this.vectorPositions[num4], vector, (vector.w == 0f) ? (Time.deltaTime * uTireGlobalSettings.Instance.animationSpeedOnCollision) : (Time.deltaTime * uTireGlobalSettings.Instance.animationSpeedOnCollision));
				}
			}
			this.setInstancedProperties();
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x000E176C File Offset: 0x000DF96C
		private void setInstancedProperties()
		{
			this.materialProperty.SetVectorArray(this.propID_positionsArray, this.vectorPositions);
			this.materialProperty.SetFloat(this.propID_distanceCheckStrength, this.matStrength);
			this.materialProperty.SetFloat(this.propID_distanceCheckMultiplier, this.matDistance);
			this.renderer.SetPropertyBlock(this.materialProperty);
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000E17CF File Offset: 0x000DF9CF
		protected void EditorSafetyChecks()
		{
			if (this.materialProperty == null || this.vectorPositions == null || this.renderer == null || this.debugData == null)
			{
				this.Start();
			}
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x000E17FD File Offset: 0x000DF9FD
		private void clearDebugData()
		{
			this.debugData = new uTireWorldSpaceDebugData[this.vectorPositions.Length];
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x000E1814 File Offset: 0x000DFA14
		public void pasteComponentData(uTireWorldSpaceBehaviour source)
		{
			this.speed = source.speed;
			this.speedNoCollision = source.speedNoCollision;
			this.rayCount = source.rayCount;
			this.rayRowCount = source.rayRowCount;
			this.rayLength = source.rayLength;
			this.radius = source.radius;
			this.minRadius = source.minRadius;
			this.angle = source.angle;
			this.offsetAngle = source.offsetAngle;
			this.matStrength = source.matStrength;
			this.matDistance = source.matDistance;
			this.originOffset = source.originOffset;
			this.castDirectionMax = source.castDirectionMax;
			this.offsetPos = source.offsetPos;
			if (source.editorData == null)
			{
				return;
			}
			if (this.editorData == null)
			{
				this.editorData = new EditorData();
			}
			Debug.Log(source.editorData);
			this.editorData.selectedMeshSide = source.editorData.selectedMeshSide;
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x000E1908 File Offset: 0x000DFB08
		private void setDefaultSettings()
		{
			this.rayCount = uTireGlobalSettings.Instance.rayCount;
			this.rayRowCount = uTireGlobalSettings.Instance.rayRingCount;
			this.angle = uTireGlobalSettings.Instance.rayAngle;
			this.offsetAngle = uTireGlobalSettings.Instance.rayAngleOffset;
		}

		// Token: 0x0400263C RID: 9788
		public float speed = 35f;

		// Token: 0x0400263D RID: 9789
		public float speedNoCollision = 75f;

		// Token: 0x0400263E RID: 9790
		public int rayCount = -1;

		// Token: 0x0400263F RID: 9791
		public int rayRowCount = -1;

		// Token: 0x04002640 RID: 9792
		public float rayLength = 0.08f;

		// Token: 0x04002641 RID: 9793
		public float radius;

		// Token: 0x04002642 RID: 9794
		public float minRadius;

		// Token: 0x04002643 RID: 9795
		[Range(0f, 360f)]
		public float angle = 360f;

		// Token: 0x04002644 RID: 9796
		[Range(0f, 360f)]
		public float offsetAngle;

		// Token: 0x04002645 RID: 9797
		public float matStrength = 1f;

		// Token: 0x04002646 RID: 9798
		public float matDistance = 1f;

		// Token: 0x04002647 RID: 9799
		public Vector3 originOffset;

		// Token: 0x04002648 RID: 9800
		public Vector3 castDirectionMax = new Vector3(0f, 1f, 0f);

		// Token: 0x04002649 RID: 9801
		public Vector3 offsetPos = new Vector3(0f, 0f, 0f);

		// Token: 0x0400264B RID: 9803
		private MaterialPropertyBlock materialProperty;

		// Token: 0x0400264C RID: 9804
		private int propID_positionsArray;

		// Token: 0x0400264D RID: 9805
		private int propID_distanceCheckStrength;

		// Token: 0x0400264E RID: 9806
		private int propID_distanceCheckMultiplier;

		// Token: 0x0400264F RID: 9807
		private Vector4[] vectorPositions;

		// Token: 0x04002650 RID: 9808
		private Renderer renderer;

		// Token: 0x04002651 RID: 9809
		private static Transform _dummyTransform;

		// Token: 0x04002653 RID: 9811
		[HideInInspector]
		public static int shaderArraySize = 320;

		// Token: 0x04002654 RID: 9812
		public static bool drawDebug = true;

		// Token: 0x04002655 RID: 9813
		private uTireWorldSpaceDebugData[] debugData;
	}
}
