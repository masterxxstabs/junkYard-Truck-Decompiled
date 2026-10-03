using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
	// Token: 0x02000285 RID: 645
	[Serializable]
	public class ModuleManager : VehicleComponent
	{
		// Token: 0x06001172 RID: 4466 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x000C1C2C File Offset: 0x000BFE2C
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
			this.ReloadModulesList();
			foreach (VehicleModule vehicleModule in this.modules)
			{
				vehicleModule.Awake(vc);
			}
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x000C1C8C File Offset: 0x000BFE8C
		public override void FixedUpdate()
		{
			foreach (VehicleModule vehicleModule in this.modules)
			{
				vehicleModule.FixedUpdate();
			}
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x000C1CDC File Offset: 0x000BFEDC
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			foreach (VehicleModule vehicleModule in this.modules)
			{
				vehicleModule.Update();
			}
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x000C1D38 File Offset: 0x000BFF38
		public override void OnDrawGizmosSelected(VehicleController vc)
		{
			foreach (VehicleModule vehicleModule in vc.moduleManager.modules)
			{
				vehicleModule.OnDrawGizmosSelected(vc);
			}
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x000C1D90 File Offset: 0x000BFF90
		public TM AddModule<TW, TM>() where TW : ModuleWrapper where TM : VehicleModule
		{
			if (this.vc == null)
			{
				return default(TM);
			}
			VehicleModule module = this.vc.gameObject.AddComponent<TW>().GetModule();
			this.modules.Add(module);
			return module as TM;
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x000C1DE8 File Offset: 0x000BFFE8
		public TM GetModule<TM>() where TM : VehicleModule
		{
			if (this.vc == null)
			{
				return default(TM);
			}
			return this.modules.FirstOrDefault((VehicleModule m) => m.GetType() == typeof(TM)) as TM;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x000C1E44 File Offset: 0x000C0044
		public override void CheckState(int lodIndex)
		{
			foreach (VehicleModule vehicleModule in this.modules)
			{
				vehicleModule.CheckState(lodIndex);
			}
			base.CheckState(lodIndex);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x000C1E9C File Offset: 0x000C009C
		public void RemoveModule<TW>() where TW : ModuleWrapper
		{
			if (this.vc == null)
			{
				return;
			}
			ModuleWrapper moduleWrapper = this.vc.gameObject.GetComponent<TW>();
			this.modules.Remove(moduleWrapper.GetModule());
			if (Application.isPlaying)
			{
				Object.Destroy(moduleWrapper);
				return;
			}
			Object.DestroyImmediate(moduleWrapper);
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x000C1EF4 File Offset: 0x000C00F4
		public void ReloadModulesList()
		{
			this.modules.Clear();
			foreach (ModuleWrapper moduleWrapper in this.vc.GetComponents<ModuleWrapper>().ToList<ModuleWrapper>())
			{
				this.modules.Add(moduleWrapper.GetModule());
				if (Application.isPlaying)
				{
					moduleWrapper.GetModule().Awake(this.vc);
				}
			}
		}

		// Token: 0x040021C4 RID: 8644
		[Tooltip("    Vehicle modules. Only modules in this list will get updated.")]
		public List<VehicleModule> modules = new List<VehicleModule>();
	}
}
