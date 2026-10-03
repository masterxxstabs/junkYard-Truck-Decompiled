using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
	// Token: 0x02000281 RID: 641
	[Serializable]
	public class WheelComponent : PowertrainComponent
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x0600112F RID: 4399 RVA: 0x000C0E59 File Offset: 0x000BF059
		// (set) Token: 0x06001130 RID: 4400 RVA: 0x000C0E66 File Offset: 0x000BF066
		public float BrakeTorque
		{
			get
			{
				return this.wheelController.brakeTorque;
			}
			set
			{
				this.wheelController.brakeTorque = value;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06001131 RID: 4401 RVA: 0x000C0E74 File Offset: 0x000BF074
		public GameObject ControllerGO
		{
			get
			{
				return this.wheelController.gameObject;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x000C0E81 File Offset: 0x000BF081
		public Transform ControllerTransform
		{
			get
			{
				return this.wheelController.cachedTransform;
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x000C0E8E File Offset: 0x000BF08E
		// (set) Token: 0x06001134 RID: 4404 RVA: 0x000C0E9B File Offset: 0x000BF09B
		public float Damage
		{
			get
			{
				return this.wheelController.Damage;
			}
			set
			{
				this.wheelController.Damage = value;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06001135 RID: 4405 RVA: 0x000C0EA9 File Offset: 0x000BF0A9
		// (set) Token: 0x06001136 RID: 4406 RVA: 0x000C0EB1 File Offset: 0x000BF0B1
		public float DamageSteerDirection { get; private set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06001137 RID: 4407 RVA: 0x000C0EBA File Offset: 0x000BF0BA
		public bool HasLateralSlip
		{
			get
			{
				return this.NormalizedLateralSlip > this.vc.lateralSlipThreshold;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06001138 RID: 4408 RVA: 0x000C0ECF File Offset: 0x000BF0CF
		public bool HasLongitudinalSlip
		{
			get
			{
				return this.NormalizedLongitudinalSlip > this.vc.longitudinalSlipThreshold;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x000C0EE4 File Offset: 0x000BF0E4
		public bool IsGrounded
		{
			get
			{
				return this.wheelController.isGrounded;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600113A RID: 4410 RVA: 0x000C0EF1 File Offset: 0x000BF0F1
		public float LateralSlip
		{
			get
			{
				return this.wheelController.sideFriction.slip;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x000C0F03 File Offset: 0x000BF103
		// (set) Token: 0x0600113C RID: 4412 RVA: 0x000C0F0B File Offset: 0x000BF10B
		public float LinearVelocity
		{
			get
			{
				return this._linearVelocity;
			}
			set
			{
				this._linearVelocity = value;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x000C0F14 File Offset: 0x000BF114
		public float LongitudinalSlip
		{
			get
			{
				return this.wheelController.forwardFriction.slip;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600113E RID: 4414 RVA: 0x000C0F26 File Offset: 0x000BF126
		// (set) Token: 0x0600113F RID: 4415 RVA: 0x000C0F33 File Offset: 0x000BF133
		public float MotorTorque
		{
			get
			{
				return this.wheelController.motorTorque;
			}
			set
			{
				this.wheelController.motorTorque = value;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x000C0F44 File Offset: 0x000BF144
		public float NormalizedLateralSlip
		{
			get
			{
				float num = (this.wheelController.sideFriction.slip < 0f) ? (-this.wheelController.sideFriction.slip) : this.wheelController.sideFriction.slip;
				if (num < 0f)
				{
					return 0f;
				}
				if (num <= 1f)
				{
					return num;
				}
				return 1f;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x000C0FAC File Offset: 0x000BF1AC
		public float NormalizedLongitudinalSlip
		{
			get
			{
				float num = (this.wheelController.forwardFriction.slip < 0f) ? (-this.wheelController.forwardFriction.slip) : this.wheelController.forwardFriction.slip;
				if (num < 0f)
				{
					return 0f;
				}
				if (num <= 1f)
				{
					return num;
				}
				return 1f;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x000C1011 File Offset: 0x000BF211
		// (set) Token: 0x06001143 RID: 4419 RVA: 0x000C1019 File Offset: 0x000BF219
		public float OutputTorque
		{
			get
			{
				return this._outputTorque;
			}
			set
			{
				this._outputTorque = value;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x000C1022 File Offset: 0x000BF222
		public float Radius
		{
			get
			{
				return this.wheelController.radius;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06001145 RID: 4421 RVA: 0x000C102F File Offset: 0x000BF22F
		// (set) Token: 0x06001146 RID: 4422 RVA: 0x000C1037 File Offset: 0x000BF237
		public float Slip
		{
			get
			{
				return this._slip;
			}
			set
			{
				this._slip = value;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06001147 RID: 4423 RVA: 0x000C1040 File Offset: 0x000BF240
		public float SpringTravel
		{
			get
			{
				return this.wheelController.springCompression * this.wheelController.springLength;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06001148 RID: 4424 RVA: 0x000C1059 File Offset: 0x000BF259
		// (set) Token: 0x06001149 RID: 4425 RVA: 0x000C1066 File Offset: 0x000BF266
		public float SteerAngle
		{
			get
			{
				return this.wheelController.steerAngle;
			}
			set
			{
				this.wheelController.steerAngle = value;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x000C1074 File Offset: 0x000BF274
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x000C107C File Offset: 0x000BF27C
		public float VerticalLoad
		{
			get
			{
				return this._verticalLoad;
			}
			set
			{
				this._verticalLoad = value;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x000C1085 File Offset: 0x000BF285
		public float Width
		{
			get
			{
				return this.wheelController.width;
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x000C1092 File Offset: 0x000BF292
		public override void Initialize()
		{
			this.inertia = this.wheelController.wheel.inertia;
			this.DamageSteerDirection = Random.Range(-1f, 1f);
			this._singleRayByDefault = this.wheelController.singleRay;
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x000C10D0 File Offset: 0x000BF2D0
		public void PostUpdate()
		{
			this._slip = ((this._slip < -1f) ? -1f : ((this._slip > 1f) ? 1f : this._slip));
			this.wheelController.motorTorque = (this._awake ? this._outputTorque : 0f);
			this.wheelController.forwardFriction.force = (this._awake ? (this._outputTorque / this.wheelController.wheel.radius) : 0f);
			this.wheelController.Step();
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x000C1174 File Offset: 0x000BF374
		public void PreUpdate()
		{
			this._outputTorque = 0f;
			this.wheelController.useExternalUpdate = true;
			this.wheelController.useExternalLongSlipCalculation = !this._inputIsNull;
			this.wheelController.useExternalLatSlipCalculation = false;
			if (this.surfacePreset != null && this.surfacePreset.frictionPreset != null)
			{
				this.wheelController.activeFrictionPreset = this.surfacePreset.frictionPreset;
				this._BCDEz = this.surfacePreset.frictionPreset.BCDE.z;
			}
			else
			{
				this._BCDEz = 1f;
			}
			this.angularVelocity = this.wheelController.angularVelocity;
			this.LinearVelocity = this.wheelController.forwardFriction.speed;
			this.VerticalLoad = this.wheelController.wheel.load;
			this._brakeTorque = this.BrakeTorque;
			this._radius = this.Radius;
			this._fixedDeltaTime = this.vc.fixedDeltaTime;
			this._loadCoefficient = this.wheelController.CalculateLoadCoefficient();
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x000C1290 File Offset: 0x000BF490
		public void AddBrakeTorque(float torque, bool registerAsBraking = true)
		{
			if (this.wheelGroup != null)
			{
				torque *= this.wheelGroup.brakeCoefficient;
			}
			if (torque < 0f)
			{
				this.wheelController.brakeTorque += 0f;
			}
			else
			{
				this.wheelController.brakeTorque += torque;
			}
			if (this.wheelController.brakeTorque > this.vc.brakes.maxTorque)
			{
				this.wheelController.brakeTorque = this.vc.brakes.maxTorque;
			}
			if (this.wheelController.brakeTorque < 0f)
			{
				this.wheelController.brakeTorque = 0f;
			}
			this.vc.brakes.IsBraking = registerAsBraking;
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x000C1353 File Offset: 0x000BF553
		public override void OnPreSolve()
		{
			base.OnPreSolve();
			this._outputTorque = 0f;
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x000C1366 File Offset: 0x000BF566
		public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
		{
			return this.angularVelocity;
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x000C136E File Offset: 0x000BF56E
		public override float QueryInertia()
		{
			return this.inertia;
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x000C1376 File Offset: 0x000BF576
		public void ResetBrakes(float value)
		{
			this.wheelController.brakeTorque = ((value < 0f) ? (-value) : value);
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x000C1390 File Offset: 0x000BF590
		public override float SendTorque(float torque, float inertiaSum, float dt)
		{
			float num = 0f;
			float result = Friction.CalculateLongitudinalSlip(torque, this._brakeTorque, this.wheelController.dragTorque, this._radius, this.inertia, dt, this._fixedDeltaTime, this._loadCoefficient, this._BCDEz, ref this.wheelController.forwardFriction, ref this.wheelController.wheel.angularVelocity, ref num);
			this.angularVelocity = this.wheelController.wheel.angularVelocity;
			this._outputTorque += num;
			return result;
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x000C141C File Offset: 0x000BF61C
		public void SetBrakeIntensity(float percent)
		{
			float num = (percent < 0f) ? (-percent) : percent;
			num = ((num < 0f) ? 0f : ((num > 1f) ? 1f : num));
			this.AddBrakeTorque(this.vc.brakes.maxTorque * num, true);
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x000C1470 File Offset: 0x000BF670
		public void SetWheelGroup(int wheelGroupIndex)
		{
			this.wheelGroupSelector.index = wheelGroupIndex;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x000C1480 File Offset: 0x000BF680
		public override void OnDisable()
		{
			if (this.wheelController != null && this.vc.activeLOD != null && this.vc.activeLOD.singleRayGroundDetection)
			{
				this.wheelController.singleRay = true;
				this.wheelController.useExternalUpdate = false;
				this.wheelController.useExternalLatSlipCalculation = false;
				this.wheelController.useExternalLongSlipCalculation = false;
			}
			this.wheelController.motorTorque = 0f;
			this._awake = false;
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x000C1508 File Offset: 0x000BF708
		public override void Validate(VehicleController vc)
		{
			base.Validate(vc);
			if (this.wheelController == null)
			{
				Debug.LogWarning("WheelController not set.");
				return;
			}
			if (this.wheelController.parent.GetInstanceID() != vc.gameObject.GetInstanceID())
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Wheel ",
					this.wheelController.name,
					" on vehicle ",
					vc.name,
					" belongs to ",
					this.wheelController.parent.name,
					". Make sure that you reassign the wheels when copying the script from one vehicle to another."
				}));
			}
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x000C15AC File Offset: 0x000BF7AC
		public override void OnEnable()
		{
			base.OnEnable();
			if (!this._singleRayByDefault)
			{
				this.wheelController.singleRay = false;
			}
			this._awake = true;
		}

		// Token: 0x0400219D RID: 8605
		[ShowInTelemetry]
		[Tooltip("Cached values")]
		public int surfaceMapIndex = -1;

		// Token: 0x0400219E RID: 8606
		public SurfacePreset surfacePreset;

		// Token: 0x0400219F RID: 8607
		public VehicleController vc;

		// Token: 0x040021A0 RID: 8608
		public WheelController wheelController;

		// Token: 0x040021A1 RID: 8609
		[NonSerialized]
		public WheelGroup wheelGroup;

		// Token: 0x040021A2 RID: 8610
		public WheelGroupSelector wheelGroupSelector = new WheelGroupSelector();

		// Token: 0x040021A3 RID: 8611
		private bool _awake = true;

		// Token: 0x040021A4 RID: 8612
		private float _BCDEz;

		// Token: 0x040021A5 RID: 8613
		private float _brakeTorque;

		// Token: 0x040021A6 RID: 8614
		private float _fixedDeltaTime;

		// Token: 0x040021A7 RID: 8615
		private float _linearVelocity;

		// Token: 0x040021A8 RID: 8616
		private float _loadCoefficient;

		// Token: 0x040021A9 RID: 8617
		private float _outputTorque;

		// Token: 0x040021AA RID: 8618
		private float _radius;

		// Token: 0x040021AB RID: 8619
		private bool _singleRayByDefault;

		// Token: 0x040021AC RID: 8620
		private float _slip;

		// Token: 0x040021AD RID: 8621
		private float _verticalLoad = 5000f;
	}
}
