using System;
using System.Collections;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000334 RID: 820
	public class ReverseWhenStuck : MonoBehaviour
	{
		// Token: 0x06001500 RID: 5376 RVA: 0x000DE450 File Offset: 0x000DC650
		private void Start()
		{
			this.AiCarContrtoller = base.GetComponent<AiCarContrtoller>();
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x000DE46C File Offset: 0x000DC66C
		private void FixedUpdate()
		{
			this.velocity = this.rb.velocity.magnitude;
			if (this.velocity < this.minVelocityToReverse)
			{
				this.timeToStartReverse += Time.fixedDeltaTime;
			}
			else
			{
				this.timeToStartReverse = 0f;
			}
			if (this.timeToStartReverse > this.reverseStartTime)
			{
				base.StartCoroutine(this.startReverse());
				this.timeToStartReverse = 0f;
			}
			if (this.TakingReverse)
			{
				this.TakeReverse();
			}
			if (this.reverseInitTime > 0f)
			{
				if (this.totalReverseCount > 12)
				{
					this.totalReverseCount = 0;
					this.reverseCount = 0;
					this.reverseInitTime = 0f;
					this.TakingReverse = false;
					this.wpt.RespawnAtStation();
				}
				else if (this.reverseCount > 2)
				{
					this.reverseCount = 0;
					this.reverseInitTime = 0f;
					this.TakingReverse = false;
					this.wpt.RespawnOnRoad();
				}
				if (this.reverseCount > 0 && Time.time - this.reverseInitTime > 15f)
				{
					this.reverseCount = 0;
					this.reverseInitTime = 0f;
				}
			}
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x000DE598 File Offset: 0x000DC798
		public void TakeReverse()
		{
			this.AiCarContrtoller.carVelocity = base.transform.InverseTransformDirection(this.rb.velocity);
			this.AiCarContrtoller.tireVisuals();
			this.speedValue = 600f * Time.fixedDeltaTime * 1000f * this.AiCarContrtoller.ReverseCurve.Evaluate(Mathf.Abs(this.AiCarContrtoller.carVelocity.z) / 100f);
			this.rb.AddForceAtPosition(-this.groundCheck.forward * this.speedValue, this.groundCheck.position);
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x000DE645 File Offset: 0x000DC845
		private IEnumerator startReverse()
		{
			this.reverseCount++;
			if (this.officer.playerCuffed)
			{
				this.totalReverseCount++;
			}
			this.reverseInitTime = Time.time;
			if (this.reverseCount < 4)
			{
				if (this.reverseCount == 1)
				{
					this.reverseTime = 1f;
				}
				else if (this.reverseCount == 2)
				{
					this.reverseTime = 2f;
				}
				else
				{
					this.reverseTime = 3f;
				}
				float timePassed = 0f;
				while (timePassed < this.reverseTime)
				{
					this.TakingReverse = true;
					this.AiCarContrtoller.enabled = false;
					timePassed += Time.deltaTime;
					yield return null;
				}
			}
			this.TakingReverse = false;
			this.AiCarContrtoller.enabled = true;
			this.timeToStartReverse = 0f;
			yield break;
		}

		// Token: 0x04002598 RID: 9624
		private AiCarContrtoller AiCarContrtoller;

		// Token: 0x04002599 RID: 9625
		private Rigidbody rb;

		// Token: 0x0400259A RID: 9626
		public float minVelocityToReverse;

		// Token: 0x0400259B RID: 9627
		public Transform groundCheck;

		// Token: 0x0400259C RID: 9628
		private float speedValue;

		// Token: 0x0400259D RID: 9629
		public float reverseTime;

		// Token: 0x0400259E RID: 9630
		private float velocity;

		// Token: 0x0400259F RID: 9631
		private float timeToStartReverse;

		// Token: 0x040025A0 RID: 9632
		private float reverseStartTime = 3f;

		// Token: 0x040025A1 RID: 9633
		private int reverseCount;

		// Token: 0x040025A2 RID: 9634
		private float reverseInitTime;

		// Token: 0x040025A3 RID: 9635
		public PatrolCar patrol;

		// Token: 0x040025A4 RID: 9636
		public WaypointProgressTracker wpt;

		// Token: 0x040025A5 RID: 9637
		public int totalReverseCount;

		// Token: 0x040025A6 RID: 9638
		public Officer officer;

		// Token: 0x040025A7 RID: 9639
		private bool TakingReverse;
	}
}
