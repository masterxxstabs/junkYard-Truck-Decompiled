using System;
using System.Collections;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001B9 RID: 441
	public class MotoController : MonoBehaviour
	{
		// Token: 0x06000AAF RID: 2735 RVA: 0x0008DCE8 File Offset: 0x0008BEE8
		private void Awake()
		{
			base.transform.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x0008DD28 File Offset: 0x0008BF28
		private void Start()
		{
			this.cr = GameObject.Find("EventSystem2").GetComponent<ControlRef>();
			this.rb = base.GetComponent<Rigidbody>();
			this.rb.maxAngularVelocity = float.PositiveInfinity;
			this.fWheelRb = this.motoGeometry.fPhysicsWheel.GetComponent<Rigidbody>();
			this.fWheelRb.maxAngularVelocity = float.PositiveInfinity;
			if (this.motoGeometry.secondaryFVisualWheel && this.motoGeometry.secondaryFPhysicsWheel)
			{
				this.secondaryFWheelRb = this.motoGeometry.secondaryFPhysicsWheel.GetComponent<Rigidbody>();
				this.secondaryFWheelRb.maxAngularVelocity = float.PositiveInfinity;
			}
			this.rWheelRb = this.motoGeometry.rPhysicsWheel.GetComponent<Rigidbody>();
			this.rWheelRb.maxAngularVelocity = float.PositiveInfinity;
			this.currentTopSpeed = this.engineSettings.topSpeed;
			this.initialHandlesRotation = this.motoGeometry.handles.transform.localRotation;
			this.fPhysicsWheelConfigJoint = this.motoGeometry.fPhysicsWheel.GetComponent<ConfigurableJoint>();
			if (this.motoGeometry.secondaryFVisualWheel && this.motoGeometry.secondaryFPhysicsWheel)
			{
				this.secondaryFPhysicsWheelConfigJoint = this.motoGeometry.secondaryFPhysicsWheel.GetComponent<ConfigurableJoint>();
			}
			this.rPhysicsWheelConfigJoint = this.motoGeometry.rPhysicsWheel.GetComponent<ConfigurableJoint>();
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0008DE90 File Offset: 0x0008C090
		private void FixedUpdate()
		{
			if (this.braked)
			{
				this.rb.velocity = this.rb.velocity / 1.05f;
			}
			if (Input.GetKey(this.cr.Brake) || this.cr.BrakeInput > 0f)
			{
				this.braked = true;
			}
			else
			{
				this.braked = false;
			}
			bool flag = this.stuntInput;
			this.motoGeometry.fPhysicsWheel.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y + this.customSteerAxis * this.steerAngle.Evaluate(this.rb.velocity.magnitude), 0f);
			this.fPhysicsWheelConfigJoint.axis = new Vector3(1f, 0f, 0f);
			if (this.motoGeometry.secondaryFVisualWheel && this.motoGeometry.secondaryFPhysicsWheel)
			{
				this.motoGeometry.secondaryFPhysicsWheel.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y + this.customSteerAxis * this.steerAngle.Evaluate(this.rb.velocity.magnitude), 0f);
				this.secondaryFPhysicsWheelConfigJoint.axis = new Vector3(1f, 0f, 0f);
			}
			float magnitude = this.rb.velocity.magnitude;
			if (this.stuntMode)
			{
				this.rb.centerOfMass = base.GetComponent<BoxCollider>().center;
			}
			else
			{
				this.rb.centerOfMass = this.centerOfMassOffset + new Vector3(0f, 0f, (this.rawCustomAccelerationAxis > 0f && this.customSteerAxis == 0f) ? (0.5f - (this.engineSettings.gearRatio - 1f) * 0.5f) : 0f);
			}
			if (this.currGear != this.prevGear)
			{
				base.StartCoroutine(this.GearChange(0.15f));
			}
			this.prevGear = this.currGear;
			this.currGear = Mathf.Clamp(Mathf.FloorToInt((magnitude + (float)this.engineSettings.numOfGears) / (float)this.engineSettings.numOfGears), 0, this.engineSettings.numOfGears);
			if (this.changeGear)
			{
				this.engineSettings.currentGear = this.currGear;
			}
			this.engineSettings.gearRatio = (magnitude + (float)this.engineSettings.numOfGears) / (float)this.engineSettings.numOfGears - (float)this.currGear;
			this.engineSettings.gearRatio = Mathf.Clamp01(this.engineSettings.gearRatio);
			if (this.dirtbikeScript.running)
			{
				if (this.rawCustomAccelerationAxis > 0f)
				{
					this.rWheelRb.AddTorque(base.transform.right * this.engineSettings.torque * this.customAccelerationAxis * Mathf.Clamp01((float)this.engineSettings.currentGear));
				}
				if (this.rawCustomAccelerationAxis > 0f && !this.isAirborne)
				{
					this.powerDivision = this.dirtbikeScript.powerDivision;
					this.additiveBonus = this.dirtbikeScript.additiveBonus;
					Vector3 force = base.transform.forward * this.engineSettings.accelerationCurve.Evaluate(this.engineSettings.gearRatio) / (float)this.powerDivision + base.transform.forward * (float)this.additiveBonus;
					this.rb.AddForce(force);
				}
			}
			else if (this.rawCustomAccelerationAxis > 0f)
			{
				this.rWheelRb.AddTorque(base.transform.right * 10f);
			}
			else if (this.rawCustomAccelerationAxis > 0f && !this.isAirborne)
			{
				this.rb.AddForce(base.transform.forward * 10f);
			}
			if (magnitude < this.engineSettings.reversingSpeed && this.rawCustomAccelerationAxis < 0f && !this.isAirborne)
			{
				this.rb.AddForce(-base.transform.forward * this.engineSettings.accelerationCurve.Evaluate(this.customAccelerationAxis) * 0.5f);
			}
			if (base.transform.InverseTransformDirection(this.rb.velocity).z < 0f)
			{
				this.isReversing = true;
			}
			else
			{
				this.isReversing = false;
			}
			if (this.rawCustomAccelerationAxis < 0f && !this.isReversing && !this.isAirborne)
			{
				this.rb.AddForce(-base.transform.forward * this.engineSettings.accelerationCurve.Evaluate(this.customAccelerationAxis) * 2f);
			}
			this.pickUpSpeed = Mathf.Clamp(magnitude * 0.2f, 0f, 1f);
			this.motoGeometry.handles.transform.localRotation = Quaternion.Euler(0f, this.customSteerAxis * this.steerAngle.Evaluate(magnitude), -this.customSteerAxis * this.axisAngle) * this.initialHandlesRotation;
			this.motoGeometry.fVisualWheel.transform.position = new Vector3(this.motoGeometry.fPhysicsWheel.transform.position.x, this.motoGeometry.fPhysicsWheel.transform.position.y, this.motoGeometry.fPhysicsWheel.transform.position.z);
			this.xQuat = Mathf.Sin(0.017453292f * base.transform.rotation.eulerAngles.y);
			this.zQuat = Mathf.Cos(0.017453292f * base.transform.rotation.eulerAngles.y);
			this.motoGeometry.fVisualWheel.transform.rotation = Quaternion.Euler(this.xQuat * (this.customSteerAxis * -this.axisAngle), this.customSteerAxis * this.steerAngle.Evaluate(magnitude), this.zQuat * (this.customSteerAxis * -this.axisAngle));
			this.motoGeometry.fVisualWheel.transform.GetChild(0).transform.localRotation = this.motoGeometry.rPhysicsWheel.transform.rotation;
			if (this.motoGeometry.secondaryFVisualWheel && this.motoGeometry.secondaryFPhysicsWheel)
			{
				this.motoGeometry.secondaryFVisualWheel.transform.position = new Vector3(this.motoGeometry.secondaryFPhysicsWheel.transform.position.x, this.motoGeometry.secondaryFPhysicsWheel.transform.position.y, this.motoGeometry.secondaryFPhysicsWheel.transform.position.z);
				this.motoGeometry.secondaryFVisualWheel.transform.rotation = Quaternion.Euler(this.xQuat * (this.customSteerAxis * -this.axisAngle), this.customSteerAxis * this.steerAngle.Evaluate(magnitude), this.zQuat * (this.customSteerAxis * -this.axisAngle));
				this.motoGeometry.secondaryFVisualWheel.transform.GetChild(0).transform.localRotation = this.motoGeometry.rPhysicsWheel.transform.rotation;
			}
			if (this.interactor.currency.drunk > 6)
			{
				this.turnLeanAmount = -this.leanCurve.Evaluate(this.customLeanAxis) * Mathf.Clamp(magnitude * 0.3f, 0f, 1f);
			}
			else
			{
				this.turnLeanAmount = -this.leanCurve.Evaluate(this.customLeanAxis) * Mathf.Clamp(magnitude * 0.1f, 0f, 1f);
			}
			this.wheelFrictionSettings.fPhysicMaterial.staticFriction = this.wheelFrictionSettings.fFriction.x;
			this.wheelFrictionSettings.fPhysicMaterial.dynamicFriction = this.wheelFrictionSettings.fFriction.y;
			this.wheelFrictionSettings.rPhysicMaterial.staticFriction = this.wheelFrictionSettings.rFriction.x;
			this.wheelFrictionSettings.rPhysicMaterial.dynamicFriction = this.wheelFrictionSettings.rFriction.y;
			if (Physics.Raycast(this.motoGeometry.fPhysicsWheel.transform.position, Vector3.down, out this.hit, float.PositiveInfinity) && this.hit.distance < 0.5f)
			{
				Vector3 direction = this.motoGeometry.fPhysicsWheel.transform.InverseTransformDirection(this.fWheelRb.velocity);
				direction.x *= Mathf.Clamp01(1f / (this.wheelFrictionSettings.fFriction.x + this.wheelFrictionSettings.fFriction.y));
				this.fWheelRb.velocity = this.motoGeometry.fPhysicsWheel.transform.TransformDirection(direction);
			}
			if (Physics.Raycast(this.motoGeometry.rPhysicsWheel.transform.position, Vector3.down, out this.hit, float.PositiveInfinity) && this.hit.distance < 0.5f)
			{
				Vector3 direction2 = this.motoGeometry.rPhysicsWheel.transform.InverseTransformDirection(this.rWheelRb.velocity);
				direction2.x *= Mathf.Clamp01(1f / (this.wheelFrictionSettings.rFriction.x + this.wheelFrictionSettings.rFriction.y));
				this.rWheelRb.velocity = this.motoGeometry.rPhysicsWheel.transform.TransformDirection(direction2);
			}
			if (this.motoGeometry.secondaryFVisualWheel && this.motoGeometry.secondaryFPhysicsWheel && Physics.Raycast(this.motoGeometry.secondaryFPhysicsWheel.transform.position, Vector3.down, out this.hit, float.PositiveInfinity) && this.hit.distance < 0.5f)
			{
				Vector3 direction3 = this.motoGeometry.secondaryFPhysicsWheel.transform.InverseTransformDirection(this.secondaryFWheelRb.velocity);
				direction3.x *= Mathf.Clamp01(1f / (this.wheelFrictionSettings.fFriction.x + this.wheelFrictionSettings.fFriction.y));
				this.secondaryFWheelRb.velocity = this.motoGeometry.fPhysicsWheel.transform.TransformDirection(direction3);
			}
			if (Physics.Raycast(base.transform.position + new Vector3(0f, 1f, 0f), Vector3.down, out this.hit, float.PositiveInfinity))
			{
				if (this.hit.distance > 1.6f)
				{
					this.isAirborne = true;
				}
				else
				{
					this.isAirborne = false;
				}
				if (this.hit.distance > this.airTimeSettings.heightThreshold && this.airTimeSettings.freestyle)
				{
					this.stuntMode = true;
					this.rb.AddTorque(base.transform.right * this.rawCustomAccelerationAxis * 3f * this.airTimeSettings.airTimeRotationSensitivity, ForceMode.Impulse);
				}
				else
				{
					this.stuntMode = false;
				}
			}
			if (this.airTimeSettings.freestyle)
			{
				if (!this.stuntMode && this.isAirborne)
				{
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, this.turnLeanAmount + this.GroundConformity(this.groundConformity)), Time.deltaTime * this.airTimeSettings.groundSnapSensitivity);
				}
				else if (!this.stuntMode && !this.isAirborne)
				{
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, this.turnLeanAmount + this.GroundConformity(this.groundConformity)), Time.deltaTime * 10f * this.airTimeSettings.groundSnapSensitivity);
				}
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, this.turnLeanAmount + this.GroundConformity(this.groundConformity));
			}
			this.deceleration = (this.fWheelRb.velocity - this.lastVelocity) / Time.fixedDeltaTime;
			this.lastVelocity = this.fWheelRb.velocity;
			this.lastDeceleration = this.deceleration;
			if (!this.isAirborne && this.wheelieInput && this.rawCustomAccelerationAxis > 0f)
			{
				if (this.dirtbikeScript.running)
				{
					this.rb.angularDrag = 15f;
					this.wheeliePower = this.customAccelerationAxis * 150f;
					Quaternion quaternion = Quaternion.FromToRotation(base.transform.forward, new Vector3(base.transform.forward.x, 2.5f, base.transform.forward.z));
					this.rb.AddTorque(new Vector3(quaternion.x, quaternion.y, quaternion.z) * this.wheeliePower, ForceMode.Acceleration);
				}
				if (base.transform.eulerAngles.x < 322f && base.transform.eulerAngles.x > 300f)
				{
					base.GetComponent<MotoStatus>().dislodged = true;
				}
			}
			else
			{
				this.rb.angularDrag = 1f;
			}
			if (base.transform.InverseTransformDirection(this.rb.velocity).z > 0f && !this.isAirborne && this.wheelieInput && this.rawCustomAccelerationAxis < 0f && !this.isReversing)
			{
				this.rb.angularDrag = 15f;
				this.wheeliePower = this.customAccelerationAxis * 150f;
				Quaternion quaternion2 = Quaternion.FromToRotation(base.transform.forward, new Vector3(base.transform.forward.x, -1.6f, base.transform.forward.z));
				this.rb.AddTorque(new Vector3(quaternion2.x, quaternion2.y, quaternion2.z) * this.wheeliePower, ForceMode.Acceleration);
			}
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x0008EE98 File Offset: 0x0008D098
		private void Update()
		{
			this.ApplyCustomInput();
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0008EEA0 File Offset: 0x0008D0A0
		private float GroundConformity(bool toggle)
		{
			if (toggle)
			{
				this.groundZ = base.transform.rotation.eulerAngles.z;
			}
			return this.groundZ;
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0008EED4 File Offset: 0x0008D0D4
		private void ApplyCustomInput()
		{
			this.CustomInput("Horizontal", ref this.customSteerAxis, this.steerControls.x, this.steerControls.y, false);
			this.CustomInput("Vertical", ref this.customAccelerationAxis, 1f, 1f, false);
			this.CustomInput("Horizontal", ref this.customLeanAxis, this.steerControls.x, this.steerControls.y, false);
			this.CustomInput("Vertical", ref this.rawCustomAccelerationAxis, 1f, 1f, true);
			this.wheelieInput = Input.GetKey(this.cr.Wheelie);
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0008EF84 File Offset: 0x0008D184
		private float CustomInput(string name, ref float axis, float sensitivity, float gravity, bool isRaw)
		{
			int num;
			if (name == "Horizontal")
			{
				if (this.cr.xAxes == 0f || this.cr.Horiz != 0f)
				{
					num = Mathf.RoundToInt(this.cr.Horiz);
				}
				else
				{
					num = Mathf.RoundToInt(this.cr.xAxes * 2f);
				}
			}
			else if (this.cr.GasInput == 0f)
			{
				num = Mathf.RoundToInt(this.cr.Vert);
			}
			else
			{
				num = Mathf.RoundToInt(this.cr.GasInput);
			}
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (isRaw)
			{
				axis = (float)num;
			}
			else if (num != 0)
			{
				axis = Mathf.Clamp(axis + (float)num * sensitivity * unscaledDeltaTime, -1f, 1f);
			}
			else
			{
				axis = Mathf.Clamp01(Mathf.Abs(axis) - gravity * unscaledDeltaTime) * Mathf.Sign(axis);
			}
			if ((float)this.interactor.currency.drunk > 6f && name == "Horizontal")
			{
				float num2 = (float)this.interactor.currency.drunk * 0.001f;
				float num3 = Mathf.Sin(Time.time * 2f) * num2;
				axis += num3;
			}
			return axis;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0008F0D0 File Offset: 0x0008D2D0
		private IEnumerator GearChange(float time)
		{
			this.changeGear = false;
			this.engineSettings.currentGear = 0;
			yield return new WaitForSeconds(time);
			this.changeGear = true;
			yield break;
		}

		// Token: 0x04001CD3 RID: 7379
		public Dirtbike dirtbikeScript;

		// Token: 0x04001CD4 RID: 7380
		public EngineSettings engineSettings;

		// Token: 0x04001CD5 RID: 7381
		public MotoGeometry motoGeometry;

		// Token: 0x04001CD6 RID: 7382
		public WheelFrictionSettings wheelFrictionSettings;

		// Token: 0x04001CD7 RID: 7383
		[Tooltip("Steer Angle over Speed")]
		public AnimationCurve steerAngle;

		// Token: 0x04001CD8 RID: 7384
		public Vector2 steerControls;

		// Token: 0x04001CD9 RID: 7385
		public AnimationCurve leanCurve;

		// Token: 0x04001CDA RID: 7386
		[Tooltip("X: Steer Sensitivity, Y: Steer Gravity")]
		public float axisAngle;

		// Token: 0x04001CDB RID: 7387
		public Vector3 centerOfMassOffset;

		// Token: 0x04001CDC RID: 7388
		[HideInInspector]
		public bool isReversing;

		// Token: 0x04001CDD RID: 7389
		[HideInInspector]
		public bool isAirborne;

		// Token: 0x04001CDE RID: 7390
		[HideInInspector]
		public bool stuntMode;

		// Token: 0x04001CDF RID: 7391
		[HideInInspector]
		public Rigidbody rb;

		// Token: 0x04001CE0 RID: 7392
		[HideInInspector]
		public Rigidbody fWheelRb;

		// Token: 0x04001CE1 RID: 7393
		[HideInInspector]
		public Rigidbody secondaryFWheelRb;

		// Token: 0x04001CE2 RID: 7394
		[HideInInspector]
		public Rigidbody rWheelRb;

		// Token: 0x04001CE3 RID: 7395
		private float turnAngle;

		// Token: 0x04001CE4 RID: 7396
		private float xQuat;

		// Token: 0x04001CE5 RID: 7397
		private float zQuat;

		// Token: 0x04001CE6 RID: 7398
		public float turnLeanAmount;

		// Token: 0x04001CE7 RID: 7399
		private RaycastHit hit;

		// Token: 0x04001CE8 RID: 7400
		public float customSteerAxis;

		// Token: 0x04001CE9 RID: 7401
		public float customLeanAxis;

		// Token: 0x04001CEA RID: 7402
		public float customAccelerationAxis;

		// Token: 0x04001CEB RID: 7403
		public float rawCustomAccelerationAxis;

		// Token: 0x04001CEC RID: 7404
		private bool isRaw;

		// Token: 0x04001CED RID: 7405
		[HideInInspector]
		public float currentTopSpeed;

		// Token: 0x04001CEE RID: 7406
		[HideInInspector]
		public float pickUpSpeed;

		// Token: 0x04001CEF RID: 7407
		private Quaternion initialLowerForkLocalRotaion;

		// Token: 0x04001CF0 RID: 7408
		private Quaternion initialHandlesRotation;

		// Token: 0x04001CF1 RID: 7409
		private ConfigurableJoint fPhysicsWheelConfigJoint;

		// Token: 0x04001CF2 RID: 7410
		private ConfigurableJoint secondaryFPhysicsWheelConfigJoint;

		// Token: 0x04001CF3 RID: 7411
		private ConfigurableJoint rPhysicsWheelConfigJoint;

		// Token: 0x04001CF4 RID: 7412
		public bool groundConformity;

		// Token: 0x04001CF5 RID: 7413
		private RaycastHit hitGround;

		// Token: 0x04001CF6 RID: 7414
		private Vector3 theRay;

		// Token: 0x04001CF7 RID: 7415
		private float groundZ;

		// Token: 0x04001CF8 RID: 7416
		private JointDrive fDrive;

		// Token: 0x04001CF9 RID: 7417
		private JointDrive rYDrive;

		// Token: 0x04001CFA RID: 7418
		private JointDrive rZDrive;

		// Token: 0x04001CFB RID: 7419
		[HideInInspector]
		public Vector3 lastVelocity;

		// Token: 0x04001CFC RID: 7420
		[HideInInspector]
		public Vector3 deceleration;

		// Token: 0x04001CFD RID: 7421
		[HideInInspector]
		public Vector3 lastDeceleration;

		// Token: 0x04001CFE RID: 7422
		public AirTimeSettings airTimeSettings;

		// Token: 0x04001CFF RID: 7423
		private int currGear;

		// Token: 0x04001D00 RID: 7424
		private int prevGear;

		// Token: 0x04001D01 RID: 7425
		private bool changeGear;

		// Token: 0x04001D02 RID: 7426
		[HideInInspector]
		public bool wheelieInput;

		// Token: 0x04001D03 RID: 7427
		[HideInInspector]
		public bool stuntInput;

		// Token: 0x04001D04 RID: 7428
		[HideInInspector]
		public float wheeliePower;

		// Token: 0x04001D05 RID: 7429
		private int powerDivision;

		// Token: 0x04001D06 RID: 7430
		private int additiveBonus;

		// Token: 0x04001D07 RID: 7431
		public ControlRef cr;

		// Token: 0x04001D08 RID: 7432
		public bool braked;

		// Token: 0x04001D09 RID: 7433
		public Interactor interactor;
	}
}
