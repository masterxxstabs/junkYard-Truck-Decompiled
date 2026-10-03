using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class AnyCarAI : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000063 RID: 99 RVA: 0x00004EE2 File Offset: 0x000030E2
	// (set) Token: 0x06000064 RID: 100 RVA: 0x00004EEA File Offset: 0x000030EA
	public float AccelInput { get; private set; }

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000065 RID: 101 RVA: 0x00004EF3 File Offset: 0x000030F3
	// (set) Token: 0x06000066 RID: 102 RVA: 0x00004EFB File Offset: 0x000030FB
	public float BrakeInput { get; private set; }

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000067 RID: 103 RVA: 0x00004F04 File Offset: 0x00003104
	// (set) Token: 0x06000068 RID: 104 RVA: 0x00004F0C File Offset: 0x0000310C
	public Rigidbody rb { get; set; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000069 RID: 105 RVA: 0x00004F15 File Offset: 0x00003115
	// (set) Token: 0x0600006A RID: 106 RVA: 0x00004F1D File Offset: 0x0000311D
	public float RPM { get; private set; }

	// Token: 0x0600006B RID: 107 RVA: 0x00004F28 File Offset: 0x00003128
	public void UnpackPrefab()
	{
		Transform parent = base.transform.parent;
		this.objToUnpack = parent.gameObject;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004F50 File Offset: 0x00003150
	public void CreateColliders()
	{
		this.frontLeft.AddComponent(typeof(SphereCollider));
		this.frontRight.AddComponent(typeof(SphereCollider));
		this.backLeft.AddComponent(typeof(SphereCollider));
		this.backRight.AddComponent(typeof(SphereCollider));
		this.frontLeftCol = new GameObject("FLCOL");
		this.frontRightCol = new GameObject("FRCOL");
		this.backLeftCol = new GameObject("BLCOL");
		this.backRightCol = new GameObject("BRCOL");
		this.frontLeftCol.transform.parent = base.transform.GetChild(0);
		this.frontLeftCol.transform.position = this.frontLeft.transform.position;
		this.frontLeftCol.transform.rotation = this.frontLeft.transform.rotation;
		this.frontRightCol.transform.parent = base.transform.GetChild(0);
		this.frontRightCol.transform.position = this.frontRight.transform.position;
		this.frontRightCol.transform.rotation = this.frontRight.transform.rotation;
		this.backLeftCol.transform.parent = base.transform.GetChild(0);
		this.backLeftCol.transform.position = this.backLeft.transform.position;
		this.backLeftCol.transform.rotation = this.backLeft.transform.rotation;
		this.backRightCol.transform.parent = base.transform.GetChild(0);
		this.backRightCol.transform.position = this.backRight.transform.position;
		this.backRightCol.transform.rotation = this.backRight.transform.rotation;
		this.frontLeftCol.AddComponent(typeof(WheelCollider));
		this.frontRightCol.AddComponent(typeof(WheelCollider));
		this.backLeftCol.AddComponent(typeof(WheelCollider));
		this.backRightCol.AddComponent(typeof(WheelCollider));
		this.frontLeftCol.AddComponent(typeof(WheelsFXAI));
		this.frontRightCol.AddComponent(typeof(WheelsFXAI));
		this.backLeftCol.AddComponent(typeof(WheelsFXAI));
		this.backRightCol.AddComponent(typeof(WheelsFXAI));
		this.FLRadius = this.frontLeft.GetComponent<SphereCollider>().radius * this.frontLeft.transform.lossyScale.x;
		this.FRRadius = this.frontRight.GetComponent<SphereCollider>().radius * this.frontLeft.transform.lossyScale.x;
		this.BLRadius = this.backLeft.GetComponent<SphereCollider>().radius * this.frontLeft.transform.lossyScale.x;
		this.BRRadius = this.backRight.GetComponent<SphereCollider>().radius * this.frontLeft.transform.lossyScale.x;
		this.frontLeftCol.GetComponent<WheelCollider>().radius = Mathf.Abs(this.FLRadius);
		this.frontRightCol.GetComponent<WheelCollider>().radius = Mathf.Abs(this.FRRadius);
		this.backLeftCol.GetComponent<WheelCollider>().radius = Mathf.Abs(this.BLRadius);
		this.backRightCol.GetComponent<WheelCollider>().radius = Mathf.Abs(this.BRRadius);
		Object.DestroyImmediate(this.frontLeft.GetComponent<SphereCollider>());
		Object.DestroyImmediate(this.frontRight.GetComponent<SphereCollider>());
		Object.DestroyImmediate(this.backLeft.GetComponent<SphereCollider>());
		Object.DestroyImmediate(this.backRight.GetComponent<SphereCollider>());
		if (this.bodyMesh != null)
		{
			this.bodyMesh.AddComponent(typeof(MeshCollider));
			this.bodyMesh.AddComponent(typeof(CompetitiveDrivingCheck));
			this.bodyMesh.GetComponent<MeshCollider>().convex = true;
		}
		this.extraWheelsColList.Clear();
		int num = 1;
		foreach (CarWheelsAI carWheelsAI in this.extraWheels)
		{
			carWheelsAI.model.AddComponent(typeof(SphereCollider));
			this.extraWheelCol.collider = new GameObject("extraWheel " + num);
			this.extraWheelCol.axel = carWheelsAI.axel;
			this.extraWheelCol.type = carWheelsAI.type;
			this.extraWheelCol.collider.transform.parent = base.transform.GetChild(0);
			this.extraWheelCol.collider.transform.position = carWheelsAI.model.transform.position;
			this.extraWheelCol.collider.transform.rotation = carWheelsAI.model.transform.rotation;
			this.extraWheelCol.collider.AddComponent(typeof(WheelCollider));
			this.extraWheelCol.collider.AddComponent(typeof(WheelsFXAI));
			this.extraWheelRadius = carWheelsAI.model.GetComponent<SphereCollider>().radius * carWheelsAI.model.transform.lossyScale.x;
			this.extraWheelCol.collider.GetComponent<WheelCollider>().radius = Mathf.Abs(this.extraWheelRadius);
			Object.DestroyImmediate(carWheelsAI.model.GetComponent<SphereCollider>());
			this.extraWheelsColList.Add(this.extraWheelCol);
			num++;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00005584 File Offset: 0x00003784
	public void CreateDebugBodyCol()
	{
		this.extraBodyCol = (Object.Instantiate(Resources.Load("BodyCollider"), base.transform.position, Quaternion.identity) as GameObject);
		this.extraBodyCol.transform.parent = base.transform;
		this.extraBodyCol.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		this.extraBodyCol.transform.rotation = base.transform.rotation;
		this.bodyMesh = this.extraBodyCol;
		this.bodyMesh.AddComponent(typeof(CompetitiveDrivingCheck));
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00005654 File Offset: 0x00003854
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.mass = this.vehicleMass;
		this.centerOfMass = this.rb.centerOfMass;
		this.currentTorque = this.motorTorque - this.tractionControl * this.motorTorque;
		this.frontLeftCol = base.transform.GetChild(0).GetChild(0).gameObject;
		this.frontRightCol = base.transform.GetChild(0).GetChild(1).gameObject;
		this.backLeftCol = base.transform.GetChild(0).GetChild(2).gameObject;
		this.backRightCol = base.transform.GetChild(0).GetChild(3).gameObject;
		this.rearLights = base.transform.GetChild(1).GetChild(2).gameObject;
		this.rearLights.SetActive(false);
		this.smokeParticles = base.transform.root.GetComponentInChildren<ParticleSystem>();
		if (!this.smokeOn)
		{
			this.smokeParticles.Stop();
		}
		else
		{
			this.smokeParticles.Play();
		}
		this.carAIInputs = base.gameObject.AddComponent<CarAIInputs>();
		this.carAIWaypointTracker.enabled = true;
		base.gameObject.AddComponent<EngineAudioAI>();
		base.gameObject.AddComponent<DamageSystemAI>();
		this.carAItargetObj = new GameObject("WaypointsTarget");
		this.carAItargetObj.transform.parent = base.transform.GetChild(1);
		this.carAItarget = this.carAItargetObj.transform;
		this.carAItargetNonChar = this.carAItargetObj.transform;
		if (this.exhaustFlame && this.exhaustObj == null)
		{
			this.exhaustObj = base.transform.GetChild(1).Find("ExhaustPipe(Clone)").gameObject;
			this.exhaustVisual = this.exhaustObj.GetComponent<ParticleSystem>();
			this.exhaustSoundSource = this.exhaustObj.GetComponent<AudioSource>();
			this.exhaustSoundSource.clip = this.exhaustSound;
			this.exhaustSoundSource.volume = this.exhaustVolume;
		}
		this.SetWheelsValues();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00005880 File Offset: 0x00003A80
	private void Update()
	{
		SpeedTypeAI speedTypeAI = this.speedType;
		if (speedTypeAI != SpeedTypeAI.MPH)
		{
			if (speedTypeAI == SpeedTypeAI.KPH)
			{
				this.currentSpeed = this.rb.velocity.magnitude * 3.6f;
			}
		}
		else
		{
			this.currentSpeed = this.rb.velocity.magnitude * 2.2369363f;
		}
		this.isDrivingDebug();
	}

	// Token: 0x06000070 RID: 112 RVA: 0x000058E4 File Offset: 0x00003AE4
	public void Move(float steering, float Accel, float footbrake, float handbrake)
	{
		this.AnimateWheels();
		steering = Mathf.Clamp(steering, -1f, 1f);
		Accel = (this.AccelInput = Mathf.Clamp(Accel, 0f, 1f));
		footbrake = (this.BrakeInput = -1f * Mathf.Clamp(footbrake, -1f, 0f));
		handbrake = Mathf.Clamp(handbrake, 0f, 1f);
		this.CalculateRPM();
		this.AutoGearSystem();
		this.Steering(steering);
		this.SteerHelper();
		this.ApplyDrive(Accel, footbrake);
		this.MaxSpeedReached();
		this.HandBreaking(handbrake);
		this.AddDownForce();
		if (this.skidMarks)
		{
			this.CheckForWheelSpin();
		}
		this.TractionControl();
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000059A0 File Offset: 0x00003BA0
	private void AnimateWheels()
	{
		Vector3 position;
		Quaternion quaternion;
		this.frontLeftCol.GetComponent<WheelCollider>().GetWorldPose(out position, out quaternion);
		quaternion *= Quaternion.Euler(this.wheelsRotation);
		this.frontLeft.transform.position = position;
		this.frontLeft.transform.rotation = quaternion;
		Vector3 position2;
		Quaternion quaternion2;
		this.frontRightCol.GetComponent<WheelCollider>().GetWorldPose(out position2, out quaternion2);
		quaternion2 *= Quaternion.Euler(this.wheelsRotation);
		this.frontRight.transform.position = position2;
		this.frontRight.transform.rotation = quaternion2;
		Vector3 position3;
		Quaternion quaternion3;
		this.backLeftCol.GetComponent<WheelCollider>().GetWorldPose(out position3, out quaternion3);
		quaternion3 *= Quaternion.Euler(this.wheelsRotation);
		this.backLeft.transform.position = position3;
		this.backLeft.transform.rotation = quaternion3;
		Vector3 position4;
		Quaternion quaternion4;
		this.backRightCol.GetComponent<WheelCollider>().GetWorldPose(out position4, out quaternion4);
		quaternion4 *= Quaternion.Euler(this.wheelsRotation);
		this.backRight.transform.position = position4;
		this.backRight.transform.rotation = quaternion4;
		int num = 0;
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			Vector3 position5;
			Quaternion quaternion5;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().GetWorldPose(out position5, out quaternion5);
			quaternion5 *= Quaternion.Euler(this.wheelsRotation);
			this.extraWheels[num].model.transform.position = position5;
			this.extraWheels[num].model.transform.rotation = quaternion5;
			num++;
		}
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00005B80 File Offset: 0x00003D80
	private void Steering(float steering)
	{
		float b = steering * this.maximumSteerAngle;
		this.frontLeftCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(this.frontLeftCol.GetComponent<WheelCollider>().steerAngle, b, 0.5f);
		this.frontRightCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(this.frontRightCol.GetComponent<WheelCollider>().steerAngle, b, 0.5f);
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			if (carWheelsColsAI.axel == AxlAI.Front)
			{
				carWheelsColsAI.collider.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(carWheelsColsAI.collider.GetComponent<WheelCollider>().steerAngle, b, 0.5f);
			}
		}
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00005C60 File Offset: 0x00003E60
	private void SteerHelper()
	{
		WheelHit wheelHit;
		this.frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
		WheelHit wheelHit2;
		this.frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit2);
		WheelHit wheelHit3;
		this.backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit3);
		WheelHit wheelHit4;
		this.backRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit4);
		if (wheelHit.normal == Vector3.zero)
		{
			return;
		}
		if (wheelHit2.normal == Vector3.zero)
		{
			return;
		}
		if (wheelHit3.normal == Vector3.zero)
		{
			return;
		}
		if (wheelHit4.normal == Vector3.zero)
		{
			return;
		}
		int num = 0;
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			WheelHit wheelHit5;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit5);
			if (wheelHit5.normal == Vector3.zero)
			{
				return;
			}
			num++;
		}
		if (Mathf.Abs(this.oldRotation - base.transform.eulerAngles.y) < 10f)
		{
			Quaternion rotation = Quaternion.AngleAxis((base.transform.eulerAngles.y - this.oldRotation) * this.steerHelper, Vector3.up);
			this.rb.velocity = rotation * this.rb.velocity;
		}
		this.oldRotation = base.transform.eulerAngles.y;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00005DFC File Offset: 0x00003FFC
	private void ApplyDrive(float Accel, float footbrake)
	{
		float num;
		switch (this.carDriveType)
		{
		case CarDriveTypeAI.FrontWheelDrive:
			break;
		case CarDriveTypeAI.RearWheelDrive:
			goto IL_15D;
		case CarDriveTypeAI.FourWheelDrive:
			num = Accel * (this.currentTorque / 4f) * this.enginePower.Evaluate(1f);
			this.frontLeftCol.GetComponent<WheelCollider>().motorTorque = num;
			this.frontRightCol.GetComponent<WheelCollider>().motorTorque = num;
			this.backLeftCol.GetComponent<WheelCollider>().motorTorque = num;
			this.backRightCol.GetComponent<WheelCollider>().motorTorque = num;
			using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CarWheelsColsAI carWheelsColsAI = enumerator.Current;
					if (carWheelsColsAI.type == TypeAI.Drive)
					{
						carWheelsColsAI.collider.GetComponent<WheelCollider>().motorTorque = num;
					}
				}
				goto IL_1EA;
			}
			break;
		default:
			goto IL_1EA;
		}
		num = Accel * (this.currentTorque / 2f) * this.enginePower.Evaluate(1f);
		this.frontLeftCol.GetComponent<WheelCollider>().motorTorque = num;
		this.frontRightCol.GetComponent<WheelCollider>().motorTorque = num;
		using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CarWheelsColsAI carWheelsColsAI2 = enumerator.Current;
				if (carWheelsColsAI2.type == TypeAI.Drive)
				{
					carWheelsColsAI2.collider.GetComponent<WheelCollider>().motorTorque = num;
				}
			}
			goto IL_1EA;
		}
		IL_15D:
		num = Accel * (this.currentTorque / 2f) * this.enginePower.Evaluate(1f);
		this.backLeftCol.GetComponent<WheelCollider>().motorTorque = num;
		this.backRightCol.GetComponent<WheelCollider>().motorTorque = num;
		foreach (CarWheelsColsAI carWheelsColsAI3 in this.extraWheelsColList)
		{
			if (carWheelsColsAI3.type == TypeAI.Drive)
			{
				carWheelsColsAI3.collider.GetComponent<WheelCollider>().motorTorque = num;
			}
		}
		IL_1EA:
		if (footbrake > 0f)
		{
			if (this.currentSpeed > 5f && Vector3.Angle(base.transform.forward, this.rb.velocity) < 50f)
			{
				this.reverseGearOn = false;
			}
			else
			{
				this.reverseGearOn = true;
			}
		}
		else
		{
			this.reverseGearOn = false;
		}
		if (!this.ABS)
		{
			if (this.currentSpeed > 5f && Vector3.Angle(base.transform.forward, this.rb.velocity) < 50f)
			{
				this.frontLeftCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
				this.frontRightCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
				this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
				this.backRightCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
				foreach (CarWheelsColsAI carWheelsColsAI4 in this.extraWheelsColList)
				{
					carWheelsColsAI4.collider.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
				}
				if (footbrake > 0f)
				{
					this.rearLights.SetActive(true);
					return;
				}
				this.rearLights.SetActive(false);
				return;
			}
			else
			{
				if (footbrake <= 0f)
				{
					return;
				}
				this.rearLights.SetActive(false);
				this.frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
				this.frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
				this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
				this.backRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
				this.frontLeftCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
				this.frontRightCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
				this.backLeftCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
				this.backRightCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
				using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CarWheelsColsAI carWheelsColsAI5 = enumerator.Current;
						carWheelsColsAI5.collider.GetComponent<WheelCollider>().brakeTorque = 0f;
						carWheelsColsAI5.collider.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
					}
					return;
				}
			}
		}
		if (this.currentSpeed > 5f && Vector3.Angle(base.transform.forward, this.rb.velocity) < 50f)
		{
			base.StartCoroutine(this.ABSCoroutine(footbrake));
			if (footbrake > 0f)
			{
				this.rearLights.SetActive(true);
				return;
			}
			this.rearLights.SetActive(false);
			return;
		}
		else if (footbrake > 0f)
		{
			this.rearLights.SetActive(false);
			this.frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
			this.frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
			this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
			this.backRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
			this.frontLeftCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
			this.frontRightCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
			this.backLeftCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
			this.backRightCol.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
			foreach (CarWheelsColsAI carWheelsColsAI6 in this.extraWheelsColList)
			{
				carWheelsColsAI6.collider.GetComponent<WheelCollider>().brakeTorque = 0f;
				carWheelsColsAI6.collider.GetComponent<WheelCollider>().motorTorque = -this.reverseTorque * footbrake;
			}
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x0000644C File Offset: 0x0000464C
	private void MaxSpeedReached()
	{
		SpeedTypeAI speedTypeAI = this.speedType;
		if (speedTypeAI != SpeedTypeAI.MPH)
		{
			if (speedTypeAI != SpeedTypeAI.KPH)
			{
				return;
			}
			if (this.currentSpeed > this.maxSpeed)
			{
				this.rb.velocity = this.maxSpeed / 3.6f * this.rb.velocity.normalized;
			}
		}
		else if (this.currentSpeed > this.maxSpeed)
		{
			this.rb.velocity = this.maxSpeed / 2.2369363f * this.rb.velocity.normalized;
			return;
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000064E4 File Offset: 0x000046E4
	private void HandBreaking(float handbrake)
	{
		if (handbrake > 0f)
		{
			float num = handbrake * this.handbrakeTorque;
			this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = num;
			this.backRightCol.GetComponent<WheelCollider>().brakeTorque = num;
		}
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00006524 File Offset: 0x00004724
	private IEnumerator ABSCoroutine(float footbrake)
	{
		this.frontLeftCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
		this.frontRightCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
		this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
		this.backRightCol.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			carWheelsColsAI.collider.GetComponent<WheelCollider>().brakeTorque = this.brakeTorque * footbrake;
		}
		yield return new WaitForSeconds(0.1f);
		this.frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
		this.frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
		this.backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0f;
		this.backRightCol.GetComponent<WheelCollider>().brakeTorque = 0f;
		foreach (CarWheelsColsAI carWheelsColsAI2 in this.extraWheelsColList)
		{
			carWheelsColsAI2.collider.GetComponent<WheelCollider>().brakeTorque = 0f;
		}
		yield return new WaitForSeconds(0.1f);
		yield break;
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0000653C File Offset: 0x0000473C
	private void isDrivingDebug()
	{
		if (this.rb.velocity.magnitude < 1f && this.isDriving)
		{
			if (this.carAIInputs.reverseGearOn)
			{
				this.rb.AddRelativeForce(0f, 0f, -this.vehicleMass * 1000f * Time.deltaTime);
				return;
			}
			this.rb.AddRelativeForce(0f, 0f, this.vehicleMass * 1000f * Time.deltaTime);
		}
	}

	// Token: 0x06000079 RID: 121 RVA: 0x000065C8 File Offset: 0x000047C8
	private void SetWheelsValues()
	{
		this.frontLeftCol.GetComponent<WheelCollider>().mass = this.wheelsMass;
		this.frontRightCol.GetComponent<WheelCollider>().mass = this.wheelsMass;
		this.backLeftCol.GetComponent<WheelCollider>().mass = this.wheelsMass;
		this.backRightCol.GetComponent<WheelCollider>().mass = this.wheelsMass;
		this.frontLeftCol.GetComponent<WheelCollider>().radius *= this.wheelsRadius;
		this.frontRightCol.GetComponent<WheelCollider>().radius *= this.wheelsRadius;
		this.backLeftCol.GetComponent<WheelCollider>().radius *= this.wheelsRadius;
		this.backRightCol.GetComponent<WheelCollider>().radius *= this.wheelsRadius;
		this.frontLeftCol.GetComponent<WheelCollider>().forceAppPointDistance = this.forcePoint;
		this.frontRightCol.GetComponent<WheelCollider>().forceAppPointDistance = this.forcePoint;
		this.backLeftCol.GetComponent<WheelCollider>().forceAppPointDistance = this.forcePoint;
		this.backRightCol.GetComponent<WheelCollider>().forceAppPointDistance = this.forcePoint;
		this.frontLeftCol.GetComponent<WheelCollider>().wheelDampingRate = this.dumpingRate;
		this.frontRightCol.GetComponent<WheelCollider>().wheelDampingRate = this.dumpingRate;
		this.backLeftCol.GetComponent<WheelCollider>().wheelDampingRate = this.dumpingRate;
		this.backRightCol.GetComponent<WheelCollider>().wheelDampingRate = this.dumpingRate;
		this.frontLeftCol.GetComponent<WheelCollider>().suspensionDistance = this.suspensionDistance;
		this.frontRightCol.GetComponent<WheelCollider>().suspensionDistance = this.suspensionDistance;
		this.backLeftCol.GetComponent<WheelCollider>().suspensionDistance = this.suspensionDistance;
		this.backRightCol.GetComponent<WheelCollider>().suspensionDistance = this.suspensionDistance;
		this.frontLeftCol.GetComponent<WheelCollider>().center = this.wheelsPosition;
		this.frontRightCol.GetComponent<WheelCollider>().center = this.wheelsPosition;
		this.backLeftCol.GetComponent<WheelCollider>().center = this.wheelsPosition;
		this.backRightCol.GetComponent<WheelCollider>().center = this.wheelsPosition;
		JointSpring jointSpring = this.frontLeftCol.GetComponent<WheelCollider>().suspensionSpring;
		JointSpring jointSpring2 = this.frontRightCol.GetComponent<WheelCollider>().suspensionSpring;
		JointSpring jointSpring3 = this.backLeftCol.GetComponent<WheelCollider>().suspensionSpring;
		JointSpring jointSpring4 = this.backRightCol.GetComponent<WheelCollider>().suspensionSpring;
		jointSpring.spring = this.suspensionSpring;
		jointSpring2.spring = this.suspensionSpring;
		jointSpring3.spring = this.suspensionSpring;
		jointSpring4.spring = this.suspensionSpring;
		jointSpring.damper = this.suspensionDamper;
		jointSpring2.damper = this.suspensionDamper;
		jointSpring3.damper = this.suspensionDamper;
		jointSpring4.damper = this.suspensionDamper;
		jointSpring.targetPosition = this.targetPosition;
		jointSpring2.targetPosition = this.targetPosition;
		jointSpring3.targetPosition = this.targetPosition;
		jointSpring4.targetPosition = this.targetPosition;
		this.frontLeftCol.GetComponent<WheelCollider>().suspensionSpring = jointSpring;
		this.frontRightCol.GetComponent<WheelCollider>().suspensionSpring = jointSpring2;
		this.backLeftCol.GetComponent<WheelCollider>().suspensionSpring = jointSpring3;
		this.backRightCol.GetComponent<WheelCollider>().suspensionSpring = jointSpring4;
		WheelFrictionCurve sidewaysFriction = this.frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
		WheelFrictionCurve sidewaysFriction2 = this.frontRightCol.GetComponent<WheelCollider>().sidewaysFriction;
		WheelFrictionCurve sidewaysFriction3 = this.backLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
		WheelFrictionCurve sidewaysFriction4 = this.backRightCol.GetComponent<WheelCollider>().sidewaysFriction;
		sidewaysFriction.stiffness = this.wheelStiffness;
		sidewaysFriction2.stiffness = this.wheelStiffness;
		sidewaysFriction3.stiffness = this.wheelStiffness;
		sidewaysFriction4.stiffness = this.wheelStiffness;
		this.frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
		this.frontRightCol.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction2;
		this.backLeftCol.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction3;
		this.backRightCol.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction4;
		WheelFrictionCurve forwardFriction = this.frontLeftCol.GetComponent<WheelCollider>().forwardFriction;
		WheelFrictionCurve forwardFriction2 = this.frontRightCol.GetComponent<WheelCollider>().forwardFriction;
		WheelFrictionCurve forwardFriction3 = this.backLeftCol.GetComponent<WheelCollider>().forwardFriction;
		WheelFrictionCurve forwardFriction4 = this.backRightCol.GetComponent<WheelCollider>().forwardFriction;
		forwardFriction.stiffness = this.wheelStiffness;
		forwardFriction2.stiffness = this.wheelStiffness;
		forwardFriction3.stiffness = this.wheelStiffness;
		forwardFriction4.stiffness = this.wheelStiffness;
		this.frontLeftCol.GetComponent<WheelCollider>().forwardFriction = forwardFriction;
		this.frontRightCol.GetComponent<WheelCollider>().forwardFriction = forwardFriction2;
		this.backLeftCol.GetComponent<WheelCollider>().forwardFriction = forwardFriction3;
		this.backRightCol.GetComponent<WheelCollider>().forwardFriction = forwardFriction4;
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			carWheelsColsAI.collider.GetComponent<WheelCollider>().mass = this.wheelsMass;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().radius = this.extraWheelRadius * this.wheelsRadius;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().forceAppPointDistance = this.forcePoint;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().wheelDampingRate = this.dumpingRate;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().suspensionDistance = this.suspensionDistance;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().center = this.wheelsPosition;
			JointSpring jointSpring5 = carWheelsColsAI.collider.GetComponent<WheelCollider>().suspensionSpring;
			jointSpring5.spring = this.suspensionSpring;
			jointSpring5.damper = this.suspensionDamper;
			jointSpring5.targetPosition = this.targetPosition;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().suspensionSpring = jointSpring5;
			WheelFrictionCurve sidewaysFriction5 = carWheelsColsAI.collider.GetComponent<WheelCollider>().sidewaysFriction;
			sidewaysFriction5.stiffness = this.wheelStiffness;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction5;
		}
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00006C04 File Offset: 0x00004E04
	private void AutoGearSystem()
	{
		float num = Mathf.Abs(this.currentSpeed / this.maxSpeed);
		float num2 = 1f / (float)this.numberOfGears * (float)(this.currentGear + 1);
		float num3 = 1f / (float)this.numberOfGears * (float)this.currentGear;
		if (this.currentGear > 0 && num < num3)
		{
			this.currentGear--;
		}
		if (num > num2 && this.currentGear < this.numberOfGears - 1 && !this.reverseGearOn)
		{
			if (this.exhaustFlame)
			{
				this.ExhaustFX();
			}
			this.currentGear++;
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00006CA5 File Offset: 0x00004EA5
	private static float BiasCurve(float factor)
	{
		return 1f - (1f - factor) * (1f - factor);
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00006CBC File Offset: 0x00004EBC
	private static float SmoothLerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00006CCC File Offset: 0x00004ECC
	private void CalculateGearFactor()
	{
		float num = 1f / (float)this.numberOfGears;
		float b = Mathf.InverseLerp(num * (float)this.currentGear, num * (float)(this.currentGear + 1), Mathf.Abs(this.currentSpeed / this.maxSpeed));
		this.gearFactor = Mathf.Lerp(this.gearFactor, b, Time.deltaTime * 5f);
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00006D30 File Offset: 0x00004F30
	private void CalculateRPM()
	{
		this.CalculateGearFactor();
		float num = (float)this.currentGear / (float)this.numberOfGears;
		float from = AnyCarAI.SmoothLerp(0f, this.rpmRange, AnyCarAI.BiasCurve(num));
		float to = AnyCarAI.SmoothLerp(this.rpmRange, 1f, num);
		this.RPM = AnyCarAI.SmoothLerp(from, to, this.gearFactor);
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00006D90 File Offset: 0x00004F90
	private void AddDownForce()
	{
		this.rb.AddForce(-base.transform.up * this.downForce * this.rb.velocity.magnitude);
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00006DDC File Offset: 0x00004FDC
	private void TractionControl()
	{
		WheelHit wheelHit;
		WheelHit wheelHit2;
		WheelHit wheelHit3;
		WheelHit wheelHit4;
		switch (this.carDriveType)
		{
		case CarDriveTypeAI.FrontWheelDrive:
			goto IL_18C;
		case CarDriveTypeAI.RearWheelDrive:
			break;
		case CarDriveTypeAI.FourWheelDrive:
			this.frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
			this.AdjustTorque(wheelHit.forwardSlip);
			this.frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit2);
			this.AdjustTorque(wheelHit2.forwardSlip);
			this.backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit3);
			this.AdjustTorque(wheelHit3.forwardSlip);
			this.backRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit4);
			this.AdjustTorque(wheelHit4.forwardSlip);
			using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CarWheelsColsAI carWheelsColsAI = enumerator.Current;
					WheelHit wheelHit5;
					carWheelsColsAI.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit5);
					this.AdjustTorque(wheelHit5.forwardSlip);
				}
				return;
			}
			break;
		default:
			return;
		}
		this.backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit3);
		this.AdjustTorque(wheelHit3.forwardSlip);
		this.backRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit4);
		this.AdjustTorque(wheelHit4.forwardSlip);
		using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CarWheelsColsAI carWheelsColsAI2 = enumerator.Current;
				if (carWheelsColsAI2.axel == AxlAI.Rear)
				{
					WheelHit wheelHit5;
					carWheelsColsAI2.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit5);
					this.AdjustTorque(wheelHit5.forwardSlip);
				}
			}
			return;
		}
		IL_18C:
		this.frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
		this.AdjustTorque(wheelHit.forwardSlip);
		this.frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit2);
		this.AdjustTorque(wheelHit2.forwardSlip);
		foreach (CarWheelsColsAI carWheelsColsAI3 in this.extraWheelsColList)
		{
			if (carWheelsColsAI3.axel == AxlAI.Front)
			{
				WheelHit wheelHit5;
				carWheelsColsAI3.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit5);
				this.AdjustTorque(wheelHit5.forwardSlip);
			}
		}
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00007038 File Offset: 0x00005238
	private void AdjustTorque(float forwardSlip)
	{
		if (forwardSlip >= this.slipLimit && this.currentTorque >= 0f)
		{
			this.currentTorque -= 10f * this.tractionControl;
			return;
		}
		this.currentTorque += 10f * this.tractionControl;
		if (this.currentTorque > this.motorTorque)
		{
			this.currentTorque = this.motorTorque;
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000070A8 File Offset: 0x000052A8
	private void CheckForWheelSpin()
	{
		WheelHit wheelHit;
		this.frontLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit);
		WheelHit wheelHit2;
		this.frontRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit2);
		WheelHit wheelHit3;
		this.backLeftCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit3);
		WheelHit wheelHit4;
		this.backRightCol.GetComponent<WheelCollider>().GetGroundHit(out wheelHit4);
		if (Mathf.Abs(wheelHit.forwardSlip) >= this.slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= this.slipLimit)
		{
			this.frontLeftCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();
			if (!this.AnySkidSoundPlaying())
			{
				this.frontLeftCol.GetComponent<WheelsFXAI>().PlayAudio();
			}
		}
		else
		{
			if (this.frontLeftCol.GetComponent<WheelsFXAI>().playingAudio)
			{
				this.frontLeftCol.GetComponent<WheelsFXAI>().StopAudio();
			}
			this.frontLeftCol.GetComponent<WheelsFXAI>().EndSkidTrail();
		}
		if (Mathf.Abs(wheelHit2.forwardSlip) >= this.slipLimit || Mathf.Abs(wheelHit2.sidewaysSlip) >= this.slipLimit)
		{
			this.frontRightCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();
			if (!this.AnySkidSoundPlaying())
			{
				this.frontRightCol.GetComponent<WheelsFXAI>().PlayAudio();
			}
		}
		else
		{
			if (this.frontRightCol.GetComponent<WheelsFXAI>().playingAudio)
			{
				this.frontRightCol.GetComponent<WheelsFXAI>().StopAudio();
			}
			this.frontRightCol.GetComponent<WheelsFXAI>().EndSkidTrail();
		}
		if (Mathf.Abs(wheelHit3.forwardSlip) >= this.slipLimit || Mathf.Abs(wheelHit3.sidewaysSlip) >= this.slipLimit)
		{
			this.backLeftCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();
			if (!this.AnySkidSoundPlaying())
			{
				this.backLeftCol.GetComponent<WheelsFXAI>().PlayAudio();
			}
		}
		else
		{
			if (this.backLeftCol.GetComponent<WheelsFXAI>().playingAudio)
			{
				this.backLeftCol.GetComponent<WheelsFXAI>().StopAudio();
			}
			this.backLeftCol.GetComponent<WheelsFXAI>().EndSkidTrail();
		}
		if (Mathf.Abs(wheelHit4.forwardSlip) >= this.slipLimit || Mathf.Abs(wheelHit4.sidewaysSlip) >= this.slipLimit)
		{
			this.backRightCol.GetComponent<WheelsFXAI>().EmitTyreSmoke();
			if (!this.AnySkidSoundPlaying())
			{
				this.backRightCol.GetComponent<WheelsFXAI>().PlayAudio();
			}
		}
		else
		{
			if (this.backRightCol.GetComponent<WheelsFXAI>().playingAudio)
			{
				this.backRightCol.GetComponent<WheelsFXAI>().StopAudio();
			}
			this.backRightCol.GetComponent<WheelsFXAI>().EndSkidTrail();
		}
		foreach (CarWheelsColsAI carWheelsColsAI in this.extraWheelsColList)
		{
			WheelHit wheelHit5;
			carWheelsColsAI.collider.GetComponent<WheelCollider>().GetGroundHit(out wheelHit5);
			if (Mathf.Abs(wheelHit5.forwardSlip) >= this.slipLimit || Mathf.Abs(wheelHit5.sidewaysSlip) >= this.slipLimit)
			{
				carWheelsColsAI.collider.GetComponent<WheelsFXAI>().EmitTyreSmoke();
				if (!this.AnySkidSoundPlaying())
				{
					carWheelsColsAI.collider.GetComponent<WheelsFXAI>().PlayAudio();
				}
			}
			else
			{
				if (carWheelsColsAI.collider.GetComponent<WheelsFXAI>().playingAudio)
				{
					carWheelsColsAI.collider.GetComponent<WheelsFXAI>().StopAudio();
				}
				carWheelsColsAI.collider.GetComponent<WheelsFXAI>().EndSkidTrail();
			}
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000073F8 File Offset: 0x000055F8
	private bool AnySkidSoundPlaying()
	{
		if (this.frontLeftCol.GetComponent<WheelsFXAI>().playingAudio)
		{
			return true;
		}
		if (this.frontRightCol.GetComponent<WheelsFXAI>().playingAudio)
		{
			return true;
		}
		if (this.backLeftCol.GetComponent<WheelsFXAI>().playingAudio)
		{
			return true;
		}
		if (this.backRightCol.GetComponent<WheelsFXAI>().playingAudio)
		{
			return true;
		}
		using (List<CarWheelsColsAI>.Enumerator enumerator = this.extraWheelsColList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.collider.GetComponent<WheelsFXAI>().playingAudio)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000074AC File Offset: 0x000056AC
	public void ExhaustFX()
	{
		if (this.exhaustObj != null)
		{
			this.exhaustSoundSource.Play();
			this.exhaustVisual.Play();
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x000074D2 File Offset: 0x000056D2
	public void CreateExhaustGameObj()
	{
		this.exhaustObjectPrefab = Resources.Load<Transform>("ExhaustPipe");
		this.exhaustObject = Object.Instantiate<Transform>(this.exhaustObjectPrefab);
		this.exhaustObject.transform.parent = base.transform.GetChild(1);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00007511 File Offset: 0x00005711
	public void DestroyExhaustGameObj()
	{
		this.exhaustObj = base.transform.GetChild(1).Find("ExhaustPipe(Clone)").gameObject;
		Object.DestroyImmediate(this.exhaustObj, true);
	}

	// Token: 0x04000103 RID: 259
	public int toolbarTab;

	// Token: 0x04000104 RID: 260
	public string currentTab;

	// Token: 0x04000105 RID: 261
	public List<CarWheelsAI> extraWheels;

	// Token: 0x04000106 RID: 262
	public List<CarWheelsColsAI> extraWheelsColList = new List<CarWheelsColsAI>();

	// Token: 0x04000107 RID: 263
	public CarWheelsColsAI extraWheelCol;

	// Token: 0x04000108 RID: 264
	public GameObject frontLeft;

	// Token: 0x04000109 RID: 265
	public GameObject frontRight;

	// Token: 0x0400010A RID: 266
	public GameObject backLeft;

	// Token: 0x0400010B RID: 267
	public GameObject backRight;

	// Token: 0x0400010C RID: 268
	public GameObject frontLeftCol;

	// Token: 0x0400010D RID: 269
	public GameObject frontRightCol;

	// Token: 0x0400010E RID: 270
	public GameObject backLeftCol;

	// Token: 0x0400010F RID: 271
	public GameObject backRightCol;

	// Token: 0x04000110 RID: 272
	public float extraWheelRadius;

	// Token: 0x04000111 RID: 273
	private float FLRadius;

	// Token: 0x04000112 RID: 274
	private float FRRadius;

	// Token: 0x04000113 RID: 275
	private float BLRadius;

	// Token: 0x04000114 RID: 276
	private float BRRadius;

	// Token: 0x04000115 RID: 277
	public float wheelsMass = 20f;

	// Token: 0x04000116 RID: 278
	public float forcePoint;

	// Token: 0x04000117 RID: 279
	public float dumpingRate = 0.025f;

	// Token: 0x04000118 RID: 280
	public float suspensionDistance = 0.2f;

	// Token: 0x04000119 RID: 281
	public Vector3 wheelsPosition;

	// Token: 0x0400011A RID: 282
	public Vector3 wheelsRotation;

	// Token: 0x0400011B RID: 283
	[Range(0.1f, 1f)]
	public float wheelStiffness = 1f;

	// Token: 0x0400011C RID: 284
	public float suspensionSpring = 70000f;

	// Token: 0x0400011D RID: 285
	public float suspensionDamper = 3500f;

	// Token: 0x0400011E RID: 286
	[Range(0.1f, 1f)]
	public float targetPosition = 0.5f;

	// Token: 0x0400011F RID: 287
	[Range(0.5f, 2f)]
	public float wheelsRadius = 1f;

	// Token: 0x04000120 RID: 288
	public GameObject bodyMesh;

	// Token: 0x04000121 RID: 289
	public GameObject extraBodyCol;

	// Token: 0x04000124 RID: 292
	public AnimationCurve enginePower;

	// Token: 0x04000125 RID: 293
	public float maximumSteerAngle;

	// Token: 0x04000126 RID: 294
	[Range(0f, 1f)]
	public float steerHelper;

	// Token: 0x04000127 RID: 295
	[Range(0f, 0.5f)]
	public float tractionControl;

	// Token: 0x04000128 RID: 296
	[Range(0f, 0.5f)]
	public float slipLimit = 0.3f;

	// Token: 0x04000129 RID: 297
	[SerializeField]
	public CarDriveTypeAI carDriveType = CarDriveTypeAI.FourWheelDrive;

	// Token: 0x0400012A RID: 298
	[SerializeField]
	public SpeedTypeAI speedType = SpeedTypeAI.KPH;

	// Token: 0x0400012B RID: 299
	public Vector3 centerOfMass;

	// Token: 0x0400012C RID: 300
	public float vehicleMass = 1000f;

	// Token: 0x0400012D RID: 301
	public float motorTorque = 2500f;

	// Token: 0x0400012E RID: 302
	public float brakeTorque = 20000f;

	// Token: 0x0400012F RID: 303
	public float reverseTorque = 500f;

	// Token: 0x04000130 RID: 304
	public float handbrakeTorque = 10000000f;

	// Token: 0x04000131 RID: 305
	public float maxSpeed = 200f;

	// Token: 0x04000132 RID: 306
	public int numberOfGears = 5;

	// Token: 0x04000133 RID: 307
	public float downForce = 300f;

	// Token: 0x04000134 RID: 308
	public bool ABS = true;

	// Token: 0x04000135 RID: 309
	public bool skidMarks = true;

	// Token: 0x04000136 RID: 310
	public bool smokeOn;

	// Token: 0x04000137 RID: 311
	private ParticleSystem smokeParticles;

	// Token: 0x04000138 RID: 312
	public bool turboON;

	// Token: 0x04000139 RID: 313
	public AudioClip turboAudioClip;

	// Token: 0x0400013A RID: 314
	[Range(0f, 1f)]
	public float turboVolume = 0.5f;

	// Token: 0x0400013B RID: 315
	public Transform exhaustObjectPrefab;

	// Token: 0x0400013C RID: 316
	public Transform exhaustObject;

	// Token: 0x0400013D RID: 317
	public GameObject exhaustObj;

	// Token: 0x0400013E RID: 318
	public bool exhaustFlame;

	// Token: 0x0400013F RID: 319
	public ParticleSystem exhaustVisual;

	// Token: 0x04000140 RID: 320
	public AudioSource exhaustSoundSource;

	// Token: 0x04000141 RID: 321
	public AudioClip exhaustSound;

	// Token: 0x04000142 RID: 322
	[Range(0.01f, 1f)]
	public float exhaustVolume;

	// Token: 0x04000143 RID: 323
	private float oldRotation;

	// Token: 0x04000145 RID: 325
	public float currentSpeed;

	// Token: 0x04000146 RID: 326
	private float currentTorque;

	// Token: 0x04000147 RID: 327
	private float rpmRange = 1f;

	// Token: 0x04000148 RID: 328
	public int currentGear;

	// Token: 0x04000149 RID: 329
	private float gearFactor;

	// Token: 0x0400014A RID: 330
	public bool reverseGearOn;

	// Token: 0x0400014C RID: 332
	public GameObject objToUnpack;

	// Token: 0x0400014D RID: 333
	public GameObject frontLights;

	// Token: 0x0400014E RID: 334
	public GameObject rearLights;

	// Token: 0x0400014F RID: 335
	public bool collisionSystem;

	// Token: 0x04000150 RID: 336
	public AudioClip collisionSound;

	// Token: 0x04000151 RID: 337
	[Range(0.01f, 1f)]
	public float collisionVolume;

	// Token: 0x04000152 RID: 338
	public OptionalMeshesAI[] optionalMeshList;

	// Token: 0x04000153 RID: 339
	[Range(0.01f, 50f)]
	public float demolutionStrenght;

	// Token: 0x04000154 RID: 340
	[Range(0.1f, 500f)]
	public float demolutionRange;

	// Token: 0x04000155 RID: 341
	public bool customMesh;

	// Token: 0x04000156 RID: 342
	public bool collisionParticles;

	// Token: 0x04000157 RID: 343
	public AudioClip skidSound;

	// Token: 0x04000158 RID: 344
	[Range(0.01f, 1f)]
	public float skidVolume = 0.3f;

	// Token: 0x04000159 RID: 345
	public AudioClip lowAcceleration;

	// Token: 0x0400015A RID: 346
	public AudioClip lowDeceleration;

	// Token: 0x0400015B RID: 347
	public AudioClip highAcceleration;

	// Token: 0x0400015C RID: 348
	public AudioClip highDeceleration;

	// Token: 0x0400015D RID: 349
	[Range(0.01f, 1f)]
	public float engineVolume;

	// Token: 0x0400015E RID: 350
	public AudioSource suspensionsSource;

	// Token: 0x0400015F RID: 351
	public AudioSource skidSource;

	// Token: 0x04000160 RID: 352
	public AudioClip suspensionsSound;

	// Token: 0x04000161 RID: 353
	[Range(0f, 1f)]
	public float suspensionsVolume;

	// Token: 0x04000162 RID: 354
	public CarAIInputs carAIInputs;

	// Token: 0x04000163 RID: 355
	public CarAIWaipointTracker carAIWaypointTracker;

	// Token: 0x04000164 RID: 356
	[SerializeField]
	public BrakeCondition brakeCondition = BrakeCondition.TargetDistance;

	// Token: 0x04000165 RID: 357
	[SerializeField]
	[Range(0f, 1f)]
	public float cautiousSpeedFactor = 0.05f;

	// Token: 0x04000166 RID: 358
	[SerializeField]
	[Range(0f, 180f)]
	public float cautiousAngle = 50f;

	// Token: 0x04000167 RID: 359
	[SerializeField]
	[Range(0f, 200f)]
	public float cautiousDistance = 100f;

	// Token: 0x04000168 RID: 360
	[SerializeField]
	public float cautiousAngularVelocityFactor = 30f;

	// Token: 0x04000169 RID: 361
	[SerializeField]
	[Range(0f, 0.1f)]
	public float steerSensitivity = 0.05f;

	// Token: 0x0400016A RID: 362
	[SerializeField]
	[Range(0f, 0.1f)]
	public float accelSensitivity = 0.04f;

	// Token: 0x0400016B RID: 363
	[SerializeField]
	[Range(0f, 1f)]
	public float brakeSensitivity = 1f;

	// Token: 0x0400016C RID: 364
	[SerializeField]
	[Range(0f, 10f)]
	public float lateralWander = 3f;

	// Token: 0x0400016D RID: 365
	[SerializeField]
	public float lateralWanderSpeed = 0.5f;

	// Token: 0x0400016E RID: 366
	[SerializeField]
	[Range(0f, 1f)]
	public float wanderAmount = 0.1f;

	// Token: 0x0400016F RID: 367
	[SerializeField]
	public float accelWanderSpeed = 0.1f;

	// Token: 0x04000170 RID: 368
	[SerializeField]
	public bool isDriving;

	// Token: 0x04000171 RID: 369
	[SerializeField]
	public Transform carAItarget;

	// Token: 0x04000172 RID: 370
	public Transform carAItargetNonChar;

	// Token: 0x04000173 RID: 371
	private GameObject carAItargetObj;

	// Token: 0x04000174 RID: 372
	[SerializeField]
	public bool stopWhenTargetReached;

	// Token: 0x04000175 RID: 373
	[SerializeField]
	public float reachTargetThreshold = 2f;

	// Token: 0x04000176 RID: 374
	[SerializeField]
	[Range(15f, 50f)]
	public float sensorsAngle;

	// Token: 0x04000177 RID: 375
	[SerializeField]
	public float avoidDistance = 10f;

	// Token: 0x04000178 RID: 376
	[SerializeField]
	public float brakeDistance = 6f;

	// Token: 0x04000179 RID: 377
	[SerializeField]
	public float reverseDistance = 3f;

	// Token: 0x0400017A RID: 378
	public bool persuitAiOn;

	// Token: 0x0400017B RID: 379
	public GameObject persuitTarget;

	// Token: 0x0400017C RID: 380
	public float persuitDistance;

	// Token: 0x0400017D RID: 381
	[SerializeField]
	public ProgressStyle progressStyle;

	// Token: 0x0400017E RID: 382
	[SerializeField]
	public WaypointsPath AIcircuit;

	// Token: 0x0400017F RID: 383
	[SerializeField]
	public WaypointsPath AIcircuit2;

	// Token: 0x04000180 RID: 384
	[SerializeField]
	public WaypointsPath AIcircuit3;

	// Token: 0x04000181 RID: 385
	[SerializeField]
	public WaypointsPath AIcircuit4;

	// Token: 0x04000182 RID: 386
	[SerializeField]
	public WaypointsPath AIcircuitLost1;

	// Token: 0x04000183 RID: 387
	[SerializeField]
	public WaypointsPath AIcircuitLost2;

	// Token: 0x04000184 RID: 388
	[SerializeField]
	public WaypointsPath AIcircuitLost3;

	// Token: 0x04000185 RID: 389
	[SerializeField]
	[Range(5f, 50f)]
	public float lookAheadForTarget = 5f;

	// Token: 0x04000186 RID: 390
	[SerializeField]
	public float lookAheadForTargetFactor = 0.1f;

	// Token: 0x04000187 RID: 391
	[SerializeField]
	public float lookAheadForSpeedOffset = 10f;

	// Token: 0x04000188 RID: 392
	[SerializeField]
	public float lookAheadForSpeedFactor = 0.2f;

	// Token: 0x04000189 RID: 393
	[SerializeField]
	[Range(1f, 10f)]
	public float pointThreshold = 4f;

	// Token: 0x0400018A RID: 394
	public Transform AItarget;
}
