using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000332 RID: 818
	public class AiCarContrtoller : MonoBehaviour
	{
		// Token: 0x060014EF RID: 5359 RVA: 0x000DD831 File Offset: 0x000DBA31
		public void start_Vehicle()
		{
			this.StopVehicle = false;
			this.rws.enabled = true;
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x000DD846 File Offset: 0x000DBA46
		public void stop_Vehicle()
		{
			this.StopVehicle = true;
			this.rws.enabled = false;
			this.getOutStation = false;
			this.getOutGas = false;
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x000DD86C File Offset: 0x000DBA6C
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.grounded = false;
			this.engineSounds[1].mute = true;
			this.rb.centerOfMass = this.CenterOfMass.localPosition;
			Vector3 a = Vector3.zero;
			for (int i = 0; i < this.TireMeshes.Length; i++)
			{
				a += this.TireMeshes[i].parent.parent.localPosition;
			}
			a.y = 0f;
			this.centerOfMass_ground = a / 4f;
			if (base.GetComponent<GravityCustom>())
			{
				this.VehicleGravity = base.GetComponent<GravityCustom>().gravity;
				return;
			}
			this.VehicleGravity = Physics.gravity.y;
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x000DD933 File Offset: 0x000DBB33
		private void Start()
		{
			this.stop_Vehicle();
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x000DD93C File Offset: 0x000DBB3C
		private void FixedUpdate()
		{
			if (this.getOutStation && !this.StopVehicle)
			{
				this.wpt.progressNum = 0;
				this.patrol.GetOut();
				this.stop_Vehicle();
			}
			if (this.getOutGas && !this.StopVehicle)
			{
				this.wpt.progressNum++;
				this.patrol.GetOutGasStation();
				this.stop_Vehicle();
			}
			this.carVelocity = base.transform.InverseTransformDirection(this.rb.velocity);
			this.curveVelocity = Mathf.Abs(this.carVelocity.magnitude) / 100f;
			float num;
			if (this.sensorScript.obstacleInPath)
			{
				num = (this.StopVehicle ? 0f : (this.turnTorque * -this.sensorScript.turnmultiplyer * Time.fixedDeltaTime * 1000f));
			}
			else
			{
				num = (this.StopVehicle ? 0f : (this.turnTorque * this.TurnAI * Time.fixedDeltaTime * 1000f));
			}
			float num2 = this.StopVehicle ? 0f : (this.accelerationForce * this.SpeedAI * Time.fixedDeltaTime * 1000f);
			if (this.carVelocity.z > this.maxSpeed)
			{
				num2 = 0f;
			}
			this.brakeInput = (this.StopVehicle ? (this.brakeForce * Time.fixedDeltaTime * 1000f) : (this.brakeForce * -this.brakeAI * Time.fixedDeltaTime * 1000f));
			this.brakeInput *= Mathf.Clamp01(this.carVelocity.magnitude);
			this.speedValue = num2 * this.accelerationCurve.Evaluate(Mathf.Abs(this.carVelocity.z) / 100f);
			if (this.separateReverseCurve && this.carVelocity.z < 0f && num2 < 0f)
			{
				this.speedValue = num2 * this.ReverseCurve.Evaluate(Mathf.Abs(this.carVelocity.z) / 100f);
			}
			this.fricValue = this.frictionForce * this.frictionCurve.Evaluate(this.carVelocity.magnitude / 100f);
			Vector3 position = this.TargetTransform.position;
			position.y = base.transform.position.y;
			Vector3 normalized = (position - base.transform.position).normalized;
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			this.desiredTurning = Mathf.Abs(Vector3.Angle(forward, normalized));
			this.turnValue = num * this.turnCurve.Evaluate(this.desiredTurning / this.TurnAngle);
			if (Physics.Raycast(this.groundCheck.position, -base.transform.up, out this.hit, this.maxRayLength))
			{
				this.accelarationLogic();
				this.turningLogic();
				this.frictionLogic();
				this.brakeLogic();
				this.rb.angularDrag = this.dragAmount * this.driftCurve.Evaluate(Mathf.Abs(this.carVelocity.x) / 70f);
				Debug.DrawLine(this.groundCheck.position, this.hit.point, Color.green);
				this.grounded = true;
				this.rb.drag = 0.1f;
				if (this.StopVehicle)
				{
					this.rb.drag = 5f;
				}
				this.rb.centerOfMass = this.centerOfMass_ground;
				return;
			}
			this.grounded = false;
			this.rb.drag = 0.1f;
			this.rb.centerOfMass = this.CenterOfMass.localPosition;
			if (!this.airDrag)
			{
				this.rb.angularDrag = 0.1f;
			}
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x000DDD34 File Offset: 0x000DBF34
		private void Update()
		{
			this.tireVisuals();
			this.audioControl();
			this.SetTargetPosition(this.TargetTransform.position);
			float num = 1f;
			float num2 = Vector3.Distance(base.transform.position, this.targetPosition);
			Vector3 normalized = (this.targetPosition - base.transform.position).normalized;
			float num3 = Vector3.Dot(base.transform.forward, normalized);
			this.angleToMove = Vector3.Angle(base.transform.forward, normalized);
			if (this.angleToMove > this.brakeAngle || this.sensorScript.obstacleInPath)
			{
				if (this.carVelocity.z > 15f)
				{
					this.brakeAI = -1f;
				}
				else
				{
					this.brakeAI = 0f;
				}
			}
			else
			{
				this.brakeAI = 0f;
			}
			if (num2 <= num)
			{
				float z = this.carVelocity.z;
				this.TurnAI = 0f;
				return;
			}
			if (num3 > 0f)
			{
				this.SpeedAI = 1f;
				float num4 = 5f;
				float num5 = 5f;
				if (num2 < num4 && this.curveVelocity > num5)
				{
				}
			}
			else
			{
				float num6 = 5f;
				if (num2 > num6)
				{
					this.SpeedAI = 1f;
				}
			}
			if (Vector3.SignedAngle(base.transform.forward, normalized, Vector3.up) > 0f)
			{
				this.TurnAI = 1f * this.turnCurve.Evaluate(this.desiredTurning / this.TurnAngle);
				return;
			}
			this.TurnAI = -1f * this.turnCurve.Evaluate(this.desiredTurning / this.TurnAngle);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x000DDEEA File Offset: 0x000DC0EA
		public void SetTargetPosition(Vector3 TargetPos)
		{
			this.targetPosition = TargetPos;
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x000DDEF4 File Offset: 0x000DC0F4
		public void audioControl()
		{
			if (this.grounded)
			{
				if (Mathf.Abs(this.carVelocity.x) > this.SkidEnable - 0.1f)
				{
					this.engineSounds[1].mute = false;
				}
				else
				{
					this.engineSounds[1].mute = true;
				}
			}
			else
			{
				this.engineSounds[1].mute = true;
			}
			this.engineSounds[1].pitch = 1f;
			this.engineSounds[0].pitch = 2f * this.engineCurve.Evaluate(this.curveVelocity);
			if (this.engineSounds.Length == 2)
			{
				return;
			}
			this.engineSounds[2].pitch = 2f * this.engineCurve.Evaluate(this.curveVelocity);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x000DDFBC File Offset: 0x000DC1BC
		public void tireVisuals()
		{
			foreach (Transform transform in this.TireMeshes)
			{
				transform.transform.RotateAround(transform.transform.position, transform.transform.right, this.carVelocity.z * 2f);
			}
			foreach (Transform transform2 in this.TurnTires)
			{
				if (this.sensorScript.obstacleInPath)
				{
					transform2.localRotation = Quaternion.Slerp(transform2.localRotation, Quaternion.Euler(transform2.localRotation.eulerAngles.x, Mathf.Clamp(this.desiredTurning, this.desiredTurning, this.TurnAngle * 2f) * -this.sensorScript.turnmultiplyer, transform2.localRotation.eulerAngles.z), this.slerpTime);
				}
				else
				{
					transform2.localRotation = Quaternion.Slerp(transform2.localRotation, Quaternion.Euler(transform2.localRotation.eulerAngles.x, Mathf.Clamp(this.desiredTurning, this.desiredTurning, this.TurnAngle * 2f) * this.TurnAI, transform2.localRotation.eulerAngles.z), this.slerpTime);
				}
			}
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x000DE118 File Offset: 0x000DC318
		public void accelarationLogic()
		{
			if (this.SpeedAI > 0.1f)
			{
				this.rb.AddForceAtPosition(base.transform.forward * this.speedValue, this.groundCheck.position);
			}
			if (this.SpeedAI < -0.1f)
			{
				this.rb.AddForceAtPosition(base.transform.forward * this.speedValue, this.groundCheck.position);
			}
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x000DE198 File Offset: 0x000DC398
		public void turningLogic()
		{
			if (this.carVelocity.z > 0.1f)
			{
				this.rb.AddTorque(base.transform.up * this.turnValue);
			}
			if (this.carVelocity.z < -0.1f)
			{
				this.rb.AddTorque(base.transform.up * -this.turnValue);
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x000DE20C File Offset: 0x000DC40C
		public void frictionLogic()
		{
			Vector3 a = this.carVelocity.x * base.transform.right;
			Vector3 vector = -a / Time.fixedDeltaTime;
			float maxLength = this.rb.mass * vector.magnitude;
			Vector3 b = -Vector3.Project(this.VehicleGravity * this.rb.mass * Vector3.up, base.transform.right);
			Vector3 a2 = Vector3.ClampMagnitude(this.fricValue * 50f * -a.normalized, maxLength);
			this.rb.AddForceAtPosition(a2 + b, this.fricAt.position);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x000DE2D0 File Offset: 0x000DC4D0
		public void brakeLogic()
		{
			if (this.carVelocity.z > 0.1f)
			{
				this.rb.AddForceAtPosition(base.transform.forward * -this.brakeInput, this.groundCheck.position);
			}
			if (this.carVelocity.z < -0.1f)
			{
				this.rb.AddForceAtPosition(base.transform.forward * this.brakeInput, this.groundCheck.position);
			}
		}

		// Token: 0x0400255A RID: 9562
		[Header("Suspension")]
		[Range(0f, 5f)]
		public float SuspensionDistance = 0.2f;

		// Token: 0x0400255B RID: 9563
		public float suspensionForce = 30000f;

		// Token: 0x0400255C RID: 9564
		public float suspensionDamper = 200f;

		// Token: 0x0400255D RID: 9565
		public Transform groundCheck;

		// Token: 0x0400255E RID: 9566
		public Transform fricAt;

		// Token: 0x0400255F RID: 9567
		public Transform CenterOfMass;

		// Token: 0x04002560 RID: 9568
		private Rigidbody rb;

		// Token: 0x04002561 RID: 9569
		[Header("Car Stats")]
		public float accelerationForce = 200f;

		// Token: 0x04002562 RID: 9570
		public float turnTorque = 100f;

		// Token: 0x04002563 RID: 9571
		public float brakeForce = 150f;

		// Token: 0x04002564 RID: 9572
		public float frictionForce = 70f;

		// Token: 0x04002565 RID: 9573
		public float dragAmount = 4f;

		// Token: 0x04002566 RID: 9574
		public float TurnAngle = 30f;

		// Token: 0x04002567 RID: 9575
		public float maxRayLength = 0.8f;

		// Token: 0x04002568 RID: 9576
		public float slerpTime = 0.2f;

		// Token: 0x04002569 RID: 9577
		[HideInInspector]
		public bool grounded;

		// Token: 0x0400256A RID: 9578
		public Transform TargetTransform;

		// Token: 0x0400256B RID: 9579
		[Header("Visuals")]
		public Transform[] TireMeshes;

		// Token: 0x0400256C RID: 9580
		public Transform[] TurnTires;

		// Token: 0x0400256D RID: 9581
		[Header("Curves")]
		public AnimationCurve frictionCurve;

		// Token: 0x0400256E RID: 9582
		public AnimationCurve accelerationCurve;

		// Token: 0x0400256F RID: 9583
		public bool separateReverseCurve;

		// Token: 0x04002570 RID: 9584
		public AnimationCurve ReverseCurve;

		// Token: 0x04002571 RID: 9585
		public AnimationCurve turnCurve;

		// Token: 0x04002572 RID: 9586
		public AnimationCurve driftCurve;

		// Token: 0x04002573 RID: 9587
		public AnimationCurve engineCurve;

		// Token: 0x04002574 RID: 9588
		private float speedValue;

		// Token: 0x04002575 RID: 9589
		private float fricValue;

		// Token: 0x04002576 RID: 9590
		private float turnValue;

		// Token: 0x04002577 RID: 9591
		private float curveVelocity;

		// Token: 0x04002578 RID: 9592
		private float brakeInput;

		// Token: 0x04002579 RID: 9593
		private float angleToMove;

		// Token: 0x0400257A RID: 9594
		[HideInInspector]
		public Vector3 carVelocity;

		// Token: 0x0400257B RID: 9595
		[HideInInspector]
		public RaycastHit hit;

		// Token: 0x0400257C RID: 9596
		[Header("Other Settings")]
		public AudioSource[] engineSounds;

		// Token: 0x0400257D RID: 9597
		public bool airDrag;

		// Token: 0x0400257E RID: 9598
		public float SkidEnable = 20f;

		// Token: 0x0400257F RID: 9599
		public float skidWidth = 0.12f;

		// Token: 0x04002580 RID: 9600
		private float frictionAngle;

		// Token: 0x04002581 RID: 9601
		[HideInInspector]
		public float TurnAI = 1f;

		// Token: 0x04002582 RID: 9602
		[HideInInspector]
		public float SpeedAI = 1f;

		// Token: 0x04002583 RID: 9603
		[HideInInspector]
		public float brakeAI;

		// Token: 0x04002584 RID: 9604
		private Vector3 targetPosition;

		// Token: 0x04002585 RID: 9605
		public float brakeAngle = 30f;

		// Token: 0x04002586 RID: 9606
		public Sensors sensorScript;

		// Token: 0x04002587 RID: 9607
		private float desiredTurning;

		// Token: 0x04002588 RID: 9608
		private float VehicleGravity = -30f;

		// Token: 0x04002589 RID: 9609
		private Vector3 centerOfMass_ground;

		// Token: 0x0400258A RID: 9610
		public bool StopVehicle;

		// Token: 0x0400258B RID: 9611
		public bool getOutStation;

		// Token: 0x0400258C RID: 9612
		public bool getOutGas;

		// Token: 0x0400258D RID: 9613
		public float maxSpeed;

		// Token: 0x0400258E RID: 9614
		public WaypointProgressTracker wpt;

		// Token: 0x0400258F RID: 9615
		public ReverseWhenStuck rws;

		// Token: 0x04002590 RID: 9616
		public bool hasChased;

		// Token: 0x04002591 RID: 9617
		public PatrolCar patrol;
	}
}
