using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class CarAIWaipointTracker : MonoBehaviour
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000091 RID: 145 RVA: 0x00008573 File Offset: 0x00006773
	// (set) Token: 0x06000092 RID: 146 RVA: 0x0000857B File Offset: 0x0000677B
	[HideInInspector]
	public WaypointsPath.RoutePoint targetPoint { get; private set; }

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000093 RID: 147 RVA: 0x00008584 File Offset: 0x00006784
	// (set) Token: 0x06000094 RID: 148 RVA: 0x0000858C File Offset: 0x0000678C
	[HideInInspector]
	public WaypointsPath.RoutePoint speedPoint { get; private set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000095 RID: 149 RVA: 0x00008595 File Offset: 0x00006795
	// (set) Token: 0x06000096 RID: 150 RVA: 0x0000859D File Offset: 0x0000679D
	[HideInInspector]
	public WaypointsPath.RoutePoint progressPoint { get; private set; }

	// Token: 0x06000097 RID: 151 RVA: 0x000085A8 File Offset: 0x000067A8
	private void Start()
	{
		this.ACAI = base.GetComponent<AnyCarAI>();
		this.circuit2 = this.ACAI.AIcircuit2;
		this.circuit3 = this.ACAI.AIcircuit3;
		this.lookAheadForTargetOffset = this.ACAI.lookAheadForTarget;
		this.lookAheadForTargetFactor = this.ACAI.lookAheadForTargetFactor;
		this.lookAheadForSpeedOffset = this.ACAI.lookAheadForSpeedOffset;
		this.lookAheadForSpeedFactor = this.ACAI.lookAheadForSpeedFactor;
		this.pointToPointThreshold = this.ACAI.pointThreshold;
		this.progressStyle = (int)this.ACAI.progressStyle;
		if (this.target == null)
		{
			this.target = this.ACAI.carAItarget;
		}
		this.patrolCar = base.transform.parent.gameObject.GetComponent<PatrolCar>();
		this.Reset();
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0000868C File Offset: 0x0000688C
	public void Reset()
	{
		this.beat = Random.Range(1, 4);
		this.progressDistance = 0f;
		this.progressNum = 0;
		if (this.beat == 1)
		{
			this.circuit = this.ACAI.AIcircuit2;
		}
		else if (this.beat == 2)
		{
			this.circuit = this.ACAI.AIcircuit3;
		}
		else if (this.beat == 3)
		{
			this.circuit = this.ACAI.AIcircuit4;
		}
		else if (this.beat == 4)
		{
			this.circuit = this.ACAI.AIcircuitLost1;
		}
		else if (this.beat == 5)
		{
			this.circuit = this.ACAI.AIcircuitLost2;
		}
		else if (this.beat == 6)
		{
			this.circuit = this.ACAI.AIcircuitLost3;
		}
		if (this.progressStyle == 1)
		{
			this.target.position = this.circuit.nodes[this.progressNum].position;
			this.target.rotation = this.circuit.nodes[this.progressNum].rotation;
		}
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000087B8 File Offset: 0x000069B8
	public void Recalibrate()
	{
		this.progressDistance = 0f;
		if (this.beat == 1)
		{
			this.circuit = this.ACAI.AIcircuit2;
		}
		else if (this.beat == 2)
		{
			this.circuit = this.ACAI.AIcircuit3;
		}
		else if (this.beat == 3)
		{
			this.circuit = this.ACAI.AIcircuit4;
		}
		else if (this.beat == 4)
		{
			this.circuit = this.ACAI.AIcircuitLost1;
		}
		else if (this.beat == 5)
		{
			this.circuit = this.ACAI.AIcircuitLost2;
		}
		else if (this.beat == 6)
		{
			this.circuit = this.ACAI.AIcircuitLost3;
		}
		if (this.progressStyle == 1)
		{
			this.target.position = this.circuit.nodes[this.progressNum].position;
			this.target.rotation = this.circuit.nodes[this.progressNum].rotation;
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x000088D0 File Offset: 0x00006AD0
	private void Update()
	{
		if (!base.transform.GetComponent<AnyCarAI>().persuitAiOn)
		{
			base.transform.GetComponent<CarAIInputs>().persuitAiOn = false;
			this.FollowPath();
			return;
		}
		if (this.ACAI.persuitTarget != null)
		{
			Transform transform = this.ACAI.persuitTarget.transform;
			this.target = transform;
			return;
		}
		this.FollowPath();
	}

	// Token: 0x0600009B RID: 155 RVA: 0x0000893C File Offset: 0x00006B3C
	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, this.target.position);
			Gizmos.DrawWireSphere(this.circuit.GetRoutePosition(this.progressDistance), 1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.target.position, this.target.position + this.target.forward);
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000089C8 File Offset: 0x00006BC8
	public void FollowPath()
	{
		if (this.ACAI.progressStyle == ProgressStyle.SmoothAlongRoute)
		{
			if (Time.deltaTime > 0f)
			{
				this.speed = Mathf.Lerp(this.speed, (this.lastPosition - base.transform.position).magnitude / Time.deltaTime, Time.deltaTime);
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
		if (!this.carStopped)
		{
			if ((this.target.position - base.transform.position).magnitude < this.pointToPointThreshold)
			{
				this.progressNum = (this.progressNum + 1) % this.circuit.nodes.Count;
			}
			if (this.target.name != "FPSController")
			{
				this.target.position = this.circuit.nodes[this.progressNum].position;
				this.target.rotation = this.circuit.nodes[this.progressNum].rotation;
			}
			else
			{
				this.target = this.ACAI.carAItargetNonChar;
				this.target.position = this.circuit.nodes[this.progressNum].position;
				this.target.rotation = this.circuit.nodes[this.progressNum].rotation;
			}
			this.progressPoint = this.circuit.GetRoutePoint(this.progressDistance);
			Vector3 lhs2 = this.progressPoint.position - base.transform.position;
			if (Vector3.Dot(lhs2, this.progressPoint.direction) < 0f)
			{
				this.progressDistance += lhs2.magnitude;
			}
			this.lastPosition = base.transform.position;
			if (this.circuit.nodes[this.progressNum].gameObject.name == "station")
			{
				Debug.Log("station");
				this.patrolCar.GetOut();
				this.Reset();
			}
			if (this.circuit.nodes[this.progressNum].gameObject.name == "gasstation")
			{
				this.carStopped = true;
				this.progressNum = (this.progressNum + 1) % this.circuit.nodes.Count;
				this.patrolCar.GetOutGasStation();
				base.StartCoroutine(this.UnlockPosition());
			}
		}
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00008D4B File Offset: 0x00006F4B
	private IEnumerator UnlockPosition()
	{
		yield return new WaitForSeconds(2f);
		this.carStopped = false;
		yield break;
	}

	// Token: 0x04000199 RID: 409
	[HideInInspector]
	public Transform target;

	// Token: 0x0400019A RID: 410
	public WaypointsPath circuit;

	// Token: 0x0400019B RID: 411
	private WaypointsPath circuit2;

	// Token: 0x0400019C RID: 412
	private WaypointsPath circuit3;

	// Token: 0x0400019D RID: 413
	private WaypointsPath circuit4;

	// Token: 0x0400019E RID: 414
	private AnyCarAI ACAI;

	// Token: 0x0400019F RID: 415
	private float lookAheadForTargetOffset = 5f;

	// Token: 0x040001A0 RID: 416
	private float lookAheadForTargetFactor = 0.1f;

	// Token: 0x040001A1 RID: 417
	private float lookAheadForSpeedOffset = 10f;

	// Token: 0x040001A2 RID: 418
	private float lookAheadForSpeedFactor = 0.2f;

	// Token: 0x040001A3 RID: 419
	private int progressStyle;

	// Token: 0x040001A4 RID: 420
	private float pointToPointThreshold = 4f;

	// Token: 0x040001A5 RID: 421
	private float progressDistance;

	// Token: 0x040001A6 RID: 422
	public int progressNum;

	// Token: 0x040001A7 RID: 423
	private Vector3 lastPosition;

	// Token: 0x040001A8 RID: 424
	private float speed;

	// Token: 0x040001A9 RID: 425
	public PatrolCar patrolCar;

	// Token: 0x040001AA RID: 426
	public int beat;

	// Token: 0x040001AB RID: 427
	private bool carStopped;

	// Token: 0x040001AC RID: 428
	[HideInInspector]
	public Transform[] pathTransform;
}
