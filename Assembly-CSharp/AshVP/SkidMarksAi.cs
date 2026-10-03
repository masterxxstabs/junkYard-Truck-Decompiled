using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000336 RID: 822
	public class SkidMarksAi : MonoBehaviour
	{
		// Token: 0x0600150A RID: 5386 RVA: 0x000DE894 File Offset: 0x000DCA94
		private void Awake()
		{
			this.smoke = base.GetComponent<ParticleSystem>();
			this.skidMark = base.GetComponent<TrailRenderer>();
			this.skidMark.emitting = false;
			base.transform.localPosition = new Vector3(0f, -base.transform.parent.parent.GetComponent<SphereCollider>().radius + 0.03f, 0f);
			this.skidMark.startWidth = this.carController.skidWidth;
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x000DE916 File Offset: 0x000DCB16
		private void OnEnable()
		{
			this.skidMark.enabled = true;
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x000DE924 File Offset: 0x000DCB24
		private void OnDisable()
		{
			this.skidMark.enabled = false;
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x000DE934 File Offset: 0x000DCB34
		private void Update()
		{
			Vector3 carVelocity = this.carController.carVelocity;
			if (this.carController.grounded)
			{
				if (Mathf.Abs(carVelocity.x) > this.carController.SkidEnable)
				{
					this.skidMark.emitting = true;
				}
				else
				{
					this.skidMark.emitting = false;
				}
			}
			else
			{
				this.skidMark.emitting = false;
			}
			if (this.skidMark.emitting)
			{
				this.smoke.Play();
				return;
			}
			this.smoke.Stop();
		}

		// Token: 0x040025B0 RID: 9648
		private TrailRenderer skidMark;

		// Token: 0x040025B1 RID: 9649
		private ParticleSystem smoke;

		// Token: 0x040025B2 RID: 9650
		public AiCarContrtoller carController;
	}
}
