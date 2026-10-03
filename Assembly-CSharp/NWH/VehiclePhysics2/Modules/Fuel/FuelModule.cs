using System;
using NWH.VehiclePhysics2.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.Fuel
{
	// Token: 0x0200029A RID: 666
	[Serializable]
	public class FuelModule : VehicleModule
	{
		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x060011E7 RID: 4583 RVA: 0x000C3448 File Offset: 0x000C1648
		public float ConsumptionKilometersPerLiter
		{
			get
			{
				return UnitConverter.L100kmToKml(this.consumptionLPer100km);
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x000C3455 File Offset: 0x000C1655
		public float ConsumptionLitersPer100Kilometers
		{
			get
			{
				return this.consumptionLPer100km;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x000C345D File Offset: 0x000C165D
		public float ConsumptionLitersPerSecond
		{
			get
			{
				return this.consumptionPerHour / 3600f;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x000C346B File Offset: 0x000C166B
		public float ConsumptionMPG
		{
			get
			{
				return UnitConverter.L100kmToMpg(this.consumptionLPer100km);
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x000C3478 File Offset: 0x000C1678
		public float FuelPercentage
		{
			get
			{
				return Mathf.Clamp01(this.amount / this.capacity);
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x000C348C File Offset: 0x000C168C
		public bool HasFuel
		{
			get
			{
				return !base.Active || this.amount > 0f;
			}
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x000C34A8 File Offset: 0x000C16A8
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.powertrain.engine.IsRunning)
			{
				this.maxConsumptionPerHour = this.vc.powertrain.engine.maxPower / 10f * Mathf.Clamp01(1f - this.efficiency);
				this.consumptionPerHour = this.vc.powertrain.engine.generatedPower / this.vc.powertrain.engine.maxPower * this.maxConsumptionPerHour;
				this.consumptionPerHour = Mathf.Clamp(this.consumptionPerHour, this.maxConsumptionPerHour * this.idleConsumption, float.PositiveInfinity) * this.consumptionMultiplier;
				this.amount -= this.consumptionPerHour / 3600f * this.vc.fixedDeltaTime;
				this.amount = Mathf.Clamp(this.amount, 0f, this.capacity);
				if (this.amount == 0f && this.vc.powertrain.engine.IsRunning)
				{
					this.vc.powertrain.engine.Stop();
				}
				this._distanceTraveled = this.vc.Speed * this.vc.fixedDeltaTime;
				this._consumptionThisFrame = this.consumptionPerHour / 3600f * Time.fixedDeltaTime;
				float num = 3600f / Time.fixedDeltaTime;
				float num2 = this._consumptionThisFrame * num;
				float num3 = this._distanceTraveled * num / 100000f;
				this.consumptionLPer100km = ((num3 == 0f) ? 0f : Mathf.Clamp(num2 / num3, 0f, 99.9f));
			}
			else
			{
				this.consumptionPerHour = 0f;
				this.consumptionLPer100km = 0f;
			}
			if (this.amount == 0f && this._prevAmount > 0f)
			{
				this.onOutOfFuel.Invoke();
			}
			this._prevAmount = this.amount;
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x000C36B0 File Offset: 0x000C18B0
		public override void Enable()
		{
			base.Enable();
			this._prevAmount = this.amount;
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x00092832 File Offset: 0x00090A32
		public override VehicleModule.ModuleCategory GetModuleCategory()
		{
			return VehicleModule.ModuleCategory.Powertrain;
		}

		// Token: 0x0400220D RID: 8717
		[Tooltip("    Maximum amount in liters that the fuel tank can hold.")]
		public float amount = 50f;

		// Token: 0x0400220E RID: 8718
		[Tooltip("    Fuel capacity in liters.")]
		public float capacity = 50f;

		// Token: 0x0400220F RID: 8719
		[Tooltip("In case you do not need physically accurate fuel consumption you can lower/rise the consumption in here.")]
		public float consumptionMultiplier = 1f;

		// Token: 0x04002210 RID: 8720
		[Tooltip("Engine efficiency (in percent). 1 would mean that all the energy contained in fuel would go into output power.")]
		public float efficiency = 0.45f;

		// Token: 0x04002211 RID: 8721
		[Tooltip("    Consumption when idling indicated in percentage of max consumption. 0.05f = 5% out of maximum.")]
		public float idleConsumption = 0.1f;

		// Token: 0x04002212 RID: 8722
		public float maxConsumptionPerHour = 20f;

		// Token: 0x04002213 RID: 8723
		[Tooltip("    Called when vehicle runs out of fuel.")]
		public UnityEvent onOutOfFuel;

		// Token: 0x04002214 RID: 8724
		private float _consumptionThisFrame;

		// Token: 0x04002215 RID: 8725
		private float _distanceTraveled;

		// Token: 0x04002216 RID: 8726
		private float _prevAmount;

		// Token: 0x04002217 RID: 8727
		[SerializeField]
		private float consumptionLPer100km;

		// Token: 0x04002218 RID: 8728
		[SerializeField]
		private float consumptionPerHour;
	}
}
