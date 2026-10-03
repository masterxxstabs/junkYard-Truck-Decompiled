using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Effects;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Input;
using NWH.VehiclePhysics2.Modules;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.VehiclePhysics2.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2
{
	// Token: 0x02000258 RID: 600
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class VehicleController : MonoBehaviour
	{
		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000F9B RID: 3995 RVA: 0x000B7301 File Offset: 0x000B5501
		// (set) Token: 0x06000F9C RID: 3996 RVA: 0x000B7309 File Offset: 0x000B5509
		public Vector3 Acceleration { get; private set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000F9D RID: 3997 RVA: 0x000B7314 File Offset: 0x000B5514
		public float Direction
		{
			get
			{
				float z = base.transform.InverseTransformDirection(base.GetComponent<Rigidbody>().velocity).z;
				if (z > 0f)
				{
					return 1f;
				}
				if (z < 0f)
				{
					return -1f;
				}
				return 0f;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x000B735E File Offset: 0x000B555E
		// (set) Token: 0x06000F9F RID: 3999 RVA: 0x000B7366 File Offset: 0x000B5566
		public float ForwardAcceleration { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x000B736F File Offset: 0x000B556F
		// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x000B7377 File Offset: 0x000B5577
		public float ForwardVelocity { get; private set; }

		// Token: 0x06000FA2 RID: 4002 RVA: 0x000B7380 File Offset: 0x000B5580
		public bool IsGrounded()
		{
			int count = this.Wheels.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.Wheels[i].IsGrounded)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x000B73BC File Offset: 0x000B55BC
		public bool IsFullyGrounded()
		{
			int count = this.Wheels.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.Wheels[i].IsGrounded)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x000B73F7 File Offset: 0x000B55F7
		public bool IsAwake
		{
			get
			{
				return this._isAwake;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x000B73FF File Offset: 0x000B55FF
		// (set) Token: 0x06000FA6 RID: 4006 RVA: 0x000B7407 File Offset: 0x000B5607
		public Vector3 LocalVelocity { get; private set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x000B7410 File Offset: 0x000B5610
		public float Speed
		{
			get
			{
				if (this.ForwardVelocity >= 0f)
				{
					return this.ForwardVelocity;
				}
				return -this.ForwardVelocity;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x000B742D File Offset: 0x000B562D
		public Vector3 Velocity
		{
			get
			{
				return base.transform.TransformDirection(this.LocalVelocity);
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x000B7440 File Offset: 0x000B5640
		// (set) Token: 0x06000FAA RID: 4010 RVA: 0x000B7448 File Offset: 0x000B5648
		public float VelocityMagnitude { get; private set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000FAB RID: 4011 RVA: 0x000B7451 File Offset: 0x000B5651
		// (set) Token: 0x06000FAC RID: 4012 RVA: 0x000B745E File Offset: 0x000B565E
		public List<WheelGroup> WheelGroups
		{
			get
			{
				return this.powertrain.wheelGroups;
			}
			set
			{
				this.powertrain.wheelGroups = value;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000FAD RID: 4013 RVA: 0x000B746C File Offset: 0x000B566C
		// (set) Token: 0x06000FAE RID: 4014 RVA: 0x000B7479 File Offset: 0x000B5679
		public List<WheelComponent> Wheels
		{
			get
			{
				return this.powertrain.wheels;
			}
			set
			{
				this.powertrain.wheels = value;
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000FAF RID: 4015 RVA: 0x000B7487 File Offset: 0x000B5687
		public Vector3 WorldEnginePosition
		{
			get
			{
				return base.transform.TransformPoint(this.enginePosition);
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x000B749A File Offset: 0x000B569A
		public Vector3 WorldExhaustPosition
		{
			get
			{
				return base.transform.TransformPoint(this.exhaustPosition);
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x000B74AD File Offset: 0x000B56AD
		public Vector3 WorldTransmissionPosition
		{
			get
			{
				return base.transform.TransformPoint(this.transmissionPosition);
			}
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x000B74C0 File Offset: 0x000B56C0
		private void Awake()
		{
			this.input.Awake(this);
			this.steering.Awake(this);
			this.powertrain.Awake(this);
			this.soundManager.Awake(this);
			this.effectsManager.Awake(this);
			this.damageHandler.Awake(this);
			this.brakes.Awake(this);
			this.groundDetection.Awake(this);
			this.moduleManager.Awake(this);
			this._timeSinceSpawned = 0f;
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x000B7544 File Offset: 0x000B5744
		private void Start()
		{
			this.vehicleTransform = base.transform;
			this.vehicleRigidbody = base.GetComponent<Rigidbody>();
			this.vehicleRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			this.vehicleRigidbody.maxAngularVelocity = this.maxAngularVelocity;
			this.vehicleRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			this.vehicleRigidbody.drag = this.drag;
			this.vehicleRigidbody.mass = this.mass;
			this.vehicleRigidbody.angularDrag = this.angularDrag;
			this.vehicleRigidbody.centerOfMass = this.centerOfMass;
			this.vehicleRigidbody.inertiaTensor = this.inertiaTensor;
			this.vehicleRigidbody.sleepThreshold = 0f;
			this._initialRbConstraints = this.vehicleRigidbody.constraints;
			this.SetupMultiplayerInstance();
			base.InvokeRepeating("LODCheck", Random.Range(0f, 1f), Random.Range(0.3f, 0.5f));
			this.CheckComponentStates();
			if (!this._isAwake)
			{
				this.Sleep();
			}
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x000B764C File Offset: 0x000B584C
		private void FixedUpdate()
		{
			this.fixedDeltaTime = Time.fixedDeltaTime;
			if (this.centerOfMass != this._prevCenterOfMass)
			{
				this.vehicleRigidbody.centerOfMass = this.centerOfMass;
				this._prevCenterOfMass = this.centerOfMass;
			}
			if (this.inertiaTensor != this._prevInertiaTensor)
			{
				this.vehicleRigidbody.inertiaTensor = this.inertiaTensor;
				this._prevInertiaTensor = this.inertiaTensor;
			}
			if (this._prevMass != this.mass)
			{
				this.vehicleRigidbody.mass = this.mass;
				this._prevMass = this.mass;
			}
			if (this._prevDrag != this.drag)
			{
				this.vehicleRigidbody.drag = this.drag;
				this._prevDrag = this.drag;
			}
			if (this._prevAngularDrag != this.angularDrag)
			{
				this.vehicleRigidbody.angularDrag = this.angularDrag;
				this._prevAngularDrag = this.angularDrag;
			}
			if (this._prevMaxAngularVelocity != this.maxAngularVelocity)
			{
				this.vehicleRigidbody.maxAngularVelocity = this.maxAngularVelocity;
				this._prevMaxAngularVelocity = this.maxAngularVelocity;
			}
			this._prevVelocity = this.LocalVelocity;
			this.LocalVelocity = base.transform.InverseTransformDirection(this.vehicleRigidbody.velocity);
			this.Acceleration = (this.LocalVelocity - this._prevVelocity) / this.fixedDeltaTime;
			this.ForwardVelocity = this.LocalVelocity.z;
			this.ForwardAcceleration = this.Acceleration.z;
			this.VelocityMagnitude = this.Velocity.magnitude;
			this.ApplyLowSpeedFixes();
			if (this.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Local)
			{
				this.brakes.FixedUpdate();
				this.steering.FixedUpdate();
				this.powertrain.FixedUpdate();
				this.moduleManager.FixedUpdate();
			}
			else
			{
				this.steering.FixedUpdate();
			}
			this._timeSinceSpawned += this.fixedDeltaTime;
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x000B784C File Offset: 0x000B5A4C
		public void Update()
		{
			this.deltaTime = Time.deltaTime;
			this.CheckComponentStates();
			if (this.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Local)
			{
				this.input.Update();
				this.effectsManager.Update();
				this.soundManager.Update();
				this.damageHandler.Update();
				this.moduleManager.Update();
				return;
			}
			this.effectsManager.Update();
			this.soundManager.Update();
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x000B78C0 File Offset: 0x000B5AC0
		private void OnEnable()
		{
			this.vehicleTransform = base.transform;
			this.vehicleRigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x000B78DC File Offset: 0x000B5ADC
		private void OnDrawGizmosSelected()
		{
			if (this.vehicleRigidbody == null)
			{
				this.vehicleRigidbody = base.GetComponent<Rigidbody>();
			}
			Gizmos.color = Color.green;
			this.steering.OnDrawGizmosSelected(this);
			this.powertrain.OnDrawGizmosSelected(this);
			this.soundManager.OnDrawGizmosSelected(this);
			this.effectsManager.OnDrawGizmosSelected(this);
			this.damageHandler.OnDrawGizmosSelected(this);
			this.brakes.OnDrawGizmosSelected(this);
			this.groundDetection.OnDrawGizmosSelected(this);
			this.moduleManager.OnDrawGizmosSelected(this);
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x000B7970 File Offset: 0x000B5B70
		public Vector3 CaclulateCenterOfMass()
		{
			Vector3 vector = Vector3.zero;
			if (this.vehicleRigidbody == null)
			{
				this.vehicleRigidbody = base.GetComponent<Rigidbody>();
			}
			Vector3 zero = Vector3.zero;
			Vector3 a = Vector3.zero;
			int num = 0;
			foreach (WheelComponent wheelComponent in this.Wheels)
			{
				a += base.transform.InverseTransformPoint(wheelComponent.wheelController.transform.position);
				num++;
			}
			if (num == 0)
			{
				return vector;
			}
			vector = a / (float)num;
			vector -= this.Wheels[0].wheelController.springLength * 0.5f * base.transform.up;
			return vector;
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x000B7A54 File Offset: 0x000B5C54
		public void LODCheck()
		{
			if (this.stateSettings == null)
			{
				return;
			}
			this._lodCount = this.stateSettings.LODs.Count;
			if (!this._isAwake && this._lodCount > 0)
			{
				this.activeLODIndex = this._lodCount - 1;
				this.activeLOD = this.stateSettings.LODs[this.activeLODIndex];
				return;
			}
			if (this.updateLODs)
			{
				if (this.useCameraMainForLOD)
				{
					this.LODCamera = Camera.main;
				}
				else if (this.LODCamera == null)
				{
					Debug.LogWarning("LOD camera is null. Set the LOD camera or enable 'useCameraMainForLOD' instead. Falling back to Camera.main.");
					this.LODCamera = Camera.main;
				}
				if (this._lodCount > 0 && this.LODCamera != null)
				{
					this._cameraTransform = this.LODCamera.transform;
					this.stateSettings.LODs[this._lodCount - 2].distance = float.PositiveInfinity;
					this.vehicleToCamDistance = Vector3.Distance(this.vehicleTransform.position, this._cameraTransform.position);
					for (int i = 0; i < this._lodCount - 1; i++)
					{
						if (this.stateSettings.LODs[i].distance > this.vehicleToCamDistance)
						{
							this.activeLODIndex = i;
							this.activeLOD = this.stateSettings.LODs[i];
							return;
						}
					}
					return;
				}
				this.activeLODIndex = -1;
				this.activeLOD = null;
			}
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x000B7BD5 File Offset: 0x000B5DD5
		public void Reset()
		{
			this.SetDefaults();
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x000B7BE0 File Offset: 0x000B5DE0
		public void SetColliderMaterial()
		{
			if (this.physicsMaterial == null)
			{
				return;
			}
			Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = this.physicsMaterial;
			}
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x000B7C20 File Offset: 0x000B5E20
		public void SetDefaults()
		{
			this.steering.SetDefaults(this);
			this.powertrain.SetDefaults(this);
			this.soundManager.SetDefaults(this);
			this.effectsManager.SetDefaults(this);
			this.damageHandler.SetDefaults(this);
			this.brakes.SetDefaults(this);
			this.groundDetection.SetDefaults(this);
			this.moduleManager.SetDefaults(this);
			if (this.stateSettings == null)
			{
				this.stateSettings = (Resources.Load("NWH Vehicle Physics/Defaults/DefaultStateSettings") as StateSettings);
			}
			if (this.physicsMaterial == null)
			{
				this.physicsMaterial = (Resources.Load("NWH Vehicle Physics/Defaults/VehicleMaterial") as PhysicMaterial);
			}
			this.centerOfMass = this.CaclulateCenterOfMass();
			this.inertiaTensor = this.CalculateInertiaTensor();
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x000B7CEC File Offset: 0x000B5EEC
		public void Validate()
		{
			Debug.Log(base.gameObject.name + ": Validating VehicleController setup. If no other messages show up after this one, the vehicle is good to go.");
			if (base.transform.localScale != Vector3.one)
			{
				Debug.LogWarning("VehicleController Transform scale is other than [1,1,1]. It is recommended to avoid  scaling the vehicle parent object and use Scale Factor from Unity model import settings instead.");
			}
			this.steering.Validate(this);
			this.powertrain.Validate(this);
			this.soundManager.Validate(this);
			this.effectsManager.Validate(this);
			this.damageHandler.Validate(this);
			this.brakes.Validate(this);
			this.groundDetection.Validate(this);
			this.moduleManager.Validate(this);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x000B7D94 File Offset: 0x000B5F94
		public Vector3 CalculateInertiaTensor()
		{
			Vector3 a = new Vector3((this.vehicleDimensions.y + this.vehicleDimensions.z) * 0.12f * this.mass, (this.vehicleDimensions.z + this.vehicleDimensions.x) * 0.15f * this.mass, (this.vehicleDimensions.x + this.vehicleDimensions.y) * 0.21f * this.mass);
			Vector3 zero = Vector3.zero;
			foreach (WheelComponent wheelComponent in this.Wheels)
			{
				Vector3 vector = base.transform.InverseTransformPoint(wheelComponent.wheelController.Visual.transform.position);
				zero.x += (Mathf.Abs(vector.y) + Mathf.Abs(vector.z)) * wheelComponent.wheelController.wheel.mass;
				zero.y += (Mathf.Abs(vector.x) + Mathf.Abs(vector.z)) * wheelComponent.wheelController.wheel.mass;
				zero.z += (Mathf.Abs(vector.x) + Mathf.Abs(vector.y)) * wheelComponent.wheelController.wheel.mass;
			}
			return a + zero;
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x000B7F28 File Offset: 0x000B6128
		private void OnCollisionEnter(Collision collision)
		{
			this.damageHandler.HandleCollision(collision);
			this.vehicleRigidbody.drag = this.drag;
			this.vehicleRigidbody.angularDrag = this.angularDrag;
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x000B7F58 File Offset: 0x000B6158
		private void ApplyLowSpeedFixes()
		{
			float vertical = this.input.Vertical;
			float num = (vertical < 0f) ? (-vertical) : vertical;
			float sqrMagnitude = this.vehicleRigidbody.angularVelocity.sqrMagnitude;
			float num2 = this.VelocityMagnitude * 0.35f + sqrMagnitude * 1.2f;
			float d = Mathf.Lerp(5f, 1f, num2);
			this.vehicleRigidbody.inertiaTensor = this.inertiaTensor * d;
			if (this.freezeWhenAsleep && !this._isAwake && num2 + num < 0.2f && this._timeSinceSpawned > 2f)
			{
				if (this.freezeWhenAsleep)
				{
					this.vehicleRigidbody.drag = 200f;
				}
				if (this.constrainWhenAsleep && !this._constraintsApplied)
				{
					this._initialRbConstraints = this.vehicleRigidbody.constraints;
					this.vehicleRigidbody.constraints = RigidbodyConstraints.FreezeAll;
					this._constraintsApplied = true;
					return;
				}
			}
			else
			{
				this.vehicleRigidbody.drag = Mathf.Lerp(this.vehicleRigidbody.drag, this.drag, this.fixedDeltaTime * 10f);
				this.vehicleRigidbody.constraints = this._initialRbConstraints;
				this._constraintsApplied = false;
			}
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x000B8090 File Offset: 0x000B6290
		public void Sleep()
		{
			this._isAwake = false;
			if (this.stateSettings != null)
			{
				this.activeLODIndex = this.stateSettings.LODs.Count - 1;
				this.activeLOD = this.stateSettings.LODs[this.activeLODIndex];
			}
			this.onSleep.Invoke();
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x000B80F4 File Offset: 0x000B62F4
		private void CheckComponentStates()
		{
			this.input.CheckState(this.activeLODIndex);
			this.steering.CheckState(this.activeLODIndex);
			this.powertrain.CheckState(this.activeLODIndex);
			this.soundManager.CheckState(this.activeLODIndex);
			this.effectsManager.CheckState(this.activeLODIndex);
			this.damageHandler.CheckState(this.activeLODIndex);
			this.brakes.CheckState(this.activeLODIndex);
			this.groundDetection.CheckState(this.activeLODIndex);
			this.moduleManager.CheckState(this.activeLODIndex);
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x000B819C File Offset: 0x000B639C
		public void SetupMultiplayerInstance()
		{
			if (this.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Remote)
			{
				this.vehicleRigidbody.isKinematic = true;
				this.vehicleRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
				this.input.autoSettable = false;
				using (List<WheelComponent>.Enumerator enumerator = this.Wheels.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WheelComponent wheelComponent = enumerator.Current;
						wheelComponent.wheelController.visualOnlyUpdate = true;
						wheelComponent.wheelController.useExternalUpdate = false;
					}
					return;
				}
			}
			this.vehicleRigidbody.isKinematic = false;
			this.vehicleRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			this.input.autoSettable = true;
			foreach (WheelComponent wheelComponent2 in this.Wheels)
			{
				wheelComponent2.wheelController.visualOnlyUpdate = false;
			}
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x000B8294 File Offset: 0x000B6494
		public void Wake()
		{
			this._isAwake = true;
			this.LODCheck();
			this.onWake.Invoke();
		}

		// Token: 0x0400201D RID: 8221
		public const string DEFAULT_RESOURCES_PATH = "NWH Vehicle Physics/Defaults/";

		// Token: 0x0400201E RID: 8222
		public const string RESOURCES_PATH = "NWH Vehicle Physics/";

		// Token: 0x0400201F RID: 8223
		[Tooltip("Currently active LOD.")]
		public LOD activeLOD;

		// Token: 0x04002020 RID: 8224
		[Tooltip("Currently active LOD index.")]
		public int activeLODIndex;

		// Token: 0x04002021 RID: 8225
		[Tooltip("LODs will only be updated when this value is true.\r\nDoes not affect sleep LOD.")]
		public bool updateLODs = true;

		// Token: 0x04002022 RID: 8226
		[Tooltip("When enabled Camera.main will be used as lod camera.")]
		public bool useCameraMainForLOD = true;

		// Token: 0x04002023 RID: 8227
		[Tooltip("Camera from which the LOD distance will be measured.\r\nTo use Camera.main instead, set 'useCameraMainForLOD' to true instead.")]
		public Camera LODCamera;

		// Token: 0x04002024 RID: 8228
		[Tooltip("    Angular drag of the vehicle rigidbody.")]
		public float angularDrag;

		// Token: 0x04002025 RID: 8229
		public Brakes brakes = new Brakes();

		// Token: 0x04002026 RID: 8230
		[Tooltip("Center of mass of the rigidbody. Needs to be readjusted when new colliders are added.")]
		public Vector3 centerOfMass = Vector3.zero;

		// Token: 0x04002027 RID: 8231
		public DamageHandler damageHandler = new DamageHandler();

		// Token: 0x04002028 RID: 8232
		[Tooltip("    Cached Time.deltaTime value.")]
		public float deltaTime = 0.02f;

		// Token: 0x04002029 RID: 8233
		[Tooltip("    Drag of the vehicle rigidbody.")]
		public float drag;

		// Token: 0x0400202A RID: 8234
		public EffectManager effectsManager = new EffectManager();

		// Token: 0x0400202B RID: 8235
		[Tooltip("Position of the engine relative to the vehicle. Turn on gizmos to see the marker.")]
		public Vector3 enginePosition = new Vector3(0f, 0.4f, 1.5f);

		// Token: 0x0400202C RID: 8236
		[Tooltip("Position of the exhaust relative to the vehicle. Turn on gizmos to see the marker.")]
		public Vector3 exhaustPosition = new Vector3(0f, 0.1f, -2f);

		// Token: 0x0400202D RID: 8237
		[Tooltip("    Cached Time.fixedDeltaTime value.")]
		public float fixedDeltaTime = 0.02f;

		// Token: 0x0400202E RID: 8238
		public GroundDetection groundDetection = new GroundDetection();

		// Token: 0x0400202F RID: 8239
		[Tooltip("    Vector by which the inertia tensor of the rigidbody will be scaled on Start().\r\n    Due to the unform density of the rigidbodies, versus the very non-uniform density of a vehicle, inertia can feel\r\n    off.\r\n    Use this to adjust inertia tensor values.")]
		public Vector3 inertiaTensor = new Vector3(170f, 1640f, 1350f);

		// Token: 0x04002030 RID: 8240
		[Tooltip("COMPONENTS")]
		public NWH.VehiclePhysics2.Input.Input input = new NWH.VehiclePhysics2.Input.Input();

		// Token: 0x04002031 RID: 8241
		[Tooltip("Used as a threshold value for lateral slip. When absolute lateral slip of a wheel is\r\nlower than this value wheel is considered to have no lateral slip (wheel skid). Used mostly for effects and sound.")]
		public float lateralSlipThreshold = 0.2f;

		// Token: 0x04002032 RID: 8242
		[Tooltip("Used as a threshold value for longitudinal slip. When absolute longitudinal slip of a wheel is\r\nlower than this value wheel is considered to have no longitudinal slip (wheel spin). Used mostly for effects and sound.")]
		public float longitudinalSlipThreshold = 0.4f;

		// Token: 0x04002033 RID: 8243
		[Tooltip("    Mass of the vehicle in [kg].")]
		public float mass = 1400f;

		// Token: 0x04002034 RID: 8244
		[Tooltip("Maximum angular velocity of the rigidbody. Use to prevent vehicle spinning unrealistically fast on collisions.\r\nCan also be used to artificially limit tank's rotation speed.")]
		public float maxAngularVelocity = 8f;

		// Token: 0x04002035 RID: 8245
		public ModuleManager moduleManager = new ModuleManager();

		// Token: 0x04002036 RID: 8246
		[Tooltip("    Determines if vehicle is running locally is synchronized over active multiplayer framework.")]
		public VehicleController.MultiplayerInstanceType multiplayerInstanceType;

		// Token: 0x04002037 RID: 8247
		[Tooltip("    Called when vehicle is put to sleep.")]
		public UnityEvent onSleep = new UnityEvent();

		// Token: 0x04002038 RID: 8248
		[Tooltip("    Called when vehicle is woken up.")]
		public UnityEvent onWake = new UnityEvent();

		// Token: 0x04002039 RID: 8249
		[Tooltip("    Material that will be used on all vehicle colliders")]
		public PhysicMaterial physicsMaterial;

		// Token: 0x0400203A RID: 8250
		public Powertrain powertrain = new Powertrain();

		// Token: 0x0400203B RID: 8251
		public SoundManager soundManager = new SoundManager();

		// Token: 0x0400203C RID: 8252
		[Tooltip("State settings for the current vehicle.\r\nState settings determine which components are enabled or disabled, as well as which LOD they belong to.")]
		public StateSettings stateSettings;

		// Token: 0x0400203D RID: 8253
		public Steering steering = new Steering();

		// Token: 0x0400203E RID: 8254
		[Tooltip("Position of the transmission relative to the vehicle. Turn on gizmos to see the marker.")]
		public Vector3 transmissionPosition = new Vector3(0f, 0.2f, 0.2f);

		// Token: 0x0400203F RID: 8255
		[Tooltip("    Prevents creeping when velocity is ~0 and the vehicle is on the slope by increasing linear drag when asleep.\r\n    To fully prevent vehicle from moving and reacting to the outer forces, use 'constrainWhenAsleep' option.")]
		public bool freezeWhenAsleep = true;

		// Token: 0x04002040 RID: 8256
		[Tooltip("Constrains vehicle rigidbody position and rotation so that the vehicle is fully immobile when sleeping.")]
		public bool constrainWhenAsleep;

		// Token: 0x04002041 RID: 8257
		[Tooltip("    Vehicle dimensions in [m]. X - width, Y - height, Z - length.")]
		public Vector3 vehicleDimensions = new Vector3(1.5f, 1.5f, 4.6f);

		// Token: 0x04002042 RID: 8258
		[Tooltip("    Vehicle rigidbody.")]
		public Rigidbody vehicleRigidbody;

		// Token: 0x04002043 RID: 8259
		[Tooltip("Distance between camera and vehicle used for determining LOD.")]
		public float vehicleToCamDistance;

		// Token: 0x04002044 RID: 8260
		[Tooltip("    Cached value of vehicle transform.")]
		public Transform vehicleTransform;

		// Token: 0x04002045 RID: 8261
		private Transform _cameraTransform;

		// Token: 0x04002046 RID: 8262
		[SerializeField]
		private bool _isAwake = true;

		// Token: 0x04002047 RID: 8263
		private int _lodCount;

		// Token: 0x04002048 RID: 8264
		private float _prevAngularDrag;

		// Token: 0x04002049 RID: 8265
		private Vector3 _prevCenterOfMass;

		// Token: 0x0400204A RID: 8266
		private float _prevDrag;

		// Token: 0x0400204B RID: 8267
		private Vector3 _prevInertiaTensor;

		// Token: 0x0400204C RID: 8268
		private float _prevMass;

		// Token: 0x0400204D RID: 8269
		private float _prevMaxAngularVelocity;

		// Token: 0x0400204E RID: 8270
		private Vector3 _prevVelocity;

		// Token: 0x0400204F RID: 8271
		private Vector3 _targetZInertia = Vector3.zero;

		// Token: 0x04002050 RID: 8272
		private float _timeSinceSpawned;

		// Token: 0x04002051 RID: 8273
		private RigidbodyConstraints _initialRbConstraints;

		// Token: 0x04002052 RID: 8274
		private bool _constraintsApplied;

		// Token: 0x04002053 RID: 8275
		private bool _wasFrozen;

		// Token: 0x020004AA RID: 1194
		public enum MultiplayerInstanceType
		{
			// Token: 0x04002BC7 RID: 11207
			Local,
			// Token: 0x04002BC8 RID: 11208
			Remote
		}
	}
}
