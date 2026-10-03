using System;
using UnityEngine;

namespace SMPScripts
{
	// Token: 0x020001BD RID: 445
	public class MotoSuspensionManager : MonoBehaviour
	{
		// Token: 0x06000AC5 RID: 2757 RVA: 0x0008F688 File Offset: 0x0008D888
		private void Start()
		{
			this.motoController = Object.FindObjectOfType<MotoController>();
			if (this.enable)
			{
				if (this.fSuspension != null)
				{
					this.fSuspension.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Limited;
					this.fLimit.limit = 0.01f;
					this.fSuspension.GetComponent<ConfigurableJoint>().linearLimit = this.fLimit;
					this.fSpring.spring = 10000f;
					this.fSpring.damper = 500f;
					this.fSuspension.GetComponent<ConfigurableJoint>().linearLimitSpring = this.fSpring;
					this.fDrive.positionSpring = this.frontSpring;
					this.fDrive.positionDamper = this.frontDamper;
					this.fDrive.maximumForce = float.PositiveInfinity;
					this.fSuspension.GetComponent<ConfigurableJoint>().yDrive = this.fDrive;
				}
				if (this.rSuspension != null)
				{
					this.rSuspension.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Free;
					this.rDrive.positionSpring = this.rearSpring;
					this.rDrive.positionDamper = this.rearDamper;
					this.rDrive.maximumForce = float.PositiveInfinity;
					this.rSuspension.GetComponent<ConfigurableJoint>().angularXDrive = this.rDrive;
				}
			}
			else
			{
				if (this.fSuspension != null)
				{
					this.fSuspension.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Locked;
				}
				if (this.rSuspension != null)
				{
					this.rSuspension.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
				}
			}
			if (this.spring != null)
			{
				this.initialSpringScale = this.spring.transform.localScale;
			}
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0008F840 File Offset: 0x0008DA40
		private void Update()
		{
			if (this.enable)
			{
				if (this.frontSuspensionMesh != null)
				{
					this.frontSuspensionMesh.transform.rotation = this.motoController.motoGeometry.handles.transform.rotation;
				}
				if (this.spring != null && this.rSuspension != null && this.rSuspension.transform.eulerAngles.x > 0f && this.rSuspension.transform.eulerAngles.x < 20f)
				{
					this.spring.transform.localScale = new Vector3(this.initialSpringScale.x, this.initialSpringScale.y - Mathf.Clamp01(this.rSuspension.transform.eulerAngles.x / 20f), this.initialSpringScale.z);
				}
			}
		}

		// Token: 0x04001D28 RID: 7464
		public bool enable;

		// Token: 0x04001D29 RID: 7465
		[Header("Joint Data")]
		public GameObject fSuspension;

		// Token: 0x04001D2A RID: 7466
		public GameObject rSuspension;

		// Token: 0x04001D2B RID: 7467
		[Header("Settings")]
		public float frontSpring;

		// Token: 0x04001D2C RID: 7468
		public float frontDamper;

		// Token: 0x04001D2D RID: 7469
		public float rearSpring;

		// Token: 0x04001D2E RID: 7470
		public float rearDamper;

		// Token: 0x04001D2F RID: 7471
		[Header("[Optional] Misc Transform Corrections")]
		public GameObject frontSuspensionMesh;

		// Token: 0x04001D30 RID: 7472
		public GameObject spring;

		// Token: 0x04001D31 RID: 7473
		private MotoController motoController;

		// Token: 0x04001D32 RID: 7474
		private JointDrive fDrive;

		// Token: 0x04001D33 RID: 7475
		private JointDrive rDrive;

		// Token: 0x04001D34 RID: 7476
		private SoftJointLimit fLimit;

		// Token: 0x04001D35 RID: 7477
		private SoftJointLimitSpring fSpring;

		// Token: 0x04001D36 RID: 7478
		private Vector3 initialSpringScale;
	}
}
