using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x02000337 RID: 823
	[ExecuteAlways]
	public class VehicleEditorAI : MonoBehaviour
	{
		// Token: 0x0600150F RID: 5391 RVA: 0x000DE9C0 File Offset: 0x000DCBC0
		private void Update()
		{
			base.transform.GetComponent<AiCarContrtoller>().maxRayLength = -this.wheelYPosition + (this.DeltaRayLength + this.wheelRadious);
			Transform wheels = base.transform.GetComponent<ReferencesAI>().wheels;
			Transform wheelFL = base.transform.GetComponent<ReferencesAI>().wheelFL;
			Transform wheelFR = base.transform.GetComponent<ReferencesAI>().wheelFR;
			Transform wheelRL = base.transform.GetComponent<ReferencesAI>().wheelRL;
			Transform wheelRR = base.transform.GetComponent<ReferencesAI>().wheelRR;
			wheels.localPosition = new Vector3(0f, this.wheelYPosition, 0f);
			wheelFL.localPosition = new Vector3(-this.GapBetweenWheels, 0f, this.FrontWheelsZPosition);
			wheelFR.localPosition = new Vector3(this.GapBetweenWheels, 0f, this.FrontWheelsZPosition);
			wheelRL.localPosition = new Vector3(-this.GapBetweenWheels, 0f, this.RearWheelsZPosition);
			wheelRR.localPosition = new Vector3(this.GapBetweenWheels, 0f, this.RearWheelsZPosition);
			wheelFL.GetComponent<SphereCollider>().radius = this.wheelRadious;
			wheelFR.GetComponent<SphereCollider>().radius = this.wheelRadious;
			wheelRL.GetComponent<SphereCollider>().radius = this.wheelRadious;
			wheelRR.GetComponent<SphereCollider>().radius = this.wheelRadious;
			JointDrive yDrive = wheelFL.GetComponent<ConfigurableJoint>().yDrive;
			yDrive.positionDamper = this.Damper;
			yDrive.positionSpring = this.SuspentionForce;
			wheelFL.GetComponent<ConfigurableJoint>().yDrive = yDrive;
			wheelFR.GetComponent<ConfigurableJoint>().yDrive = yDrive;
			wheelRL.GetComponent<ConfigurableJoint>().yDrive = yDrive;
			wheelRR.GetComponent<ConfigurableJoint>().yDrive = yDrive;
		}

		// Token: 0x040025B3 RID: 9651
		[Header("Vehicle Stats")]
		[Range(0f, 10f)]
		public float wheelRadious = 0.34f;

		// Token: 0x040025B4 RID: 9652
		[Range(-10f, 0f)]
		public float wheelYPosition = -0.5f;

		// Token: 0x040025B5 RID: 9653
		[Range(0f, 10f)]
		public float FrontWheelsZPosition = 1.5f;

		// Token: 0x040025B6 RID: 9654
		[Range(-10f, 0f)]
		public float RearWheelsZPosition = -1.5f;

		// Token: 0x040025B7 RID: 9655
		[Range(0f, 10f)]
		public float GapBetweenWheels = 1f;

		// Token: 0x040025B8 RID: 9656
		public float SuspentionForce = 10000f;

		// Token: 0x040025B9 RID: 9657
		public float Damper = 100f;

		// Token: 0x040025BA RID: 9658
		public float DeltaRayLength = 0.1f;
	}
}
