using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B1 RID: 689
	[Serializable]
	public class EffectManager : VehicleComponent
	{
		// Token: 0x06001255 RID: 4693 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x000C5A6C File Offset: 0x000C3C6C
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
			this.GetComponents(ref this.components);
			foreach (VehicleComponent vehicleComponent in this.components)
			{
				vehicleComponent.Awake(vc);
			}
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x000C5AD0 File Offset: 0x000C3CD0
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			foreach (VehicleComponent vehicleComponent in this.components)
			{
				vehicleComponent.Update();
			}
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x000C5B2C File Offset: 0x000C3D2C
		public override void OnDrawGizmosSelected(VehicleController vc)
		{
			base.OnDrawGizmosSelected(vc);
			if (this.components == null || this.components.Count == 0)
			{
				this.GetComponents(ref this.components);
			}
			foreach (VehicleComponent vehicleComponent in this.components)
			{
				vehicleComponent.OnDrawGizmosSelected(vc);
			}
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x000C5BA8 File Offset: 0x000C3DA8
		public override void CheckState(int lodIndex)
		{
			base.CheckState(lodIndex);
			foreach (VehicleComponent vehicleComponent in this.components)
			{
				vehicleComponent.CheckState(lodIndex);
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x000C5C00 File Offset: 0x000C3E00
		public override void SetDefaults(VehicleController vc)
		{
			base.SetDefaults(vc);
			this.GetComponents(ref this.components);
			foreach (VehicleComponent vehicleComponent in this.components)
			{
				vehicleComponent.SetDefaults(vc);
			}
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x000C5C64 File Offset: 0x000C3E64
		private void GetComponents(ref List<VehicleComponent> components)
		{
			components = new List<VehicleComponent>
			{
				this.exhaustFlash,
				this.exhaustSmoke,
				this.lightsManager,
				this.skidmarkManager,
				this.surfaceParticleManager
			};
		}

		// Token: 0x040022CA RID: 8906
		[Tooltip("    All effects are placed in this list after initialization.")]
		public List<VehicleComponent> components = new List<VehicleComponent>();

		// Token: 0x040022CB RID: 8907
		public ExhaustFlash exhaustFlash = new ExhaustFlash();

		// Token: 0x040022CC RID: 8908
		public ExhaustSmoke exhaustSmoke = new ExhaustSmoke();

		// Token: 0x040022CD RID: 8909
		[FormerlySerializedAs("lights")]
		public LightsMananger lightsManager = new LightsMananger();

		// Token: 0x040022CE RID: 8910
		[FormerlySerializedAs("skidmarks")]
		public SkidmarkManager skidmarkManager = new SkidmarkManager();

		// Token: 0x040022CF RID: 8911
		public SurfaceParticleManager surfaceParticleManager = new SurfaceParticleManager();
	}
}
