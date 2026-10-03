using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace SMPScripts
{
	// Token: 0x020001BC RID: 444
	public class MotoStatus : MonoBehaviour
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x0008F3FF File Offset: 0x0008D5FF
		private void Start()
		{
			this.motoController = base.GetComponent<MotoController>();
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0008F41C File Offset: 0x0008D61C
		private void OnCollisionEnter(Collision collision)
		{
			if (collision.relativeVelocity.magnitude > 3f && this.fps.parent != null && LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
			{
				LogitechGSDK.LogiPlaySideCollisionForce(0, Mathf.RoundToInt(collision.relativeVelocity.magnitude));
			}
			if (collision.relativeVelocity.magnitude > this.impactThreshold && this.fps.parent != null && this.fps.parent.name == "SeatMount")
			{
				this.dislodged = true;
			}
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0008F4C8 File Offset: 0x0008D6C8
		private void Update()
		{
			if (this.dislodged != this.prevDislodged)
			{
				if (this.dislodged)
				{
					if (this.inactiveColliders != null)
					{
						this.motoController.motoGeometry.fPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
						this.motoController.motoGeometry.rPhysicsWheel.GetComponent<SphereCollider>().enabled = false;
						this.motoController.airTimeSettings.freestyle = false;
						this.motoController.enabled = false;
					}
					else
					{
						base.StartCoroutine(this.MotocontrollerToggle(false));
					}
					this.motoController.rb.centerOfMass = this.motoController.GetComponent<BoxCollider>().center;
					this.fps.GetComponent<Interactor>().GetOut();
					this.fpc.ReInitMouseLook();
					this.motoSound.suspension1.Play();
					this.motoController.airTimeSettings.freestyle = false;
				}
				else
				{
					if (this.inactiveColliders != null)
					{
						this.motoController.motoGeometry.fPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
						this.motoController.motoGeometry.rPhysicsWheel.GetComponent<SphereCollider>().enabled = true;
						this.inactiveColliders.SetActive(false);
						this.motoController.enabled = true;
					}
					else
					{
						base.StartCoroutine(this.MotocontrollerToggle(true));
					}
					this.motoController.rb.centerOfMass = this.motoController.centerOfMassOffset;
				}
			}
			this.prevDislodged = this.dislodged;
			this.prevOnBike = this.onBike;
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0008F662 File Offset: 0x0008D862
		private IEnumerator MotocontrollerToggle(bool toggle)
		{
			yield return new WaitForSeconds(0.25f);
			this.motoController.enabled = toggle;
			yield break;
		}

		// Token: 0x04001D1A RID: 7450
		public bool onBike = true;

		// Token: 0x04001D1B RID: 7451
		public bool dislodged;

		// Token: 0x04001D1C RID: 7452
		public float impactThreshold;

		// Token: 0x04001D1D RID: 7453
		public GameObject ragdollPrefab;

		// Token: 0x04001D1E RID: 7454
		[HideInInspector]
		public GameObject instantiatedRagdoll;

		// Token: 0x04001D1F RID: 7455
		private bool prevOnBike;

		// Token: 0x04001D20 RID: 7456
		private bool prevDislodged;

		// Token: 0x04001D21 RID: 7457
		public GameObject inactiveColliders;

		// Token: 0x04001D22 RID: 7458
		private MotoController motoController;

		// Token: 0x04001D23 RID: 7459
		private Rigidbody rb;

		// Token: 0x04001D24 RID: 7460
		public Transform fps;

		// Token: 0x04001D25 RID: 7461
		public MotoSound motoSound;

		// Token: 0x04001D26 RID: 7462
		public FirstPersonController fpc;

		// Token: 0x04001D27 RID: 7463
		public Transform fpcTrans;
	}
}
