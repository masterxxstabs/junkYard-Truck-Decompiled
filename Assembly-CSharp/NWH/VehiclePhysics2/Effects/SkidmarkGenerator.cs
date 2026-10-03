using System;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Rendering;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B8 RID: 696
	[Serializable]
	public class SkidmarkGenerator
	{
		// Token: 0x06001287 RID: 4743 RVA: 0x000C6FCC File Offset: 0x000C51CC
		public bool Initialize(WheelComponent wheelComponent, GameObject skidmarkContainer, float minDistance, int surfaceMapCount, int maxMarks, bool persistent, float persistentDistance, float groundOffset, float smoothing, float lowerIntensityThreshold, bool fadeOverDistance, Material fallbackMaterial)
		{
			this._targetWheelComponent = wheelComponent;
			this._minSqrDistance = minDistance * minDistance;
			this._maxMarks = maxMarks;
			this._surfaceMapCount = surfaceMapCount;
			this._persistent = persistent;
			this._persistentDistance = persistentDistance;
			this._fadeOverDistance = (!persistent && fadeOverDistance);
			this._groundOffset = groundOffset;
			this._smoothing = smoothing;
			this._lowerIntensityThreshold = lowerIntensityThreshold;
			this._skidmarkContainer = skidmarkContainer;
			this._fallbackMaterial = fallbackMaterial;
			this._maxTris = maxMarks * 6;
			this._outTriArray = new int[this._maxTris];
			this._markWidth = wheelComponent.Width;
			this._isInitial = true;
			this._prevSurfaceMapIndex = this._surfaceMapIndex;
			this.GenerateNewSection();
			return true;
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x000C7080 File Offset: 0x000C5280
		public void Update(int surfaceMapIndex, float newIntensity, float albedoIntensity, float normalIntensity, Vector3 velocity, float dt)
		{
			this._isGrounded = this._targetWheelComponent.IsGrounded;
			this._prevIntensity = this._intensity;
			this._surfaceMapIndex = surfaceMapIndex;
			this._albedoIntensity = albedoIntensity;
			this._normalIntensity = normalIntensity;
			if (newIntensity < this._lowerIntensityThreshold || !this._isGrounded)
			{
				this._intensity = 0f;
				this._wasGroundedFlag = false;
				return;
			}
			this._intensity = Mathf.SmoothDamp(this._intensity, newIntensity, ref this._intensityVelocity, this._smoothing);
			if (surfaceMapIndex >= 0)
			{
				Vector3 vector = this._skidObject.transform.InverseTransformPoint(this._targetWheelComponent.wheelController.wheelHit.groundPoint);
				vector += this._targetWheelComponent.wheelController.wheelHit.normal * this._groundOffset;
				vector += velocity * (dt * 0.5f);
				if ((vector - this._previousRect.position).sqrMagnitude < this._minSqrDistance)
				{
					return;
				}
				bool flag = (this._isGrounded && !this._wasGroundedFlag) || (this._intensity > 0f && this._prevIntensity <= 0f) || surfaceMapIndex != this._prevSurfaceMapIndex;
				if (this._isInitial || flag)
				{
					Transform controllerTransform = this._targetWheelComponent.ControllerTransform;
					this._currentRect.position = vector;
					this._currentRect.normal = this._targetWheelComponent.wheelController.wheelHit.normal;
					Vector3 right = controllerTransform.right;
					this._currentRect.positionLeft = vector - right * (this._markWidth * 0.5f);
					this._currentRect.positionRight = vector + right * (this._markWidth * 0.5f);
					this._direction = controllerTransform.forward;
					this._xDirection = -right;
					this._currentRect.tangent = new Vector4(this._xDirection.x, this._xDirection.y, this._xDirection.y, 1f);
					this._previousRect = this._currentRect;
					this._wasGroundedFlag = true;
					this._isInitial = false;
				}
				else
				{
					this._currentRect.position = vector;
					this._currentRect.normal = this._targetWheelComponent.wheelController.wheelHit.normal;
					this._direction = this._currentRect.position - this._previousRect.position;
					this._xDirection = Vector3.Cross(this._direction, this._targetWheelComponent.wheelController.wheelHit.normal).normalized;
					this._color.a = this._intensity;
					this._currentRect.positionLeft = vector + this._xDirection * (this._markWidth * 0.5f);
					this._currentRect.positionRight = vector - this._xDirection * (this._markWidth * 0.5f);
					this._currentRect.tangent = new Vector4(this._xDirection.x, this._xDirection.y, this._xDirection.z, 1f);
				}
				this.GenerateRectGeometry();
				this._previousRect = this._currentRect;
			}
			this._prevSurfaceMapIndex = surfaceMapIndex;
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x000C73F6 File Offset: 0x000C55F6
		public void DoubleSubArray(ref int[] data, ref int[] outArray, int index1, int index2, int length1, int length2)
		{
			Array.Copy(data, index1, outArray, 0, length1);
			Array.Copy(data, index2, outArray, length1, length2);
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x000C7414 File Offset: 0x000C5614
		public void FadeOut(int startIndex, int endIndex, float targetAlpha)
		{
			if (startIndex > endIndex || startIndex >= this._colors.Length || endIndex >= this._colors.Length)
			{
				return;
			}
			float a = this._colors[startIndex].a;
			float num = (float)(endIndex - startIndex);
			for (int i = startIndex; i < endIndex; i++)
			{
				this._colors[i].a = Mathf.Lerp(a, targetAlpha, (float)(i - startIndex) / num);
			}
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x000C7480 File Offset: 0x000C5680
		public void GenerateNewSection()
		{
			this._maxTris = this._maxMarks * 6;
			this._commonIndex = 0;
			this._head = 0;
			this._tail = 0;
			this._skidObject = new GameObject("SkidMesh_" + this._targetWheelComponent.wheelController.Parent.name + "_" + string.Format("{0}_{1}", this._targetWheelComponent.wheelController.name, this._sectionCount));
			this._skidObject.transform.parent = this._skidmarkContainer.transform;
			this._skidObject.transform.position = this._targetWheelComponent.wheelController.transform.position;
			this._skidObject.isStatic = true;
			if (this._persistent)
			{
				if (this._skidmarkDestroy != null)
				{
					this._skidmarkDestroy.skidmarkIsBeingUsed = false;
				}
				this._skidmarkDestroy = this._skidObject.AddComponent<SkidmarkDestroy>();
				this._skidmarkDestroy.targetTransform = this._targetWheelComponent.wheelController.transform;
				this._skidmarkDestroy.distanceThreshold = this._persistentDistance;
				this._skidmarkDestroy.skidmarkIsBeingUsed = true;
			}
			if (!this._skidObject.GetComponent<MeshRenderer>())
			{
				this._meshRenderer = this._skidObject.AddComponent<MeshRenderer>();
				if (this._targetWheelComponent.surfacePreset != null)
				{
					this._meshRenderer.material = this._targetWheelComponent.surfacePreset.skidmarkMaterial;
				}
				else
				{
					this._meshRenderer.material = this._fallbackMaterial;
				}
				this._meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				this._meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			}
			this._meshFilter = this._skidObject.AddComponent<MeshFilter>();
			this._vertices = new Vector3[this._maxMarks * 4 * this._surfaceMapCount];
			this._normals = new Vector3[this._maxMarks * 4 * this._surfaceMapCount];
			this._tangents = new Vector4[this._maxMarks * 4 * this._surfaceMapCount];
			this._colors = new Color[this._maxMarks * 4 * this._surfaceMapCount];
			this._uvs = new Vector2[this._maxMarks * 4 * this._surfaceMapCount];
			this._triangles = new int[this._maxMarks * 9];
			this._skidmarkMesh = new Mesh();
			this._skidmarkMesh.bounds = this._bounds;
			this._skidmarkMesh.MarkDynamic();
			this._skidmarkMesh.name = "SkidmarkMesh";
			this._skidmarkMesh.subMeshCount = this._surfaceMapCount;
			this._meshFilter.mesh = this._skidmarkMesh;
			this._isInitial = true;
			this._sectionCount++;
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x000C774C File Offset: 0x000C594C
		public void SubArray(ref int[] data, ref int[] outArray, int index, int length)
		{
			Array.Copy(data, index, outArray, 0, length);
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x000C775C File Offset: 0x000C595C
		private void GenerateRectGeometry()
		{
			int num = this._commonIndex * 4;
			this._vertices[num] = this._previousRect.positionLeft;
			this._vertices[num + 1] = this._previousRect.positionRight;
			this._vertices[num + 2] = this._currentRect.positionLeft;
			this._vertices[num + 3] = this._currentRect.positionRight;
			this._normals[num] = this._previousRect.normal;
			this._normals[num + 1] = this._previousRect.normal;
			this._normals[num + 2] = this._currentRect.normal;
			this._normals[num + 3] = this._currentRect.normal;
			this._tangents[num] = this._previousRect.tangent;
			this._tangents[num + 1] = this._previousRect.tangent;
			this._tangents[num + 2] = this._currentRect.tangent;
			this._tangents[num + 3] = this._currentRect.tangent;
			this._colors[num] = this._prevColor;
			this._colors[num + 1] = this._prevColor;
			this._color.r = this._albedoIntensity;
			this._color.g = this._normalIntensity;
			this._color.a = this._intensity;
			this._colors[num + 2] = this._color;
			this._colors[num + 3] = this._color;
			if (this._fadeOverDistance)
			{
				float num2 = 1f - 2f / (float)this._maxMarks;
				int num3 = this._colors.Length;
				for (int i = 0; i < num3; i++)
				{
					Color[] colors = this._colors;
					int num4 = i;
					colors[num4].a = colors[num4].a * num2;
				}
			}
			this._prevColor = this._color;
			this._uvs[num] = this._vector00;
			this._uvs[num + 1] = this._vector10;
			this._uvs[num + 2] = this._vector01;
			this._uvs[num + 3] = this._vector11;
			int head = this._head;
			this._triangles[head] = this._commonIndex * 4;
			this._triangles[head + 2] = this._commonIndex * 4 + 1;
			this._triangles[head + 1] = this._commonIndex * 4 + 2;
			this._triangles[head + 3] = this._commonIndex * 4 + 2;
			this._triangles[head + 5] = this._commonIndex * 4 + 1;
			this._triangles[head + 4] = this._commonIndex * 4 + 3;
			this._skidmarkMesh.vertices = this._vertices;
			this._skidmarkMesh.normals = this._normals;
			this._skidmarkMesh.tangents = this._tangents;
			this._head += 6;
			if (this._head >= this._maxMarks * 9)
			{
				this._head -= this._maxMarks * 9;
			}
			if (this._head > this._tail)
			{
				int num5 = this._head - this._tail;
				this.SubArray(ref this._triangles, ref this._outTriArray, this._head - num5, num5);
				this._skidmarkMesh.SetTriangles(this._outTriArray, 0);
			}
			else if (this._head < this._tail)
			{
				int tail = this._tail;
				int length = this._maxMarks * 9 - this._tail;
				int index = 0;
				int head2 = this._head;
				this.DoubleSubArray(ref this._triangles, ref this._outTriArray, tail, index, length, head2);
				this._skidmarkMesh.SetTriangles(this._outTriArray, 0);
			}
			this._skidmarkMesh.colors = this._colors;
			this._skidmarkMesh.uv = this._uvs;
			this._skidmarkMesh.bounds = this._bounds;
			this._meshFilter.mesh = this._skidmarkMesh;
			bool flag = this.GetTriangleCount() >= this._maxTris - 1;
			if (flag && this._persistent)
			{
				this.GenerateNewSection();
				return;
			}
			if (flag)
			{
				int tail2 = this._tail;
				this._tail += 6;
				if (this._tail >= this._maxMarks * 9)
				{
					this._tail -= this._maxMarks * 9;
				}
				if (tail2 < this._head && this._tail > this._head)
				{
					this._tail = this._head;
				}
			}
			this._commonIndex++;
			if (this._commonIndex >= this._maxMarks * this._surfaceMapCount)
			{
				this._commonIndex = 0;
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x000C7C48 File Offset: 0x000C5E48
		private int GetTriangleCount()
		{
			if (this._head == this._tail)
			{
				return 0;
			}
			if (this._head > this._tail)
			{
				return this._head - this._tail;
			}
			if (this._head < this._tail)
			{
				return this._maxMarks * 9 - this._tail + this._head;
			}
			return 0;
		}

		// Token: 0x04002300 RID: 8960
		private float _albedoIntensity;

		// Token: 0x04002301 RID: 8961
		private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

		// Token: 0x04002302 RID: 8962
		private Color _color = new Color32(0, 0, 0, 0);

		// Token: 0x04002303 RID: 8963
		private Color[] _colors;

		// Token: 0x04002304 RID: 8964
		private int _commonIndex;

		// Token: 0x04002305 RID: 8965
		private SkidmarkRect _currentRect;

		// Token: 0x04002306 RID: 8966
		private Vector3 _direction;

		// Token: 0x04002307 RID: 8967
		private Vector3 _xDirection;

		// Token: 0x04002308 RID: 8968
		private bool _fadeOverDistance = true;

		// Token: 0x04002309 RID: 8969
		private float _groundOffset = 0.014f;

		// Token: 0x0400230A RID: 8970
		private int _head;

		// Token: 0x0400230B RID: 8971
		private float _intensity;

		// Token: 0x0400230C RID: 8972
		private float _intensityVelocity;

		// Token: 0x0400230D RID: 8973
		private bool _isGrounded;

		// Token: 0x0400230E RID: 8974
		private bool _isInitial = true;

		// Token: 0x0400230F RID: 8975
		private float _lowerIntensityThreshold = 0.01f;

		// Token: 0x04002310 RID: 8976
		private float _markWidth = -1f;

		// Token: 0x04002311 RID: 8977
		private int _maxMarks = 512;

		// Token: 0x04002312 RID: 8978
		private int _maxTris;

		// Token: 0x04002313 RID: 8979
		private MeshFilter _meshFilter;

		// Token: 0x04002314 RID: 8980
		private MeshRenderer _meshRenderer;

		// Token: 0x04002315 RID: 8981
		private float _minSqrDistance;

		// Token: 0x04002316 RID: 8982
		private float _normalIntensity;

		// Token: 0x04002317 RID: 8983
		private Vector3[] _normals;

		// Token: 0x04002318 RID: 8984
		private int[] _outTriArray;

		// Token: 0x04002319 RID: 8985
		private bool _persistent;

		// Token: 0x0400231A RID: 8986
		private float _persistentDistance;

		// Token: 0x0400231B RID: 8987
		private Color _prevColor = new Color(0f, 0f, 0f, 0f);

		// Token: 0x0400231C RID: 8988
		private float _prevIntensity;

		// Token: 0x0400231D RID: 8989
		private SkidmarkRect _previousRect;

		// Token: 0x0400231E RID: 8990
		private int _prevSurfaceMapIndex;

		// Token: 0x0400231F RID: 8991
		private int _sectionCount;

		// Token: 0x04002320 RID: 8992
		private GameObject _skidmarkContainer;

		// Token: 0x04002321 RID: 8993
		private SkidmarkDestroy _skidmarkDestroy;

		// Token: 0x04002322 RID: 8994
		private Mesh _skidmarkMesh;

		// Token: 0x04002323 RID: 8995
		private GameObject _skidObject;

		// Token: 0x04002324 RID: 8996
		private float _smoothing = 0.5f;

		// Token: 0x04002325 RID: 8997
		private int _surfaceMapCount;

		// Token: 0x04002326 RID: 8998
		private int _surfaceMapIndex = -1;

		// Token: 0x04002327 RID: 8999
		private int _tail;

		// Token: 0x04002328 RID: 9000
		private Vector4[] _tangents;

		// Token: 0x04002329 RID: 9001
		private WheelComponent _targetWheelComponent;

		// Token: 0x0400232A RID: 9002
		private int[] _triangles;

		// Token: 0x0400232B RID: 9003
		private Vector2[] _uvs;

		// Token: 0x0400232C RID: 9004
		private Vector2 _vector00 = new Vector2(0f, 0f);

		// Token: 0x0400232D RID: 9005
		private Vector2 _vector01 = new Vector2(0f, 1f);

		// Token: 0x0400232E RID: 9006
		private Vector2 _vector10 = new Vector2(1f, 0f);

		// Token: 0x0400232F RID: 9007
		private Vector2 _vector11 = new Vector2(1f, 1f);

		// Token: 0x04002330 RID: 9008
		private Vector3[] _vertices;

		// Token: 0x04002331 RID: 9009
		private bool _wasGroundedFlag;

		// Token: 0x04002332 RID: 9010
		private Material _fallbackMaterial;

		// Token: 0x020004C9 RID: 1225
		public struct EndIndex
		{
			// Token: 0x06001B32 RID: 6962 RVA: 0x000F87A5 File Offset: 0x000F69A5
			public EndIndex(int surfaceMapIndex, int triIndex)
			{
				this.surfaceMapIndex = surfaceMapIndex;
				this.triIndex = triIndex;
			}

			// Token: 0x04002C2B RID: 11307
			public int surfaceMapIndex;

			// Token: 0x04002C2C RID: 11308
			public int triIndex;
		}
	}
}
