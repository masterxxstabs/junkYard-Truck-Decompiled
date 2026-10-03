using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Tests
{
	// Token: 0x02000266 RID: 614
	[RequireComponent(typeof(VehicleController))]
	public class VehicleControllerTest : MonoBehaviour
	{
		// Token: 0x06001029 RID: 4137 RVA: 0x000BA407 File Offset: 0x000B8607
		private void Awake()
		{
			this.vehicleController = base.GetComponent<VehicleController>();
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x000BA418 File Offset: 0x000B8618
		private void RandomlyEnableDisableComponent()
		{
			int index = Random.Range(0, this.components.Count);
			bool flag = Random.Range(0f, 1f) > 0.5f;
			VehicleComponent vehicleComponent = this.components[index];
			if (flag)
			{
				Debug.Log("Enable " + vehicleComponent.GetType().Name);
				this.components[index].Enable();
				return;
			}
			Debug.Log("Disable " + vehicleComponent.GetType().Name);
			this.components[index].Disable();
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x000BA4B4 File Offset: 0x000B86B4
		public void RunStateTest()
		{
			this.components = new List<VehicleComponent>();
			this.components.Add(this.vehicleController.steering);
			this.components.Add(this.vehicleController.powertrain);
			this.components.Add(this.vehicleController.damageHandler);
			this.components.Add(this.vehicleController.brakes);
			this.components.Add(this.vehicleController.groundDetection);
			this.components.Add(this.vehicleController.moduleManager);
			this.components.Add(this.vehicleController.effectsManager);
			this.components.AddRange(this.vehicleController.effectsManager.components);
			this.components.Add(this.vehicleController.soundManager);
			this.components.AddRange(this.vehicleController.soundManager.components);
			base.InvokeRepeating("RandomlyEnableDisableComponent", 0.1f, 0.02f);
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x000BA5C7 File Offset: 0x000B87C7
		public void RunTests()
		{
			this.RunStateTest();
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x000BA5CF File Offset: 0x000B87CF
		public void StopTests()
		{
			base.CancelInvoke("RandomlyEnableDisableComponent");
		}

		// Token: 0x040020AC RID: 8364
		public VehicleController vehicleController;

		// Token: 0x040020AD RID: 8365
		private List<VehicleComponent> components = new List<VehicleComponent>();
	}
}
