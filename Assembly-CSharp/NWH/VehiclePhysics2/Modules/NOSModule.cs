using System;
using System.Linq;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Modules.NOS;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
	// Token: 0x02000288 RID: 648
	[Serializable]
	public class NOSModule : VehicleModule
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x000C1FA8 File Offset: 0x000C01A8
		public override void Initialize()
		{
			this.soundComponent.nosModule = this;
			this.soundComponent.Awake(this.vc);
			this.soundComponent.container = this.vc.soundManager.engineSourceGO;
			this.soundComponent.audioMixerGroup = this.vc.soundManager.engineMixerGroup;
			this.soundComponent.Initialize();
			this.vc.soundManager.components.Add(this.soundComponent);
			this.initialEngineVolumeRange = this.vc.soundManager.engineRunningComponent.volumeRange;
			this.wasActive = this.active;
			this.initialized = true;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000C205C File Offset: 0x000C025C
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			this.wasActive = this.active;
			this.active = this.vc.input.states.boost;
			if (!this.active && this.wasActive)
			{
				this.vc.soundManager.engineRunningComponent.volumeRange = this.initialEngineVolumeRange;
			}
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x000C20C4 File Offset: 0x000C02C4
		public override void Enable()
		{
			base.Enable();
			if (this.vc != null && this.vc.powertrain.engine.powerModifiers.All((EngineComponent.PowerModifier p) => p != new EngineComponent.PowerModifier(this.NOSPowerModifier)))
			{
				this.vc.powertrain.engine.powerModifiers.Add(new EngineComponent.PowerModifier(this.NOSPowerModifier));
			}
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x000C2134 File Offset: 0x000C0334
		public override void Disable()
		{
			base.Disable();
			this.active = false;
			if (this.vc != null)
			{
				this.vc.powertrain.engine.powerModifiers.RemoveAll((EngineComponent.PowerModifier p) => p == new EngineComponent.PowerModifier(this.NOSPowerModifier));
			}
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00092832 File Offset: 0x00090A32
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Powertrain;
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x000C2184 File Offset: 0x000C0384
		public float NOSPowerModifier()
		{
			if (!this.active || !base.Active || (this.vc.powertrain.transmission.Ratio <= 0f && this.disableInReverse) || (this.vc.powertrain.engine.throttlePosition < 0.1f && this.disableOffThrottle))
			{
				this.active = false;
				return 1f;
			}
			this.charge -= this.flow * this.vc.fixedDeltaTime;
			this.charge = ((this.charge < 0f) ? 0f : ((this.charge > this.capacity) ? this.capacity : this.charge));
			if (this.charge <= 0f)
			{
				return 1f;
			}
			if (this.vc.effectsManager.exhaustFlash.Active)
			{
				this.vc.effectsManager.exhaustFlash.flash = true;
			}
			if (this.vc.soundManager.Active && this.vc.soundManager.engineRunningComponent.Active)
			{
				this.vc.soundManager.engineRunningComponent.volumeRange = this.initialEngineVolumeRange * this.engineVolumeCoefficient;
			}
			return this.powerCoefficient;
		}

		// Token: 0x040021C5 RID: 8645
		[Tooltip("    Is NOS currently active? Not to be confused with enabled.")]
		public bool active;

		// Token: 0x040021C6 RID: 8646
		[ShowInSettings("Capacity", 0f, 5f, 0.5f)]
		[Tooltip("    Capacity of NOS bottle.")]
		public float capacity = 2f;

		// Token: 0x040021C7 RID: 8647
		[ShowInSettings("Charge", 0f, 5f, 0.5f)]
		[Tooltip("    Current charge of NOS bottle.")]
		public float charge = 2f;

		// Token: 0x040021C8 RID: 8648
		[Tooltip("    Can NOS be used while in reverse?")]
		public bool disableInReverse = true;

		// Token: 0x040021C9 RID: 8649
		[Tooltip("    Can NOS be used while there is no throttle input / engine is idling?")]
		public bool disableOffThrottle = true;

		// Token: 0x040021CA RID: 8650
		[Range(1f, 3f)]
		[Tooltip("Makes engine sound louder while NOS is active.\r\nVolume range of the engine running sound component will get multiplied by this value.")]
		public float engineVolumeCoefficient = 1.5f;

		// Token: 0x040021CB RID: 8651
		[Range(1f, 3f)]
		[Tooltip("    Value that will be used as base intensity of Exhaust Smoke effect while NOS is active.")]
		public float exhaustEmissionCoefficient = 2f;

		// Token: 0x040021CC RID: 8652
		[ShowInSettings("Flow", 0f, 2f, 0.1f)]
		[Tooltip("    Maximum flow of NOS in kg/s.")]
		public float flow = 0.1f;

		// Token: 0x040021CD RID: 8653
		[Range(1f, 5f)]
		[ShowInSettings("Power Coeff.", 1f, 4f, 0.5f)]
		[Tooltip("Power of the engine will be multiplied by this value when NOS is active to get the final engine power.")]
		public float powerCoefficient = 2f;

		// Token: 0x040021CE RID: 8654
		[SerializeField]
		public NOSSoundComponent soundComponent = new NOSSoundComponent();

		// Token: 0x040021CF RID: 8655
		private float initialEngineVolumeRange;

		// Token: 0x040021D0 RID: 8656
		private bool wasActive;
	}
}
