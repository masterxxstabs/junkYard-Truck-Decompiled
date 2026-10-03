using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class PoliceDriverAI : MonoBehaviour
{
	// Token: 0x06000777 RID: 1911 RVA: 0x000618D9 File Offset: 0x0005FAD9
	public void Start()
	{
		this.currentBeat = 1;
		this.numTargets = this.BeatTargets1.childCount;
		this.targetPositionTranform = this.BeatTargets1.GetChild(0);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00061908 File Offset: 0x0005FB08
	public void NewBeat()
	{
		this.checkpoints = 0;
		this.currentBeat = 1;
		this.numTargets = this.BeatTargets1.childCount;
		this.targetPositionTranform = this.BeatTargets1.GetChild(0);
		this.nextTarget = null;
		this.carDriver.aSources[0].enabled = true;
		this.carDriver.aSources[1].enabled = true;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00061974 File Offset: 0x0005FB74
	private Transform ClosestWaypoint()
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		Transform transform = this.BeatTargets1;
		if (this.currentBeat == 2)
		{
			transform = this.BeatTargets1b;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			float num2 = Vector3.Distance(base.transform.position, transform2.position);
			if (num2 < num)
			{
				num = num2;
				result = transform2;
			}
		}
		return result;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00061A0C File Offset: 0x0005FC0C
	public void ResetPosition()
	{
		Transform transform = this.ClosestWaypoint();
		base.transform.position = transform.position;
		this.targetPositionTranform = transform;
		this.numTargets = transform.GetSiblingIndex();
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00061A44 File Offset: 0x0005FC44
	public void ResumeBeat()
	{
		Transform transform = this.ClosestWaypoint();
		this.targetPositionTranform = transform;
		this.numTargets = transform.parent.childCount;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00061A70 File Offset: 0x0005FC70
	private void FixedUpdate()
	{
		if (this.chasingPlayer)
		{
			this.targetPositionTranform = this.fps;
		}
		this.playerDistance = Vector3.Distance(base.transform.position, this.targetPositionTranform.position);
		this.SetTargetPosition(this.targetPositionTranform.position);
		this.CheckIfStuck();
		Vector3 vector = this.targetPosition;
		float forwardAmount = 0f;
		float num = 0f;
		float num2 = 4f;
		float num3 = Vector3.Distance(base.transform.position, this.targetPosition);
		if (num3 > num2)
		{
			Vector3 normalized = (this.targetPosition - base.transform.position).normalized;
			this.dot = Vector3.Dot(base.transform.forward, normalized);
			if (this.dot > 0f)
			{
				this.reversing = false;
				forwardAmount = this.forwardAcc;
				float num4 = 30f;
				float num5 = 40f;
				if (num3 < num4 && this.carDriver.GetSpeed() > num5)
				{
					forwardAmount = -this.forwardAcc;
				}
			}
			else if ((double)this.dot < -0.4)
			{
				this.reversing = true;
				forwardAmount = 0f;
			}
			float num6 = Vector3.SignedAngle(base.transform.forward, normalized, Vector3.up);
			if (num6 > 0f)
			{
				num = 0.2f;
			}
			else
			{
				num = -0.2f;
			}
			if (num6 > 35f)
			{
				num = 0.3f;
			}
			if (num6 < -35f)
			{
				num = -0.3f;
			}
			if (num6 > 60f)
			{
				num = 0.4f;
			}
			if (num6 < -60f)
			{
				num = -0.4f;
			}
			if (this.reversing)
			{
				num *= -1f;
			}
		}
		else
		{
			if (this.playerDistance < 10f && this.chasingPlayer)
			{
				if (this.carDriver.GetSpeed() > 15f)
				{
					forwardAmount = -this.forwardAcc;
				}
				else
				{
					forwardAmount = 0f;
				}
				num = 0f;
			}
			if (this.chasingPlayer && this.carDriver.GetSpeed() < 2f)
			{
				this.officerScript.chasing = true;
				this.TurnOff();
			}
		}
		if (!this.chasingPlayer)
		{
			num3 = Vector3.Distance(base.transform.position, this.targetPositionTranform.position);
			if (num3 < 10f)
			{
				if (this.checkpoints < this.numTargets - 1)
				{
					this.checkpoints++;
					if (this.currentBeat == 1)
					{
						this.targetPositionTranform = this.BeatTargets1.GetChild(this.checkpoints);
						this.nextTarget = this.BeatTargets1.GetChild(this.checkpoints);
					}
					else if (this.currentBeat == 2)
					{
						this.targetPositionTranform = this.BeatTargets1b.GetChild(this.checkpoints);
						this.nextTarget = this.BeatTargets1b.GetChild(this.checkpoints);
					}
				}
				else
				{
					forwardAmount = 0f;
				}
			}
			if (this.nextTarget != null && this.nextTarget.name == "station" && this.officerScript.playerCuffed)
			{
				this.TurnOff();
			}
			if (this.checkpoints >= this.numTargets - 1)
			{
				forwardAmount = 0f;
				if (!this.stopping)
				{
					this.stopping = true;
					base.StartCoroutine(this.Standby());
				}
			}
		}
		this.carDriver.SetInputs(forwardAmount, num);
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00061DC0 File Offset: 0x0005FFC0
	private void TurnOff()
	{
		this.chasingPlayer = false;
		this.officerScript.enabled = true;
		this.officerScript.OfficerExitCoroutine();
		this.carDriver.aSources[0].enabled = false;
		this.carDriver.aSources[1].enabled = false;
		base.GetComponent<Rigidbody>().drag = 1000f;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00061E21 File Offset: 0x00060021
	private IEnumerator RecordPosition()
	{
		while (this.chasingPlayer)
		{
			Vector3 position = this.fps.position;
			this.positionHistory.Add(position);
			if (this.positionHistory.Count > 5)
			{
				this.positionHistory.RemoveAt(0);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00061E30 File Offset: 0x00060030
	private IEnumerator Standby()
	{
		Debug.Log("standby");
		this.carDriver.aSources[0].enabled = false;
		this.carDriver.aSources[1].enabled = false;
		yield return new WaitForSeconds(3f);
		yield return new WaitForSeconds(8f);
		this.NewBeat();
		yield break;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00061E40 File Offset: 0x00060040
	private void CheckIfStuck()
	{
		if (Vector3.Distance(base.transform.position, this.lastPosition) < 0.1f)
		{
			this.stuckTimer += Time.fixedDeltaTime;
			if (this.stuckTimer > this.stuckCheckInterval)
			{
			}
		}
		else
		{
			this.stuckTimer = 0f;
		}
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00061EA8 File Offset: 0x000600A8
	public void SetTargetPosition(Vector3 tpp)
	{
		this.targetPosition = tpp;
	}

	// Token: 0x04001113 RID: 4371
	public PoliceDriver carDriver;

	// Token: 0x04001114 RID: 4372
	private int i;

	// Token: 0x04001115 RID: 4373
	private bool slowDown;

	// Token: 0x04001116 RID: 4374
	public float forwardAcc;

	// Token: 0x04001117 RID: 4375
	public float dot;

	// Token: 0x04001118 RID: 4376
	public Transform fps;

	// Token: 0x04001119 RID: 4377
	private float playerDistance;

	// Token: 0x0400111A RID: 4378
	public Transform targetPositionTranform;

	// Token: 0x0400111B RID: 4379
	private Vector3 targetPosition;

	// Token: 0x0400111C RID: 4380
	public int numTargets;

	// Token: 0x0400111D RID: 4381
	public Transform BeatTargets1;

	// Token: 0x0400111E RID: 4382
	public Transform BeatTargets1b;

	// Token: 0x0400111F RID: 4383
	public Transform nextTarget;

	// Token: 0x04001120 RID: 4384
	public int checkpoints;

	// Token: 0x04001121 RID: 4385
	private bool reversing;

	// Token: 0x04001122 RID: 4386
	private bool stopping;

	// Token: 0x04001123 RID: 4387
	public int currentBeat;

	// Token: 0x04001124 RID: 4388
	public bool chasingPlayer;

	// Token: 0x04001125 RID: 4389
	public GameObject officer;

	// Token: 0x04001126 RID: 4390
	public Officer officerScript;

	// Token: 0x04001127 RID: 4391
	private Vector3 lastPosition;

	// Token: 0x04001128 RID: 4392
	private float stuckTimer;

	// Token: 0x04001129 RID: 4393
	public float stuckCheckInterval = 5f;

	// Token: 0x0400112A RID: 4394
	public Transform wpOverride;

	// Token: 0x0400112B RID: 4395
	private List<Vector3> positionHistory = new List<Vector3>();
}
