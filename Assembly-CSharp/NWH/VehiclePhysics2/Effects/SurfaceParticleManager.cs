using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002BB RID: 699
	[Serializable]
	public class SurfaceParticleManager : Effect
	{
		// Token: 0x06001296 RID: 4758 RVA: 0x000C82D4 File Offset: 0x000C64D4
		public override void Initialize()
		{
			foreach (WheelComponent wheelComponent in this.vc.Wheels)
			{
				SurfaceParticleSystem surfaceParticleSystem = new SurfaceParticleSystem();
				surfaceParticleSystem.Initialize(this.vc, wheelComponent);
				this.particleSystems.Add(surfaceParticleSystem);
			}
			this.initialized = true;
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x000C834C File Offset: 0x000C654C
		public override void Update()
		{
			this.particleCount = 0;
			if (!base.Active)
			{
				return;
			}
			foreach (SurfaceParticleSystem surfaceParticleSystem in this.particleSystems)
			{
				surfaceParticleSystem.longitudinalSlipCoeff = this.longitudinalSlipParticleCoeff;
				surfaceParticleSystem.lateralSlipCoeff = this.lateralSlipParticleCoeff;
				surfaceParticleSystem.particleSizeCoeff = this.particleSizeCoeff;
				surfaceParticleSystem.emissionRateCoeff = this.emissionRateCoeff;
				surfaceParticleSystem.Update();
				this.particleCount += surfaceParticleSystem.particleCount;
			}
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x000C83F4 File Offset: 0x000C65F4
		public override void Enable()
		{
			base.Enable();
			foreach (SurfaceParticleSystem surfaceParticleSystem in this.particleSystems)
			{
				surfaceParticleSystem.Enable();
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x000C844C File Offset: 0x000C664C
		public override void Disable()
		{
			base.Disable();
			foreach (SurfaceParticleSystem surfaceParticleSystem in this.particleSystems)
			{
				surfaceParticleSystem.Disable();
			}
		}

		// Token: 0x04002345 RID: 9029
		[Range(0f, 5f)]
		[Tooltip("How much will lateral slip contribute to the particle emission.\r\nIgnored when particle type for the surface is set to other than Smoke.")]
		public float lateralSlipParticleCoeff = 1f;

		// Token: 0x04002346 RID: 9030
		[Range(0f, 5f)]
		[Tooltip("How much will longitudinal slip contribute to the particle emission.\r\nIgnored when particle type for the surface is set to other than Smoke.")]
		public float longitudinalSlipParticleCoeff = 1f;

		// Token: 0x04002347 RID: 9031
		[Range(0f, 2f)]
		[Tooltip("Particle size multiplier specific to this vehicle.\r\nUse to adjust particle size on per-vehicle basis.\r\nFor global particle size adjustment for individual surfaces check SurfacePresets.")]
		public float particleSizeCoeff = 1f;

		// Token: 0x04002348 RID: 9032
		[Range(0f, 2f)]
		[Tooltip("Emission rate multiplier specific to this vehicle.\r\nUse to adjust emission on per-vehicle basis.\r\nFor global emission adjustment for individual surfaces check SurfacePresets.")]
		public float emissionRateCoeff = 1f;

		// Token: 0x04002349 RID: 9033
		[SerializeField]
		private List<SurfaceParticleSystem> particleSystems = new List<SurfaceParticleSystem>();

		// Token: 0x0400234A RID: 9034
		[Tooltip("Current particle count for all surface particle systems.")]
		public int particleCount;
	}
}
