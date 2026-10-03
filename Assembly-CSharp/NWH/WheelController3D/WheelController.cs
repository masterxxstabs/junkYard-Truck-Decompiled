using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.WheelController3D
{
	// Token: 0x0200024F RID: 591
	[Serializable]
	public class WheelController : MonoBehaviour
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000EFD RID: 3837 RVA: 0x000B2903 File Offset: 0x000B0B03
		// (set) Token: 0x06000EFE RID: 3838 RVA: 0x000B290B File Offset: 0x000B0B0B
		public float Damage
		{
			get
			{
				return this._damage;
			}
			set
			{
				this.ApplyDamage(value);
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x000B2914 File Offset: 0x000B0B14
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x000B2921 File Offset: 0x000B0B21
		public float angularVelocity
		{
			get
			{
				return this.wheel.angularVelocity;
			}
			set
			{
				this.wheel.angularVelocity = value;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x000B292F File Offset: 0x000B0B2F
		// (set) Token: 0x06000F02 RID: 3842 RVA: 0x000B293C File Offset: 0x000B0B3C
		public float brakeTorque
		{
			get
			{
				return this.wheel.brakeTorque;
			}
			set
			{
				if (value >= 0f)
				{
					this.wheel.brakeTorque = value;
					return;
				}
				this.wheel.brakeTorque = 0f;
				Debug.LogWarning("Brake torque must be positive. Received <0.");
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x000B296D File Offset: 0x000B0B6D
		public float camber
		{
			get
			{
				return this.wheel.camberAngle;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x000B297A File Offset: 0x000B0B7A
		public Vector3 center
		{
			get
			{
				return this.cachedTransform.InverseTransformPoint(this.worldCenter);
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000F05 RID: 3845 RVA: 0x000B298D File Offset: 0x000B0B8D
		// (set) Token: 0x06000F06 RID: 3846 RVA: 0x000B299A File Offset: 0x000B0B9A
		public float damperBumpForce
		{
			get
			{
				return this.damper.bumpForce;
			}
			set
			{
				this.damper.bumpForce = value;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000F07 RID: 3847 RVA: 0x000B29A8 File Offset: 0x000B0BA8
		// (set) Token: 0x06000F08 RID: 3848 RVA: 0x000B29B5 File Offset: 0x000B0BB5
		public AnimationCurve DamperCurve
		{
			get
			{
				return this.damper.curve;
			}
			set
			{
				this.damper.curve = value;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000F09 RID: 3849 RVA: 0x000B29C3 File Offset: 0x000B0BC3
		public float damperForce
		{
			get
			{
				return this.damper.force;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x000B29D0 File Offset: 0x000B0BD0
		// (set) Token: 0x06000F0B RID: 3851 RVA: 0x000B29DD File Offset: 0x000B0BDD
		public float damperReboundForce
		{
			get
			{
				return this.damper.reboundForce;
			}
			set
			{
				this.damper.reboundForce = value;
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x000B29EB File Offset: 0x000B0BEB
		// (set) Token: 0x06000F0D RID: 3853 RVA: 0x000B29F3 File Offset: 0x000B0BF3
		public int ForwardScanResolution
		{
			get
			{
				return this.longitudinalScanResolution;
			}
			set
			{
				this.longitudinalScanResolution = value;
				if (this.longitudinalScanResolution < 1)
				{
					this.longitudinalScanResolution = 1;
					Debug.LogWarning("Forward scan axisResolution must be > 0.");
				}
				this.InitializeScanParams();
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x000B2A1C File Offset: 0x000B0C1C
		public bool isGrounded
		{
			get
			{
				return this.hasHit;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000F0F RID: 3855 RVA: 0x000B2A24 File Offset: 0x000B0C24
		// (set) Token: 0x06000F10 RID: 3856 RVA: 0x000B2A2C File Offset: 0x000B0C2C
		public LayerMask LayerMask
		{
			get
			{
				return this.layerMask;
			}
			set
			{
				this.layerMask = value;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000F11 RID: 3857 RVA: 0x000B2A35 File Offset: 0x000B0C35
		// (set) Token: 0x06000F12 RID: 3858 RVA: 0x000B2A42 File Offset: 0x000B0C42
		public float mass
		{
			get
			{
				return this.wheel.mass;
			}
			set
			{
				this.wheel.mass = value;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x000B2A50 File Offset: 0x000B0C50
		// (set) Token: 0x06000F14 RID: 3860 RVA: 0x000B2A5D File Offset: 0x000B0C5D
		public float motorTorque
		{
			get
			{
				return this.wheel.motorTorque;
			}
			set
			{
				this.wheel.motorTorque = value;
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x000B2A6B File Offset: 0x000B0C6B
		// (set) Token: 0x06000F16 RID: 3862 RVA: 0x000B2A78 File Offset: 0x000B0C78
		public GameObject NonRotatingVisual
		{
			get
			{
				return this.wheel.NonRotatingVisual;
			}
			set
			{
				this.wheel.NonRotatingVisual = value;
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000F17 RID: 3863 RVA: 0x000B2A86 File Offset: 0x000B0C86
		// (set) Token: 0x06000F18 RID: 3864 RVA: 0x000B2A8E File Offset: 0x000B0C8E
		public GameObject Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000F19 RID: 3865 RVA: 0x000B2A97 File Offset: 0x000B0C97
		public Vector3 pointVelocity
		{
			get
			{
				return this.parentRigidbody.GetPointVelocity(this.wheel.worldPosition);
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x000B2AAF File Offset: 0x000B0CAF
		// (set) Token: 0x06000F1B RID: 3867 RVA: 0x000B2ABC File Offset: 0x000B0CBC
		public float radius
		{
			get
			{
				return this.wheel.radius;
			}
			set
			{
				this.wheel.radius = value;
				this.InitializeScanParams();
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x000B2AD0 File Offset: 0x000B0CD0
		// (set) Token: 0x06000F1D RID: 3869 RVA: 0x000B2ADD File Offset: 0x000B0CDD
		public float rimOffset
		{
			get
			{
				return this.wheel.rimOffset;
			}
			set
			{
				this.wheel.rimOffset = value;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000F1E RID: 3870 RVA: 0x000B2AEB File Offset: 0x000B0CEB
		public float rpm
		{
			get
			{
				return this.wheel.RPM;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000F1F RID: 3871 RVA: 0x000B2AF8 File Offset: 0x000B0CF8
		// (set) Token: 0x06000F20 RID: 3872 RVA: 0x000B2B05 File Offset: 0x000B0D05
		public float suspensionDistance
		{
			get
			{
				return this.spring.maxLength;
			}
			set
			{
				this.spring.maxLength = value;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000F21 RID: 3873 RVA: 0x000B2B13 File Offset: 0x000B0D13
		// (set) Token: 0x06000F22 RID: 3874 RVA: 0x000B2B1B File Offset: 0x000B0D1B
		public int SideToSideScanResolution
		{
			get
			{
				return this.lateralScanResolution;
			}
			set
			{
				this.lateralScanResolution = value;
				if (this.lateralScanResolution < 1)
				{
					this.lateralScanResolution = 1;
					Debug.LogWarning("Side to side scan axisResolution must be > 0.");
				}
				this.InitializeScanParams();
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x000B2B44 File Offset: 0x000B0D44
		public float speed
		{
			get
			{
				return this.forwardFriction.speed;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000F24 RID: 3876 RVA: 0x000B2B51 File Offset: 0x000B0D51
		public bool springBottomedOut
		{
			get
			{
				return this.spring.bottomedOut;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x000B2B5E File Offset: 0x000B0D5E
		public float springCompression
		{
			get
			{
				return 1f - this.spring.compressionPercent;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000F26 RID: 3878 RVA: 0x000B2B71 File Offset: 0x000B0D71
		// (set) Token: 0x06000F27 RID: 3879 RVA: 0x000B2B7E File Offset: 0x000B0D7E
		public AnimationCurve springCurve
		{
			get
			{
				return this.spring.forceCurve;
			}
			set
			{
				this.spring.forceCurve = value;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x000B2AF8 File Offset: 0x000B0CF8
		// (set) Token: 0x06000F29 RID: 3881 RVA: 0x000B2B05 File Offset: 0x000B0D05
		public float springLength
		{
			get
			{
				return this.spring.maxLength;
			}
			set
			{
				this.spring.maxLength = value;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000F2A RID: 3882 RVA: 0x000B2B8C File Offset: 0x000B0D8C
		// (set) Token: 0x06000F2B RID: 3883 RVA: 0x000B2B99 File Offset: 0x000B0D99
		public float springMaximumForce
		{
			get
			{
				return this.spring.maxForce;
			}
			set
			{
				this.spring.maxForce = value;
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x000B2BA7 File Offset: 0x000B0DA7
		public bool springOverExtended
		{
			get
			{
				return this.spring.overExtended;
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000F2D RID: 3885 RVA: 0x000B2BB4 File Offset: 0x000B0DB4
		public float springTravel
		{
			get
			{
				return this.spring.length;
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x000B2BC1 File Offset: 0x000B0DC1
		public Vector3 springTravelPoint
		{
			get
			{
				return this.cachedTransform.position - this.cachedTransform.up * this.spring.length;
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000F2F RID: 3887 RVA: 0x000B2BEE File Offset: 0x000B0DEE
		public float springVelocity
		{
			get
			{
				return this.spring.velocity;
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x000B2BFB File Offset: 0x000B0DFB
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x000B2C08 File Offset: 0x000B0E08
		public float steerAngle
		{
			get
			{
				return this.wheel.steerAngle;
			}
			set
			{
				this.wheel.steerAngle = value;
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x000B2C16 File Offset: 0x000B0E16
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x000B2C23 File Offset: 0x000B0E23
		public float suspensionForce
		{
			get
			{
				return this.spring.force;
			}
			set
			{
				this.spring.force = value;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000F34 RID: 3892 RVA: 0x000B2C31 File Offset: 0x000B0E31
		// (set) Token: 0x06000F35 RID: 3893 RVA: 0x000B2C39 File Offset: 0x000B0E39
		public WheelController.Side VehicleSide
		{
			get
			{
				return this.vehicleSide;
			}
			set
			{
				this.vehicleSide = value;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000F36 RID: 3894 RVA: 0x000B2C42 File Offset: 0x000B0E42
		// (set) Token: 0x06000F37 RID: 3895 RVA: 0x000B2C4F File Offset: 0x000B0E4F
		public GameObject Visual
		{
			get
			{
				return this.wheel.Visual;
			}
			set
			{
				this.wheel.Visual = value;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x000B2C5D File Offset: 0x000B0E5D
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x000B2C6A File Offset: 0x000B0E6A
		public float width
		{
			get
			{
				return this.wheel.width;
			}
			set
			{
				this.wheel.width = value;
				this.InitializeScanParams();
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000F3A RID: 3898 RVA: 0x000B2C7E File Offset: 0x000B0E7E
		public Vector3 worldCenter
		{
			get
			{
				return this._transformPosition - this._transformUp * this.spring.length;
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x000B2CA4 File Offset: 0x000B0EA4
		public void Initialize()
		{
			this._fixedDeltaTime = Time.fixedDeltaTime;
			this.cachedTransform = base.transform;
			this.SetDefaults(false, true);
			if (this.wheel.Visual != null)
			{
				this.cachedVisualTransform = this.wheel.Visual.transform;
				this.wheel.worldPosition = this.cachedVisualTransform.position;
				this.wheel.up = this.cachedVisualTransform.up;
				this.wheel.forward = this.cachedVisualTransform.forward;
				this.wheel.right = this.cachedVisualTransform.right;
			}
			if (this.wheel.NonRotatingVisual != null)
			{
				this.wheel.nonRotatingPositionOffset = this.wheel.Visual.transform.InverseTransformDirection(this.wheel.NonRotatingVisual.transform.position - this.cachedVisualTransform.position);
			}
			this.wheel.Initialize(this);
			this.InitializeScanParams();
			this.parentRigidbody = this.parent.GetComponent<Rigidbody>();
			this.spring.length = this.spring.maxLength * 0.5f * this._yScale;
			this._prevRadius = this.wheel.radius;
			this._prevWidth = this.wheel.width;
			this._initialized = true;
			this.vehicleWheelCount = this.cachedTransform.parent.GetComponentsInChildren<WheelController>().Length;
			this.forwardFriction.Initialize();
			this.sideFriction.Initialize();
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x000B2E48 File Offset: 0x000B1048
		public void InitializeScanParams()
		{
			this._boundsX = -this.wheel.width / 2f;
			this._boundsY = -this.wheel.radius;
			this._boundsZ = this.wheel.width / 2f + 1E-06f;
			this._boundsW = this.wheel.radius + 1E-06f;
			this._stepX = ((this.lateralScanResolution == 1) ? 1f : (this.wheel.width / (float)(this.lateralScanResolution - 1)));
			this._stepY = ((this.longitudinalScanResolution == 1) ? 1f : (this.wheel.radius * 2f / (float)(this.longitudinalScanResolution - 1)));
			int num = this.longitudinalScanResolution * this.lateralScanResolution;
			this.wheelHits = new WheelHit[num];
			int num2 = 0;
			for (float num3 = this._boundsX; num3 <= this._boundsZ; num3 += this._stepX)
			{
				int num4 = 0;
				for (float num5 = this._boundsY; num5 <= this._boundsW; num5 += this._stepY)
				{
					int num6 = num2 * this.longitudinalScanResolution + num4;
					WheelHit wheelHit = new WheelHit();
					wheelHit.angleForward = Mathf.Asin(num5 / (this.wheel.radius + 1E-06f));
					wheelHit.curvatureOffset = Mathf.Cos(wheelHit.angleForward) * this.wheel.radius;
					float x = num3;
					if (this.lateralScanResolution == 1)
					{
						x = 0f;
					}
					wheelHit.offset = new Vector2(x, num5);
					this.wheelHits[num6] = wheelHit;
					num4++;
				}
				num2++;
			}
			if (this._raycastCommands.IsCreated)
			{
				this._raycastCommands.Dispose();
			}
			if (this._raycastHits.IsCreated)
			{
				this._raycastHits.Dispose();
			}
			this.GenerateRaycastArraysIfNeeded(num);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x000B3032 File Offset: 0x000B1232
		private void Awake()
		{
			this.Initialize();
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x000B303A File Offset: 0x000B123A
		private void FixedUpdate()
		{
			this._fixedDeltaTime = Time.fixedDeltaTime;
			if (!this.useExternalUpdate)
			{
				this.Step();
			}
			this.hasBeenEnabledThisFrame = false;
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x000B305C File Offset: 0x000B125C
		private void OnEnable()
		{
			this.hasBeenEnabledThisFrame = true;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x000B3068 File Offset: 0x000B1268
		private void UpdateCachedValues()
		{
			this._transformPosition = this.cachedTransform.position;
			this._transformRotation = this.cachedTransform.rotation;
			this._transformForward = this.cachedTransform.forward;
			this._transformRight = this.cachedTransform.right;
			this._transformUp = this.cachedTransform.up;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x000B30CC File Offset: 0x000B12CC
		private void HitUpdate()
		{
			float num = 9999999f;
			this._wheelDown = -this.wheel.up;
			float num2 = this.spring.maxLength - this.spring.length;
			this._rayLength = this.wheel.radius * 2.1f + num2;
			this._offsetPrecalc.x = this._transformPosition.x - this._transformUp.x * this.spring.length + this.wheel.up.x * this.wheel.radius - this.wheel.inside.x * this.wheel.rimOffset;
			this._offsetPrecalc.y = this._transformPosition.y - this._transformUp.y * this.spring.length + this.wheel.up.y * this.wheel.radius - this.wheel.inside.y * this.wheel.rimOffset;
			this._offsetPrecalc.z = this._transformPosition.z - this._transformUp.z * this.spring.length + this.wheel.up.z * this.wheel.radius - this.wheel.inside.z * this.wheel.rimOffset;
			int num3 = 0;
			this._minDistRayIndex = -1;
			this.hasHit = false;
			if (this.singleRay)
			{
				this.singleWheelHit.valid = false;
				if (Physics.Raycast(this._offsetPrecalc, this._wheelDown, out this.singleWheelHit.raycastHit, this._rayLength + this.wheel.radius, this.layerMask))
				{
					float num4 = this.singleWheelHit.raycastHit.distance - this.wheel.radius - this.wheel.radius;
					if (num4 > num2)
					{
						return;
					}
					this.singleWheelHit.valid = true;
					this.hasHit = true;
					this.singleWheelHit.distanceFromTire = num4;
					this.wheelHit.raycastHit = this.singleWheelHit.raycastHit;
					this.wheelHit.angleForward = this.singleWheelHit.angleForward;
					this.wheelHit.distanceFromTire = this.singleWheelHit.distanceFromTire;
					this.wheelHit.offset = this.singleWheelHit.offset;
					this.wheelHit.weight = this.singleWheelHit.weight;
					this.wheelHit.curvatureOffset = this.singleWheelHit.curvatureOffset;
					this.wheelHit.groundPoint = this.wheelHit.raycastHit.point;
					WheelHit wheelHit = this.wheelHit;
					wheelHit.raycastHit.point = wheelHit.raycastHit.point + this.wheel.up * this.wheel.radius;
					this.wheelHit.curvatureOffset = this.wheel.radius;
				}
			}
			else
			{
				int num5 = this.wheelHits.Length;
				this.GenerateRaycastArraysIfNeeded(num5);
				for (int i = 0; i < num5; i++)
				{
					this.wheelHits[i].valid = false;
					Vector3 vector = this.wheelHits[i].offset;
					this._origin.x = this.wheel.forward.x * vector.y + this.wheel.right.x * vector.x + this._offsetPrecalc.x;
					this._origin.y = this.wheel.forward.y * vector.y + this.wheel.right.y * vector.x + this._offsetPrecalc.y;
					this._origin.z = this.wheel.forward.z * vector.y + this.wheel.right.z * vector.x + this._offsetPrecalc.z;
					this._raycastCommandsArray[i].from = this._origin;
					this._raycastCommandsArray[i].direction = this._wheelDown;
					this._raycastCommandsArray[i].distance = this._rayLength + this.wheelHits[i].curvatureOffset;
					this._raycastCommandsArray[i].layerMask = this.layerMask;
					this._raycastCommandsArray[i].maxHits = 1;
				}
				this._raycastCommands.CopyFrom(this._raycastCommandsArray);
				this._raycastJobHandle = RaycastCommand.ScheduleBatch(this._raycastCommands, this._raycastHits, 8, default(JobHandle));
				this._raycastJobHandle.Complete();
				this._raycastHits.CopyTo(this._raycastHitsArray);
				for (int j = 0; j < num5; j++)
				{
					this.wheelHits[j].valid = false;
					if (this._raycastHitsArray[j].distance > 0f)
					{
						float num6 = this._raycastHitsArray[j].distance - this.wheelHits[j].curvatureOffset - this.wheel.radius;
						if (num6 <= num2)
						{
							this.wheelHits[j].valid = true;
							this.hasHit = true;
							this.wheelHits[j].raycastHit = this._raycastHitsArray[j];
							this.wheelHits[j].distanceFromTire = num6;
							num3++;
							if (num6 < num)
							{
								num = num6;
								this._minDistRayIndex = j;
							}
						}
					}
				}
				if (this.hasHit)
				{
					this.CalculateAverageWheelHit();
				}
			}
			if (this.hasHit)
			{
				this.wheelHit.forwardDir = Vector3.Normalize(Vector3.Cross(this.wheelHit.normal, -this.wheel.right));
				this.wheelHit.sidewaysDir = Quaternion.AngleAxis(90f, this.wheelHit.normal) * this.wheelHit.forwardDir;
			}
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x000B373C File Offset: 0x000B193C
		private void SuspensionUpdate()
		{
			if (this.hasHit)
			{
				this.spring.bottomedOut = (this.spring.overExtended = false);
				this.spring.bottomedOut = (this.spring.overExtended = false);
				Vector3 point = this.wheelHit.raycastHit.point;
				float num = this.wheel.rimOffset * (float)this.vehicleSide;
				if (this.singleRay)
				{
					Vector3 vector = this.wheelHit.raycastHit.point - this._transformUp * (this.wheel.radius * 0.06f);
					this.spring.targetPoint.x = vector.x - this.wheel.right.x * num;
					this.spring.targetPoint.y = vector.y - this.wheel.right.y * num;
					this.spring.targetPoint.z = vector.z - this.wheel.right.z * num;
				}
				else
				{
					this.spring.targetPoint.x = point.x - this.wheel.forward.x * this.wheelHit.offset.y - this.wheel.right.x * this.wheelHit.offset.x - this.wheel.right.x * num;
					this.spring.targetPoint.y = point.y - this.wheel.forward.y * this.wheelHit.offset.y - this.wheel.right.y * this.wheelHit.offset.x - this.wheel.right.y * num;
					this.spring.targetPoint.z = point.z - this.wheel.forward.z * this.wheelHit.offset.y - this.wheel.right.z * this.wheelHit.offset.x - this.wheel.right.z * num;
				}
				this.spring.length = -this.cachedTransform.InverseTransformPoint(this.spring.targetPoint).y * this._yScale;
				if (this.spring.length < 0f)
				{
					this._bottomOutDistance = -this.spring.length;
					this.spring.length = 0f;
					this.spring.bottomedOut = true;
				}
				else if (this.spring.length > this.spring.maxLength)
				{
					this.spring.length = this.spring.maxLength;
					this.spring.overExtended = true;
				}
			}
			else
			{
				this.spring.length = Mathf.Lerp(this.spring.length, this.spring.maxLength, this._fixedDeltaTime * 10f);
				this.damper.force = 0f;
			}
			if (this.hasBeenEnabledThisFrame)
			{
				this.spring.prevLength = this.spring.length;
				this._prevBottomOutDistance = this._bottomOutDistance;
			}
			this.spring.velocity = (this.spring.length - this.spring.prevLength) / this._fixedDeltaTime;
			this.spring.compressionPercent = (this.spring.maxLength - this.spring.length) / this.spring.maxLength;
			this.spring.force = (this.hasHit ? (this.spring.maxForce * this.spring.forceCurve.Evaluate(this.spring.compressionPercent)) : 0f);
			if (this.spring.bottomedOut)
			{
				float num2 = (this._bottomOutDistance - this._prevBottomOutDistance) / this._fixedDeltaTime;
				float num3 = 1f / (float)((this.vehicleWheelCount > 0) ? this.vehicleWheelCount : 4);
				float num4 = this._bottomOutDistance * this._bottomOutDistance * 100f;
				float num5 = this.parentRigidbody.mass * -Physics.gravity.y;
				float num6 = num5 * num4 * num3 * this.spring.bottomOutForceCoefficient;
				num6 += num5 * -num2 * num3 * -this.spring.bottomOutForceCoefficient * 0.25f;
				this.parentRigidbody.AddForceAtPosition(num6 * this._transformUp, this._transformPosition);
			}
			else if (this.hasHit)
			{
				if (!this.hasHit)
				{
					this.damper.force = 0f;
				}
				if (this.spring.length <= this.spring.prevLength)
				{
					this.damper.force = this.damper.bumpForce * this.damper.curve.Evaluate((this.spring.velocity < 0f) ? (-this.spring.velocity) : this.spring.velocity);
				}
				else
				{
					this.damper.force = -this.damper.reboundForce * this.damper.curve.Evaluate((this.spring.velocity < 0f) ? (-this.spring.velocity) : this.spring.velocity);
				}
			}
			this.spring.prevLength = this.spring.length;
			this.suspensionForceMagnitude = (this.hasHit ? Mathf.Clamp(this.spring.force + this.damper.force, 0f, float.PositiveInfinity) : 0f);
			this._prevBottomOutDistance = this._bottomOutDistance;
			this.parentRigidbody.AddForceAtPosition(this.suspensionForceMagnitude * this._raycastHitNormal, this._transformPosition);
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x000B3D8C File Offset: 0x000B1F8C
		private void WheelUpdate()
		{
			this.wheel.worldPosition = this._transformPosition - this._transformUp * this.spring.length - this.wheel.inside * this.wheel.rimOffset;
			this.wheel.camberAngle = Mathf.Lerp(this.wheel.camberAtTop, this.wheel.camberAtBottom, 1f - this.spring.compressionPercent);
			this.wheel.load = Mathf.Clamp(this.spring.force + this.damper.force, 0f, float.PositiveInfinity);
			if (this.hasHit)
			{
				this.wheelHit.force = this.wheel.load;
			}
			this.wheel.rotationAngle = this.wheel.rotationAngle % 360f + this.wheel.angularVelocity * 57.29578f * this._fixedDeltaTime;
			this._axleRotation = Quaternion.AngleAxis(this.wheel.rotationAngle, this._transformRight);
			this.wheel.worldRotation = this.totalRotation * this._axleRotation * this._transformRotation;
			Vector3 position = this.wheel.worldPosition + this.wheel.Visual.transform.TransformVector(this.wheel.visualPositionOffset);
			Quaternion rotation = this.wheel.worldRotation * Quaternion.Euler(this.wheel.visualRotationOffset);
			this.cachedVisualTransform.SetPositionAndRotation(position, rotation);
			if (!this.wheel.nonRotatingVisualIsNull)
			{
				Vector3 b = this.wheel.right * this.wheel.nonRotatingPositionOffset.x + this.wheel.up * this.wheel.nonRotatingPositionOffset.y + this.wheel.forward * this.wheel.nonRotatingPositionOffset.z;
				this.wheel.NonRotatingVisual.transform.SetPositionAndRotation(this.wheel.worldPosition + b, this.totalRotation * this._transformRotation);
			}
			if (this.useRimCollider)
			{
				this.wheel.rimColliderGO.transform.SetPositionAndRotation(this.wheel.worldPosition, this.steerQuaternion * this.camberQuaternion * this._transformRotation);
			}
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x000B4038 File Offset: 0x000B2238
		public void VisualUpdate()
		{
			this.spring.targetPoint = this.wheelHit.raycastHit.point - this.wheel.right * (this.wheel.rimOffset * (float)this.vehicleSide);
			this.spring.length = -this.cachedTransform.InverseTransformPoint(this.spring.targetPoint).y;
			this.spring.length = Mathf.Clamp(this.spring.length, 0f, this.spring.maxLength);
			this.wheel.camberAngle = Mathf.Lerp(this.wheel.camberAtTop, this.wheel.camberAtBottom, this.spring.length / this.spring.maxLength);
			this._prevMpPosition = this.wheel.worldPosition;
			this.wheel.worldPosition = this._transformPosition - this._transformUp * this.spring.length - this.wheel.inside * this.wheel.rimOffset;
			this.wheel.worldRotation = this.totalRotation * this._transformRotation;
			Vector3 vector = (this.wheel.worldPosition - this._prevMpPosition) / this._fixedDeltaTime;
			this.wheel.angularVelocity = base.transform.InverseTransformVector(vector).z / this.wheel.radius;
			this.wheel.rotationAngle = this.wheel.rotationAngle % 360f + this.wheel.angularVelocity * 57.29578f * this._fixedDeltaTime;
			this.steerQuaternion = Quaternion.AngleAxis(this.wheel.steerAngle, this._transformUp);
			this._axleRotation = Quaternion.AngleAxis(this.wheel.rotationAngle, this._transformRight);
			this.wheel.worldRotation = this.steerQuaternion * this._axleRotation * this._transformRotation;
			Vector3 position = this.wheel.worldPosition + this.wheel.Visual.transform.TransformVector(this.wheel.visualPositionOffset);
			this.wheel.Visual.transform.SetPositionAndRotation(position, this.wheel.worldRotation);
			if (!this.wheel.nonRotatingVisualIsNull)
			{
				Vector3 b = this.wheel.right * this.wheel.nonRotatingPositionOffset.x + this.wheel.up * this.wheel.nonRotatingPositionOffset.y + this.wheel.forward * this.wheel.nonRotatingPositionOffset.z;
				this.wheel.NonRotatingVisual.transform.SetPositionAndRotation(this.wheel.worldPosition + b, this.totalRotation * this._transformRotation);
			}
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x000B4374 File Offset: 0x000B2574
		private void FrictionUpdate()
		{
			this._contactVelocity = this.parentRigidbody.GetPointVelocity(this.wheel.worldPosition - this.wheel.up * this.wheel.radius);
			if (this.wheelHit.raycastHit.rigidbody)
			{
				this._contactVelocity -= this.wheelHit.raycastHit.rigidbody.GetPointVelocity(this._wheelHitPoint);
			}
			if (this.hasHit)
			{
				this.forwardFriction.speed = Vector3.Dot(this._contactVelocity, this.wheelHit.forwardDir);
				this.sideFriction.speed = Vector3.Dot(this._contactVelocity, this.wheelHit.sidewaysDir);
			}
			else
			{
				this.forwardFriction.speed = (this.sideFriction.speed = 0f);
			}
			float num = this.CalculateLoadCoefficient();
			float magnitude = this._contactVelocity.magnitude;
			if (!this.useExternalLatSlipCalculation)
			{
				this.sideFriction.slip = 0f;
				this.sideFriction.force = 0f;
				Friction.CalculateLateralSlip(this._fixedDeltaTime, magnitude, this.wheel.angularVelocity, num, this.forwardFriction.speed, ref this.activeFrictionPreset, ref this.sideFriction, this.hasHit, out this.sideFriction.force);
			}
			if (!this.useExternalLongSlipCalculation)
			{
				this.forwardFriction.slip = 0f;
				this.forwardFriction.force = 0f;
				float num2 = 0f;
				Friction.CalculateLongitudinalSlip(this.wheel.motorTorque, this.wheel.brakeTorque, this.dragTorque, this.wheel.radius, this.wheel.inertia, this._fixedDeltaTime, this._fixedDeltaTime, num, this.activeFrictionPreset.BCDE.z, ref this.forwardFriction, ref this.wheel.angularVelocity, ref num2);
				this.forwardFriction.force = num2 / this.wheel.radius;
			}
			this.wheel.RPM = this.wheel.angularVelocity * 9.55f;
			if (this.hasHit)
			{
				this.wheelHit.forwardSlip = this.forwardFriction.slip;
				this.wheelHit.sidewaysSlip = this.sideFriction.slip;
			}
			Vector2 vector = new Vector2(this.forwardFriction.force, this.sideFriction.force);
			vector = Vector2.ClampMagnitude(vector, num);
			this.forwardFriction.force = vector.x;
			this.sideFriction.force = vector.y;
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x000B4630 File Offset: 0x000B2830
		private void UpdateForces()
		{
			float num = 0f;
			if (this.hasHit)
			{
				this._wheelHitPoint = this.wheelHit.point;
				this._raycastHitNormal = this.wheelHit.raycastHit.normal;
				this._hitDir.x = this.wheel.worldPosition.x - this._wheelHitPoint.x;
				this._hitDir.y = this.wheel.worldPosition.y - this._wheelHitPoint.y;
				this._hitDir.z = this.wheel.worldPosition.z - this._wheelHitPoint.z;
				float num2 = Mathf.Sqrt(this._hitDir.x * this._hitDir.x + this._hitDir.y * this._hitDir.y + this._hitDir.z * this._hitDir.z);
				this._alternateForwardNormal.x = this._hitDir.x / num2;
				this._alternateForwardNormal.y = this._hitDir.y / num2;
				this._alternateForwardNormal.z = this._hitDir.z / num2;
				this._alternateForwardNormal = this._alternateForwardNormal.normalized;
				if (Vector3.Dot(this._raycastHitNormal, this._transformUp) > 0.1f)
				{
					float num3 = 0f;
					if (((this.forwardFriction.speed < 0f) ? (-this.forwardFriction.speed) : this.forwardFriction.speed) < 8f)
					{
						float num4 = Vector3.Dot(this.wheelHit.normal, this._alternateForwardNormal);
						num4 = ((num4 < 0f) ? (-num4) : num4);
						num3 = (1f - num4) * this.suspensionForceMagnitude * ((this.wheelHit.angleForward < 0f) ? 1f : -1f);
					}
					this._surfaceForceVector.x = num3 * this.wheel.forward.x + this.wheelHit.sidewaysDir.x * -this.sideFriction.force + this.wheelHit.forwardDir.x * this.forwardFriction.force;
					this._surfaceForceVector.y = num3 * this.wheel.forward.y + this.wheelHit.sidewaysDir.y * -this.sideFriction.force + this.wheelHit.forwardDir.y * this.forwardFriction.force;
					this._surfaceForceVector.z = num3 * this.wheel.forward.x + this.wheelHit.sidewaysDir.z * -this.sideFriction.force + this.wheelHit.forwardDir.z * this.forwardFriction.force;
					this.parentRigidbody.AddForceAtPosition(this._surfaceForceVector, this._wheelHitPoint);
					if (this.applyForceToOthers && this.wheelHit.raycastHit.rigidbody)
					{
						Rigidbody rigidbody = this.wheelHit.raycastHit.rigidbody;
						rigidbody.AddForceAtPosition(-this._surfaceForceVector, this._wheelHitPoint);
						rigidbody.AddForceAtPosition(-this.suspensionForceMagnitude * this._raycastHitNormal, this._wheelHitPoint);
					}
				}
				if (this.squat != 0f && this.forwardFriction.force > 0f)
				{
					num = this.forwardFriction.force * this.wheel.radius * this.squat;
				}
			}
			if (this.hasBeenEnabledThisFrame)
			{
				this.wheel.prevAngularVelocity = this.wheel.angularVelocity;
			}
			float num5 = (this.wheel.angularVelocity - this.wheel.prevAngularVelocity) * this.wheel.inertia / this._fixedDeltaTime;
			num += num5;
			Vector3 vector = this._transformForward * (num * 0.5f);
			this.parentRigidbody.AddForceAtPosition(-vector, this.wheel.worldPosition + this.wheel.up);
			this.parentRigidbody.AddForceAtPosition(vector, this.wheel.worldPosition - this.wheel.up);
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x000B4AB4 File Offset: 0x000B2CB4
		private void OnDisable()
		{
			this.OnDestroy();
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x000B4ABC File Offset: 0x000B2CBC
		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying)
			{
				this._transformPosition = base.transform.position;
			}
			Gizmos.color = Color.green;
			Vector3 b = base.transform.forward * 0.07f;
			Vector3 b2 = base.transform.up * this.spring.maxLength;
			Gizmos.DrawLine(this._transformPosition - b, this._transformPosition + b);
			Gizmos.DrawLine(this._transformPosition - b2 - b, this._transformPosition - b2 + b);
			Gizmos.DrawLine(this._transformPosition, this._transformPosition - b2);
			Vector3 zero = Vector3.zero;
			if (!Application.isPlaying && !this.wheel.visualIsNull)
			{
				this.wheel.worldPosition = this.wheel.Visual.transform.position;
				this.wheel.up = this.wheel.Visual.transform.up;
				this.wheel.forward = this.wheel.Visual.transform.forward;
				this.wheel.right = this.wheel.Visual.transform.right;
			}
			Gizmos.DrawSphere(this.wheel.worldPosition, 0.02f);
			Gizmos.color = Color.green;
			this.DrawWheelGizmo(this.wheel.radius, this.wheel.width, this.wheel.worldPosition, this.wheel.up, this.wheel.forward, this.wheel.right);
			if (this.debug && Application.isPlaying)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(new Ray(this.wheel.worldPosition, this.wheel.up));
				Gizmos.color = Color.green;
				Gizmos.DrawRay(new Ray(this.wheel.worldPosition, this.wheel.forward));
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(new Ray(this.wheel.worldPosition, this.wheel.right));
				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(new Ray(this.wheel.worldPosition, this.wheel.inside));
				if (this.spring.length < 0.01f)
				{
					Gizmos.color = Color.red;
				}
				else if (this.spring.length > this.spring.maxLength - 0.01f)
				{
					Gizmos.color = Color.yellow;
				}
				else
				{
					Gizmos.color = Color.green;
				}
				if (this.hasHit)
				{
					float num = 0f;
					float num2 = float.PositiveInfinity;
					float num3 = 0f;
					foreach (WheelHit wheelHit in this.wheelHits)
					{
						num += wheelHit.weight;
						if (wheelHit.weight < num2)
						{
							num2 = wheelHit.weight;
						}
						if (wheelHit.weight > num3)
						{
							num3 = wheelHit.weight;
						}
					}
					foreach (WheelHit wheelHit2 in this.wheelHits)
					{
						float t = (wheelHit2.weight - num2) / (num3 - num2);
						Gizmos.color = Color.Lerp(Color.black, Color.white, t);
						Gizmos.DrawSphere(wheelHit2.point, 0.04f);
						Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
						Gizmos.DrawLine(wheelHit2.point, wheelHit2.point + this.wheel.up * wheelHit2.distanceFromTire);
					}
					Gizmos.color = Color.green;
					Gizmos.DrawLine(this.wheelHit.point, this.wheelHit.point + this.wheelHit.forwardDir * (this.forwardFriction.force * 0.001f));
					Gizmos.color = Color.green;
					Gizmos.DrawLine(this.wheelHit.point, this.wheelHit.point - this.wheelHit.sidewaysDir * (this.sideFriction.force * 0.001f));
					Gizmos.color = Color.red;
					Gizmos.DrawWireSphere(this.wheelHit.point, 0.04f);
					Gizmos.DrawLine(this.wheelHit.point, this.wheelHit.point + this.wheelHit.normal * 1f);
					Gizmos.color = Color.yellow;
					Vector3 normalized = (this.wheel.worldPosition - this.wheelHit.point).normalized;
					Gizmos.DrawLine(this.wheelHit.point, this.wheelHit.point + normalized * 1f);
					Gizmos.color = Color.magenta;
					Gizmos.DrawCube(this.spring.targetPoint, new Vector3(0.1f, 0.1f, 0.04f));
				}
			}
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x000B5020 File Offset: 0x000B3220
		public bool GetGroundHit(out WheelHit hit)
		{
			hit = this.wheelHit;
			return this.hasHit;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x000B5030 File Offset: 0x000B3230
		public void GetWorldPose(out Vector3 pos, out Quaternion quat)
		{
			pos = this.wheel.worldPosition;
			quat = this.wheel.worldRotation;
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x000B5054 File Offset: 0x000B3254
		public void SetCamber(float camberAtTop, float camberAtBottom)
		{
			this.wheel.camberAtTop = camberAtTop;
			this.wheel.camberAtBottom = camberAtBottom;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x000B5070 File Offset: 0x000B3270
		public void SetCamber(float camber)
		{
			Wheel wheel = this.wheel;
			this.wheel.camberAtBottom = camber;
			wheel.camberAtTop = camber;
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x000B5097 File Offset: 0x000B3297
		private void Reset()
		{
			this.SetDefaults(false, true);
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x000B50A4 File Offset: 0x000B32A4
		public void SetDefaults(bool reset = false, bool findWheelVisuals = true)
		{
			if (this.parent == null || reset)
			{
				this.parent = this.FindParent();
				if (this.parent == null)
				{
					Debug.LogWarning("Parent Rigidbody of WheelController " + base.name + " could not be found. It will have to be assigned manually.");
				}
			}
			if (this.wheel == null || reset)
			{
				this.wheel = new Wheel();
			}
			if (this.spring == null || reset)
			{
				this.spring = new Spring();
			}
			if (this.damper == null || reset)
			{
				this.damper = new Damper();
			}
			if (this.forwardFriction == null || reset)
			{
				this.forwardFriction = new Friction();
			}
			if (this.sideFriction == null || reset)
			{
				this.sideFriction = new Friction();
			}
			if (this.activeFrictionPreset == null || reset)
			{
				this.activeFrictionPreset = Resources.Load<FrictionPreset>("Wheel Controller 3D/Defaults/DefaultTireFrictionPreset");
			}
			if (this.springCurve == null || this.springCurve.keys.Length == 0 || reset)
			{
				this.springCurve = this.GenerateDefaultSpringCurve();
			}
			if (this.DamperCurve == null || this.DamperCurve.keys.Length == 0 || reset)
			{
				this.DamperCurve = this.GenerateDefaultDamperCurve();
			}
			if (this.loadGripCurve == null || this.loadGripCurve.keys.Length == 0 || reset)
			{
				this.loadGripCurve = this.GenerateDefaultLoadGripCurve();
			}
			if ((this.vehicleSide == WheelController.Side.Auto && this.parent != null) || reset)
			{
				this.vehicleSide = this.DetermineSide(base.transform.position, this.parent.transform);
			}
			if (findWheelVisuals && this.wheel.Visual == null && this.parent != null)
			{
				Transform transform = base.transform;
				foreach (Transform transform2 in this.parent.GetComponentsInChildren<Transform>())
				{
					Vector3 position = transform.position;
					Vector3 position2 = transform2.position;
					float num = position2.x - position.x;
					float num2 = position2.z - position.z;
					if (Mathf.Sqrt(num * num + num2 * num2) < 0.2f)
					{
						string text = transform2.name.ToLower();
						if ((text.Contains("wheel") || text.Contains("whl")) && transform2.GetComponent<WheelController>() == null)
						{
							this.wheel.Visual = transform2.gameObject;
						}
					}
					if (this.wheel.Visual)
					{
						Debug.LogWarning("WheelController " + base.name + ": Could not auto-find wheel visual. Make sure to assign wheel model to the 'Visual' field of WheelController.");
					}
				}
			}
			if (this.autoSetupLayerMask && this.parent != null)
			{
				this.SetupLayerMask();
			}
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x000B5378 File Offset: 0x000B3578
		public void Step()
		{
			if (!this._initialized)
			{
				this.Initialize();
			}
			bool autoSyncTransforms = Physics.autoSyncTransforms;
			Physics.autoSyncTransforms = false;
			this._scale = this.cachedTransform.lossyScale;
			this._yScale = this._scale.y;
			if (this.wheel.radius != this._prevRadius || this.wheel.width != this._prevWidth)
			{
				this.wheel.Initialize(this);
				this.InitializeScanParams();
			}
			this._prevRadius = this.wheel.radius;
			this._prevWidth = this.wheel.width;
			this.UpdateCachedValues();
			this.HitUpdate();
			if (this.visualOnlyUpdate)
			{
				this.CalculateWheelDirectionsAndRotations();
				this.VisualUpdate();
			}
			else if (!this.parentRigidbody.IsSleeping())
			{
				this.SuspensionUpdate();
				this.CalculateWheelDirectionsAndRotations();
				this.WheelUpdate();
				this.FrictionUpdate();
				this.UpdateForces();
			}
			this.wheel.prevAngularVelocity = this.wheel.angularVelocity;
			Physics.autoSyncTransforms = autoSyncTransforms;
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x000B5484 File Offset: 0x000B3684
		private void OnDestroy()
		{
			try
			{
				this._raycastCommands.Dispose();
				this._raycastHits.Dispose();
			}
			catch
			{
			}
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x000B54BC File Offset: 0x000B36BC
		private void GenerateRaycastArraysIfNeeded(int size)
		{
			NativeArray<RaycastCommand> raycastCommands = this._raycastCommands;
			if (!this._raycastCommands.IsCreated)
			{
				this._raycastCommands = new NativeArray<RaycastCommand>(size, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this._raycastCommandsArray = new RaycastCommand[size];
			}
			NativeArray<RaycastHit> raycastHits = this._raycastHits;
			if (!this._raycastHits.IsCreated)
			{
				this._raycastHits = new NativeArray<RaycastHit>(size, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this._raycastHitsArray = new RaycastHit[size];
			}
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x000B5528 File Offset: 0x000B3728
		private void CalculateWheelDirectionsAndRotations()
		{
			this.steerQuaternion = Quaternion.AngleAxis(this.wheel.steerAngle, this._transformUp);
			this.camberQuaternion = Quaternion.AngleAxis((float)(-(float)this.vehicleSide) * this.wheel.camberAngle, this._transformForward);
			this.totalRotation = this.steerQuaternion * this.camberQuaternion;
			this.wheel.up = this.totalRotation * this._transformUp;
			this.wheel.forward = this.totalRotation * this._transformForward;
			this.wheel.right = this.totalRotation * this._transformRight;
			this.wheel.inside = this.wheel.right * (float)(-(float)this.vehicleSide);
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x000B5604 File Offset: 0x000B3804
		private void CalculateAverageWheelHit()
		{
			int num = 0;
			float num2 = (float)this.wheelHits.Length;
			float num3 = float.PositiveInfinity;
			float num4 = 0f;
			float num5 = 0f;
			num2 = (float)this.wheelHits.Length;
			this._hitPointSum = Vector3.zero;
			this._normalSum = Vector3.zero;
			this._weight = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			int num10 = 0;
			int num11 = 0;
			while ((float)num11 < num2)
			{
				WheelHit wheelHit = this.wheelHits[num11];
				if (wheelHit.valid)
				{
					this._weight = this.wheel.radius - wheelHit.distanceFromTire;
					this._weight = this._weight * this._weight * this._weight;
					if (this._weight < num3)
					{
						num3 = this._weight;
					}
					else if (this._weight > num4)
					{
						num4 = this._weight;
					}
					num5 += this._weight;
					num10++;
					this._normal = wheelHit.raycastHit.normal;
					this._point = wheelHit.raycastHit.point;
					this._hitPointSum.x = this._hitPointSum.x + this._point.x * this._weight;
					this._hitPointSum.y = this._hitPointSum.y + this._point.y * this._weight;
					this._hitPointSum.z = this._hitPointSum.z + this._point.z * this._weight;
					this._normalSum.x = this._normalSum.x + this._normal.x * this._weight;
					this._normalSum.y = this._normalSum.y + this._normal.y * this._weight;
					this._normalSum.z = this._normalSum.z + this._normal.z * this._weight;
					num6 += wheelHit.offset.y * this._weight;
					num7 += wheelHit.offset.x * this._weight;
					num8 += wheelHit.angleForward * this._weight;
					num9 += wheelHit.curvatureOffset * this._weight;
					num++;
				}
				num11++;
			}
			if (num10 == 0 || this._minDistRayIndex < 0)
			{
				this.hasHit = false;
				return;
			}
			this.wheelHit.raycastHit = this.wheelHits[this._minDistRayIndex].raycastHit;
			this.wheelHit.raycastHit.point = this._hitPointSum / num5;
			this.wheelHit.offset.y = num6 / num5;
			this.wheelHit.offset.x = num7 / num5;
			this.wheelHit.angleForward = num8 / num5;
			this.wheelHit.raycastHit.normal = Vector3.Normalize(this._normalSum / num5);
			this.wheelHit.curvatureOffset = num9 / num5;
			WheelHit wheelHit2 = this.wheelHit;
			wheelHit2.raycastHit.point = wheelHit2.raycastHit.point + this.wheel.up * this.wheelHit.curvatureOffset;
			this.wheelHit.groundPoint = this.wheelHit.raycastHit.point - this.wheel.up * this.wheelHit.curvatureOffset;
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x000B597C File Offset: 0x000B3B7C
		public void ApplyDamage(float damage)
		{
			this._damage = ((damage < 0f) ? 0f : ((damage > 1f) ? 1f : damage));
			this.wheel.visualRotationOffset.z = this._damage * 10f;
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x000B59CA File Offset: 0x000B3BCA
		public float CalculateLoadCoefficient()
		{
			return this.loadGripCurve.Evaluate(Mathf.Clamp01(this.wheel.load / this.maximumTireLoad)) * this.maximumTireGripForce;
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x000B59F8 File Offset: 0x000B3BF8
		private GameObject FindParent()
		{
			Transform transform = base.transform;
			while (transform != null)
			{
				if (transform.GetComponent<Rigidbody>())
				{
					return transform.gameObject;
				}
				transform = transform.parent;
			}
			return null;
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x000B5A33 File Offset: 0x000B3C33
		private AnimationCurve GenerateDefaultSpringCurve()
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(1f, 1f);
			return animationCurve;
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x000B5A33 File Offset: 0x000B3C33
		private AnimationCurve GenerateDefaultDamperCurve()
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(1f, 1f);
			return animationCurve;
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x000B5A5C File Offset: 0x000B3C5C
		private AnimationCurve GenerateDefaultLoadGripCurve()
		{
			return new AnimationCurve
			{
				keys = new Keyframe[]
				{
					new Keyframe(0f, 0f, 0f, 1f),
					new Keyframe(0.35f, 0.6f, 1f, 1f),
					new Keyframe(1f, 1f)
				}
			};
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x000B5AD4 File Offset: 0x000B3CD4
		private Vector3 Vector3Average(List<Vector3> vectors)
		{
			Vector3 a = Vector3.zero;
			foreach (Vector3 b in vectors)
			{
				a += b;
			}
			return a / (float)vectors.Count;
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x000B5B38 File Offset: 0x000B3D38
		private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x000B5B59 File Offset: 0x000B3D59
		public WheelController.Side DetermineSide(Vector3 pointPosition, Transform referenceTransform)
		{
			if (referenceTransform.InverseTransformPoint(pointPosition).x < 0f)
			{
				return WheelController.Side.Left;
			}
			return WheelController.Side.Right;
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x000B5B71 File Offset: 0x000B3D71
		public static bool IsInLayerMask(int layer, LayerMask layermask)
		{
			return layermask == (layermask | 1 << layer);
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x000B5B88 File Offset: 0x000B3D88
		private void SetupLayerMask()
		{
			if (this.parent == null)
			{
				Debug.LogError("Cannot set up layer mask for null parent.");
				return;
			}
			List<GameObject> list = new List<GameObject>();
			this.GetVehicleColliders(this.parent.transform, ref list);
			List<string> list2 = new List<string>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				string layer = LayerMask.LayerToName(list[i].layer);
				if (list2.All((string l) => l != layer))
				{
					list2.Add(layer);
				}
			}
			list2.Add(LayerMask.LayerToName(2));
			this.layerMask = ~LayerMask.GetMask(list2.ToArray());
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000B5C44 File Offset: 0x000B3E44
		private void GetVehicleColliders(Transform parent, ref List<GameObject> colliderGOs)
		{
			colliderGOs = new List<GameObject>();
			foreach (Collider collider in parent.GetComponentsInChildren<Collider>())
			{
				if (collider.gameObject.layer == 0)
				{
					collider.gameObject.layer = 2;
				}
				colliderGOs.Add(collider.gameObject);
			}
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x000B5C98 File Offset: 0x000B3E98
		private void DrawWheelGizmo(float radius, float width, Vector3 position, Vector3 up, Vector3 forward, Vector3 right)
		{
			float d = width / 2f;
			float num = 0f;
			float d2 = radius * Mathf.Cos(num);
			float d3 = radius * Mathf.Sin(num);
			Vector3 a = position + up * d3 + forward * d2;
			for (num = 0f; num <= 6.2831855f; num += 0.2617994f)
			{
				d2 = radius * Mathf.Cos(num);
				d3 = radius * Mathf.Sin(num);
				Vector3 vector = position + up * d3 + forward * d2;
				Gizmos.DrawLine(a - right * d, vector - right * d);
				Gizmos.DrawLine(a + right * d, vector + right * d);
				Gizmos.DrawLine(a - right * d, a + right * d);
				Gizmos.DrawLine(a - right * d, vector + right * d);
				a = vector;
			}
		}

		// Token: 0x04001F89 RID: 8073
		[Tooltip("    Current active friction preset.")]
		public FrictionPreset activeFrictionPreset;

		// Token: 0x04001F8A RID: 8074
		[Tooltip("    Should forces be applied to other rigidbodies when wheel is in contact with them?")]
		public bool applyForceToOthers;

		// Token: 0x04001F8B RID: 8075
		public bool autoSetupLayerMask = true;

		// Token: 0x04001F8C RID: 8076
		[Tooltip("    Cached value of this.transform.")]
		public Transform cachedTransform;

		// Token: 0x04001F8D RID: 8077
		[Tooltip("    Cached value of visual's transform.")]
		public Transform cachedVisualTransform;

		// Token: 0x04001F8E RID: 8078
		[SerializeField]
		[Tooltip("    Instance of the damper.")]
		public Damper damper;

		// Token: 0x04001F8F RID: 8079
		[Tooltip("    If set to true draws detailed debug info.")]
		public bool debug;

		// Token: 0x04001F90 RID: 8080
		[Range(0f, 200f)]
		[Tooltip("    Constant torque acting similar to brake torque.\r\n    Imitates rolling resistance.")]
		public float dragTorque = 10f;

		// Token: 0x04001F91 RID: 8081
		[FormerlySerializedAs("fFriction")]
		[Tooltip("    Forward (longitudinal) friction info.")]
		public Friction forwardFriction;

		// Token: 0x04001F92 RID: 8082
		[Tooltip("    True if wheel touching ground.")]
		public bool hasHit = true;

		// Token: 0x04001F93 RID: 8083
		public LayerMask layerMask = 4;

		// Token: 0x04001F94 RID: 8084
		[Tooltip("Curve where X axis represents tire load as a percentage [0,1] of maximumTireLoad and Y axis\r\nrepresents tire grip force as a percentage [0,1] of maximumTireGripForce.\r\nDrastically influences handling.")]
		public AnimationCurve loadGripCurve = new AnimationCurve
		{
			keys = new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 1f),
				new Keyframe(0.35f, 0.6f, 1f, 1f),
				new Keyframe(1f, 1f)
			}
		};

		// Token: 0x04001F95 RID: 8085
		[Tooltip("    Maximum total force a tire can exert on surface, no matter the load.")]
		public float maximumTireGripForce = 10200f;

		// Token: 0x04001F96 RID: 8086
		[Tooltip("    Tire load at which the grip force reaches it's maximum.")]
		public float maximumTireLoad = 9600f;

		// Token: 0x04001F97 RID: 8087
		[SerializeField]
		[Tooltip("    Root object of the vehicle.")]
		public GameObject parent;

		// Token: 0x04001F98 RID: 8088
		[Tooltip("    Rigidbody to which the forces will be applied.")]
		public Rigidbody parentRigidbody;

		// Token: 0x04001F99 RID: 8089
		[FormerlySerializedAs("sFriction")]
		[Tooltip("    Side (lateral) friction info.")]
		public Friction sideFriction;

		// Token: 0x04001F9A RID: 8090
		[Tooltip("When enabled only a single raycast is used to detect ground.\r\nVery fast and should be used when performance is critical.")]
		public bool singleRay;

		// Token: 0x04001F9B RID: 8091
		[SerializeField]
		[Tooltip("    Instance of the spring.")]
		public Spring spring;

		// Token: 0x04001F9C RID: 8092
		[Range(-1f, 1f)]
		[Tooltip("Amount of torque transferred from the wheel to the chassis of the vehicle. \r\nLower values for vehicles that have anti-squat. Vehicle with wheel fixed at center directly to the chassis would have\r\nvalue of 1f, while depending on rear suspension configuration this value can be <0 (rear end of the vehicle rises instead of squats on accelerationMag).\r\nSmall amount of squat is recommended on RWD cars as this loads the rear tires more giving the vehicle more traction.")]
		public float squat = 0.2f;

		// Token: 0x04001F9D RID: 8093
		public bool useExternalLatSlipCalculation;

		// Token: 0x04001F9E RID: 8094
		public bool useExternalLongSlipCalculation;

		// Token: 0x04001F9F RID: 8095
		[Tooltip("When true Step() will not be called each FixedUpdate().\r\nUsed when execution order is important and/or the other script is waiting on the result of Step().")]
		public bool useExternalUpdate;

		// Token: 0x04001FA0 RID: 8096
		[Tooltip("If enabled mesh collider mimicking the shape of rim and wheel will be positioned so that wheel can not pass through\r\nobjects in case raycast does not detect the surface in time.")]
		public bool useRimCollider = true;

		// Token: 0x04001FA1 RID: 8097
		[SerializeField]
		[Tooltip("    Side the wheel is on.")]
		public WheelController.Side vehicleSide = WheelController.Side.Auto;

		// Token: 0x04001FA2 RID: 8098
		public int vehicleWheelCount;

		// Token: 0x04001FA3 RID: 8099
		public bool visualOnlyUpdate;

		// Token: 0x04001FA4 RID: 8100
		[SerializeField]
		[Tooltip("    Instance of the wheel.")]
		public Wheel wheel;

		// Token: 0x04001FA5 RID: 8101
		[Tooltip("    Contains point in which wheel touches ground. Not valid if !isGrounded.")]
		public WheelHit wheelHit = new WheelHit();

		// Token: 0x04001FA6 RID: 8102
		private Vector3 _alternateForwardNormal;

		// Token: 0x04001FA7 RID: 8103
		private Quaternion _axleRotation;

		// Token: 0x04001FA8 RID: 8104
		private float _bottomOutDistance;

		// Token: 0x04001FA9 RID: 8105
		private float _boundsX;

		// Token: 0x04001FAA RID: 8106
		private float _boundsY;

		// Token: 0x04001FAB RID: 8107
		private float _boundsZ;

		// Token: 0x04001FAC RID: 8108
		private float _boundsW;

		// Token: 0x04001FAD RID: 8109
		private Vector3 _contactVelocity;

		// Token: 0x04001FAE RID: 8110
		private float _damage;

		// Token: 0x04001FAF RID: 8111
		private float _fixedDeltaTime;

		// Token: 0x04001FB0 RID: 8112
		private Vector3 _hitDir;

		// Token: 0x04001FB1 RID: 8113
		private Vector3 _hitPointSum = Vector3.zero;

		// Token: 0x04001FB2 RID: 8114
		private bool _initialized;

		// Token: 0x04001FB3 RID: 8115
		private int _minDistRayIndex;

		// Token: 0x04001FB4 RID: 8116
		private Vector3 _normal;

		// Token: 0x04001FB5 RID: 8117
		private Vector3 _normalSum = Vector3.zero;

		// Token: 0x04001FB6 RID: 8118
		private Vector3 _offsetPrecalc;

		// Token: 0x04001FB7 RID: 8119
		private Vector3 _origin;

		// Token: 0x04001FB8 RID: 8120
		private Vector3 _point;

		// Token: 0x04001FB9 RID: 8121
		private float _prevBottomOutDistance;

		// Token: 0x04001FBA RID: 8122
		private Vector3 _prevMpPosition;

		// Token: 0x04001FBB RID: 8123
		private float _prevRadius;

		// Token: 0x04001FBC RID: 8124
		private float _prevWidth;

		// Token: 0x04001FBD RID: 8125
		private NativeArray<RaycastCommand> _raycastCommands;

		// Token: 0x04001FBE RID: 8126
		private RaycastCommand[] _raycastCommandsArray;

		// Token: 0x04001FBF RID: 8127
		private Vector3 _raycastHitNormal;

		// Token: 0x04001FC0 RID: 8128
		private NativeArray<RaycastHit> _raycastHits;

		// Token: 0x04001FC1 RID: 8129
		private RaycastHit[] _raycastHitsArray;

		// Token: 0x04001FC2 RID: 8130
		private JobHandle _raycastJobHandle;

		// Token: 0x04001FC3 RID: 8131
		private float _rayLength;

		// Token: 0x04001FC4 RID: 8132
		private Vector3 _scale;

		// Token: 0x04001FC5 RID: 8133
		private float _stepX;

		// Token: 0x04001FC6 RID: 8134
		private float _stepY;

		// Token: 0x04001FC7 RID: 8135
		private Vector3 _surfaceForceVector;

		// Token: 0x04001FC8 RID: 8136
		private Vector3 _transformForward;

		// Token: 0x04001FC9 RID: 8137
		private Vector3 _transformPosition;

		// Token: 0x04001FCA RID: 8138
		private Vector3 _transformRight;

		// Token: 0x04001FCB RID: 8139
		private Quaternion _transformRotation;

		// Token: 0x04001FCC RID: 8140
		private Vector3 _transformUp;

		// Token: 0x04001FCD RID: 8141
		private float _weight;

		// Token: 0x04001FCE RID: 8142
		private Vector3 _wheelDown;

		// Token: 0x04001FCF RID: 8143
		private Vector3 _wheelHitPoint;

		// Token: 0x04001FD0 RID: 8144
		private float _yScale;

		// Token: 0x04001FD1 RID: 8145
		private Quaternion camberQuaternion;

		// Token: 0x04001FD2 RID: 8146
		private bool hasBeenEnabledThisFrame;

		// Token: 0x04001FD3 RID: 8147
		[SerializeField]
		private int lateralScanResolution = 3;

		// Token: 0x04001FD4 RID: 8148
		[SerializeField]
		private int longitudinalScanResolution = 8;

		// Token: 0x04001FD5 RID: 8149
		private WheelHit singleWheelHit = new WheelHit();

		// Token: 0x04001FD6 RID: 8150
		private Quaternion steerQuaternion;

		// Token: 0x04001FD7 RID: 8151
		[SerializeField]
		private float suspensionForceMagnitude;

		// Token: 0x04001FD8 RID: 8152
		private Quaternion totalRotation;

		// Token: 0x04001FD9 RID: 8153
		[SerializeField]
		private WheelHit[] wheelHits;

		// Token: 0x020004A3 RID: 1187
		public enum Side
		{
			// Token: 0x04002BBA RID: 11194
			Left = -1,
			// Token: 0x04002BBB RID: 11195
			Right = 1,
			// Token: 0x04002BBC RID: 11196
			Center = 0,
			// Token: 0x04002BBD RID: 11197
			Auto = 2
		}
	}
}
