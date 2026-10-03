using System;
using System.Collections;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000339 RID: 825
	public class WaypointProgressTracker : MonoBehaviour
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x000DF207 File Offset: 0x000DD407
		// (set) Token: 0x06001520 RID: 5408 RVA: 0x000DF20F File Offset: 0x000DD40F
		public WaypointCircuit.RoutePoint targetPoint { get; private set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x000DF218 File Offset: 0x000DD418
		// (set) Token: 0x06001522 RID: 5410 RVA: 0x000DF220 File Offset: 0x000DD420
		public WaypointCircuit.RoutePoint speedPoint { get; private set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06001523 RID: 5411 RVA: 0x000DF229 File Offset: 0x000DD429
		// (set) Token: 0x06001524 RID: 5412 RVA: 0x000DF231 File Offset: 0x000DD431
		public WaypointCircuit.RoutePoint progressPoint { get; private set; }

		// Token: 0x06001525 RID: 5413 RVA: 0x000DF23A File Offset: 0x000DD43A
		private void Start()
		{
			if (this.target == null)
			{
				this.target = new GameObject(base.name + " Waypoint Target").transform;
			}
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x000DF26C File Offset: 0x000DD46C
		public void EnterGarage()
		{
			this.garageDoor.Open();
			this.circuit = this.entryCircuit;
			this.progressNum = 0;
			this.target.position = this.circuit.Waypoints[this.progressNum].position;
			this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x000DF2DB File Offset: 0x000DD4DB
		private IEnumerator GarageReset()
		{
			yield return new WaitForSeconds(3f);
			this.circuit = this.exitCircuit;
			this.progressNum = 0;
			this.progressDistance = 0f;
			this.target.position = this.circuit.Waypoints[this.progressNum].position;
			this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
			yield break;
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x000DF2EC File Offset: 0x000DD4EC
		public void Recalibrate()
		{
			int num = this.circuitTemp;
			Debug.Log("recalibrate");
			if (num == 0)
			{
				this.circuit = this.circuit2;
			}
			else if (num == 1)
			{
				this.circuit = this.circuit3;
			}
			else if (num == 2)
			{
				this.circuit = this.circuit4;
			}
			else if (num == 3)
			{
				this.circuit = this.circuit5;
			}
			else if (num == 4)
			{
				this.circuit = this.lostCircuitDump;
			}
			else if (num == 5)
			{
				this.circuit = this.lostCircuitNorth;
			}
			else if (num == 6)
			{
				this.circuit = this.lostCircuitSouth;
			}
			this.progressNum = this.progressNumTemp;
			if (this.progressStyle == WaypointProgressTracker.ProgressStyle.PointToPoint)
			{
				this.target.position = this.circuit.Waypoints[this.progressNum].position;
				this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
			}
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x000DF3E0 File Offset: 0x000DD5E0
		public void Reset()
		{
			Debug.Log("reset");
			this.progressDistance = 0f;
			this.progressNum = 0;
			this.garageDoor.Close();
			int num = Random.Range(0, 4);
			if (num == 0)
			{
				this.circuit = this.circuit2;
			}
			else if (num == 1)
			{
				this.circuit = this.circuit3;
			}
			else if (num == 2)
			{
				this.circuit = this.circuit4;
			}
			else if (num == 3)
			{
				this.circuit = this.circuit5;
			}
			if (this.progressStyle == WaypointProgressTracker.ProgressStyle.PointToPoint)
			{
				this.target.position = this.circuit.Waypoints[this.progressNum].position;
				this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
			}
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x000DF4B0 File Offset: 0x000DD6B0
		private void Update()
		{
			bool grounded = this.aicc.grounded;
			if (this.progressStyle == WaypointProgressTracker.ProgressStyle.SmoothAlongRoute)
			{
				if (Time.deltaTime > 0f)
				{
					this.speed = this.aicc.carVelocity.z;
				}
				this.target.position = this.circuit.GetRoutePoint(this.progressDistance + this.lookAheadForTargetOffset + this.lookAheadForTargetFactor * this.speed).position;
				this.target.rotation = Quaternion.LookRotation(this.circuit.GetRoutePoint(this.progressDistance + this.lookAheadForSpeedOffset + this.lookAheadForSpeedFactor * this.speed).direction);
				this.progressPoint = this.circuit.GetRoutePoint(this.progressDistance);
				Vector3 lhs = this.progressPoint.position - base.transform.position;
				if (Vector3.Dot(lhs, this.progressPoint.direction) < 0f)
				{
					this.progressDistance += lhs.magnitude * 0.5f;
				}
				this.lastPosition = base.transform.position;
				return;
			}
			if ((this.target.position - base.transform.position).magnitude < this.pointToPointThreshold)
			{
				this.progressNum = (this.progressNum + 1) % this.circuit.Waypoints.Length;
				if (this.progressNum > this.circuit.Waypoints.Length)
				{
					this.progressNum--;
				}
			}
			this.target.position = this.circuit.Waypoints[this.progressNum].position;
			this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
			if (this.circuit.Waypoints[this.progressNum].name == "station")
			{
				this.aicc.getOutStation = true;
				this.rws.totalReverseCount = 0;
				base.StartCoroutine(this.GarageReset());
				Debug.Log("stationcheck");
			}
			else if (this.circuit.Waypoints[this.progressNum].name == "gasstation")
			{
				if (!this.officer.playerCuffed)
				{
					this.aicc.getOutGas = true;
					Debug.Log("stationcheck");
				}
			}
			else if (this.circuit.Waypoints[this.progressNum].name == "stationGate")
			{
				this.EnterGarage();
				Debug.Log("stationenter");
			}
			else if (this.circuit.Waypoints[this.progressNum].name == "stationExitPoint")
			{
				Debug.Log("stationExitPoint");
				this.Reset();
			}
			else
			{
				this.aicc.getOutStation = false;
			}
			this.progressPoint = this.circuit.GetRoutePoint(this.progressDistance);
			Vector3 lhs2 = this.progressPoint.position - base.transform.position;
			if (Vector3.Dot(lhs2, this.progressPoint.direction) < 0f)
			{
				this.progressDistance += lhs2.magnitude;
			}
			this.lastPosition = base.transform.position;
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x00002188 File Offset: 0x00000388
		private void OnDrawGizmos()
		{
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x000DF814 File Offset: 0x000DDA14
		public void RespawnOnRoad()
		{
			base.transform.position = this.target.position + new Vector3(0f, 1f, 0f);
			base.transform.rotation = this.target.rotation;
			base.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			base.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x000DF890 File Offset: 0x000DDA90
		public void RespawnAtStation()
		{
			this.target.position = this.stationLoc.position;
			this.target.rotation = this.stationLoc.rotation;
			base.transform.position = this.stationLoc.position + new Vector3(0f, 1f, 0f);
			base.transform.rotation = this.stationLoc.rotation;
			base.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			base.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			this.progressNum = 0;
			this.aicc.patrol.GetOut();
			this.aicc.stop_Vehicle();
			this.rws.totalReverseCount = 0;
			base.StartCoroutine(this.GarageReset());
		}

		// Token: 0x040025CB RID: 9675
		public WaypointCircuit circuit;

		// Token: 0x040025CC RID: 9676
		public WaypointCircuit circuit2;

		// Token: 0x040025CD RID: 9677
		public WaypointCircuit circuit3;

		// Token: 0x040025CE RID: 9678
		public WaypointCircuit circuit4;

		// Token: 0x040025CF RID: 9679
		public WaypointCircuit circuit5;

		// Token: 0x040025D0 RID: 9680
		public WaypointCircuit lostCircuitDump;

		// Token: 0x040025D1 RID: 9681
		public WaypointCircuit lostCircuitNorth;

		// Token: 0x040025D2 RID: 9682
		public WaypointCircuit lostCircuitSouth;

		// Token: 0x040025D3 RID: 9683
		public WaypointCircuit entryCircuit;

		// Token: 0x040025D4 RID: 9684
		public WaypointCircuit exitCircuit;

		// Token: 0x040025D5 RID: 9685
		public PatrolGate garageDoor;

		// Token: 0x040025D6 RID: 9686
		public Officer officer;

		// Token: 0x040025D7 RID: 9687
		public Transform stationLoc;

		// Token: 0x040025D8 RID: 9688
		public ReverseWhenStuck rws;

		// Token: 0x040025D9 RID: 9689
		[SerializeField]
		private float lookAheadForTargetOffset = 5f;

		// Token: 0x040025DA RID: 9690
		[SerializeField]
		private float lookAheadForTargetFactor = 0.1f;

		// Token: 0x040025DB RID: 9691
		private float lookAheadForSpeedOffset = 50f;

		// Token: 0x040025DC RID: 9692
		private float lookAheadForSpeedFactor = 0.2f;

		// Token: 0x040025DD RID: 9693
		[SerializeField]
		private WaypointProgressTracker.ProgressStyle progressStyle;

		// Token: 0x040025DE RID: 9694
		private float pointToPointThreshold = 4f;

		// Token: 0x040025E2 RID: 9698
		public Transform target;

		// Token: 0x040025E3 RID: 9699
		[HideInInspector]
		public float progressDistance;

		// Token: 0x040025E4 RID: 9700
		public int progressNum;

		// Token: 0x040025E5 RID: 9701
		private Vector3 lastPosition;

		// Token: 0x040025E6 RID: 9702
		private float speed;

		// Token: 0x040025E7 RID: 9703
		public AiCarContrtoller aicc;

		// Token: 0x040025E8 RID: 9704
		public int progressNumTemp;

		// Token: 0x040025E9 RID: 9705
		public int circuitTemp;

		// Token: 0x020004F3 RID: 1267
		public enum ProgressStyle
		{
			// Token: 0x04002CE1 RID: 11489
			SmoothAlongRoute,
			// Token: 0x04002CE2 RID: 11490
			PointToPoint
		}
	}
}
