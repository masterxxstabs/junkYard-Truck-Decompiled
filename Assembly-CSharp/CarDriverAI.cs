using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class CarDriverAI : MonoBehaviour
{
	// Token: 0x06000120 RID: 288 RVA: 0x0000E26E File Offset: 0x0000C46E
	public void Start()
	{
		this.targetPositionTranform = this.RaceTargets.GetChild(0);
		this.nextTarget = this.RaceTargets.GetChild(1);
		this.numTargets = this.RaceTargets.childCount;
		this.i = 0;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000E2AC File Offset: 0x0000C4AC
	private void FixedUpdate()
	{
		this.SetTargetPosition(this.targetPositionTranform.position);
		float forwardAmount = 0f;
		float turnAmount = 0f;
		float num = 10f;
		float num2 = Vector3.Distance(base.transform.position, this.targetPosition);
		if (num2 > num)
		{
			Vector3 normalized = (this.targetPosition - base.transform.position).normalized;
			this.dot = Vector3.Dot(base.transform.forward, normalized);
			if (this.dot > 0f)
			{
				forwardAmount = this.forwardAcc;
				float num3 = 30f;
				float num4 = 40f;
				if (num2 < num3 && this.carDriver.GetSpeed() > num4)
				{
					forwardAmount = -this.forwardAcc;
				}
			}
			else
			{
				float num5 = 25f;
				if (num2 > num5)
				{
					forwardAmount = this.forwardAcc;
				}
				else
				{
					forwardAmount = -this.forwardAcc;
				}
			}
			float num6 = Vector3.SignedAngle(base.transform.forward, normalized, Vector3.up);
			if (num6 > 0f)
			{
				turnAmount = 0.2f;
			}
			else
			{
				turnAmount = -0.2f;
			}
			if (num6 > 35f)
			{
				turnAmount = 0.3f;
			}
			if (num6 < -35f)
			{
				turnAmount = -0.3f;
			}
			if (num6 > 60f)
			{
				turnAmount = 0.4f;
			}
			if (num6 < -60f)
			{
				turnAmount = -0.4f;
			}
		}
		else
		{
			this.i++;
			if (this.i < this.numTargets)
			{
				this.targetPositionTranform = this.RaceTargets.GetChild(this.i);
				this.nextTarget = this.RaceTargets.GetChild(this.i + 1);
			}
			else
			{
				if (this.carDriver.GetSpeed() > 15f)
				{
					forwardAmount = -this.forwardAcc;
				}
				else
				{
					forwardAmount = 0f;
				}
				turnAmount = 0f;
			}
		}
		this.carDriver.SetInputs(forwardAmount, turnAmount);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000E479 File Offset: 0x0000C679
	public void SetTargetPosition(Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
	}

	// Token: 0x0400033C RID: 828
	[SerializeField]
	public Transform targetPositionTranform;

	// Token: 0x0400033D RID: 829
	public Transform RaceTargets;

	// Token: 0x0400033E RID: 830
	public CarDriver carDriver;

	// Token: 0x0400033F RID: 831
	private Vector3 targetPosition;

	// Token: 0x04000340 RID: 832
	private int i;

	// Token: 0x04000341 RID: 833
	private int numTargets;

	// Token: 0x04000342 RID: 834
	public Transform nextTarget;

	// Token: 0x04000343 RID: 835
	private bool slowDown;

	// Token: 0x04000344 RID: 836
	public float forwardAcc;

	// Token: 0x04000345 RID: 837
	public float dot;
}
