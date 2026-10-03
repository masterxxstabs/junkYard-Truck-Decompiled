using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B3 RID: 691
	[Serializable]
	public class ExhaustSmoke : Effect
	{
		// Token: 0x06001268 RID: 4712 RVA: 0x000C5FB4 File Offset: 0x000C41B4
		public override void Initialize()
		{
			this.initialized = true;
			using (List<ParticleSystem>.Enumerator enumerator = this.particleSystems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						Debug.LogError("One or more of the exhaust ParticleSystems on the vehicle " + this.vc.name + " is null.");
					}
				}
			}
			if (this.particleSystems == null || this.particleSystems.Count == 0)
			{
				this.state.isEnabled = false;
			}
			else
			{
				this._emissionModule = this.particleSystems[0].emission;
				this._mainModule = this.particleSystems[0].main;
				this._initLifetime = this._mainModule.startLifetime.constant;
				this._initStartSpeedMin = this._mainModule.startSpeed.constantMin;
				this._initStartSpeedMax = this._mainModule.startSpeed.constantMax;
				this._initStartSizeMin = this._mainModule.startSize.constantMin;
				this._initStartSizeMax = this._mainModule.startSize.constantMax;
			}
			this.maxSizeMultiplier = Mathf.Clamp(this.maxSizeMultiplier, 1f, float.PositiveInfinity);
			this.maxSpeedMultiplier = Mathf.Clamp(this.maxSpeedMultiplier, 1f, float.PositiveInfinity);
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x000C6134 File Offset: 0x000C4334
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.powertrain.Active && this.vc.powertrain.engine.IsRunning)
			{
				this._vehicleSpeed = this.vc.Speed;
				this._absVehicleSpeed = ((this._vehicleSpeed < 0f) ? (-this._vehicleSpeed) : this._vehicleSpeed);
				using (List<ParticleSystem>.Enumerator enumerator = this.particleSystems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ParticleSystem particleSystem = enumerator.Current;
						if (!particleSystem.isPlaying)
						{
							particleSystem.Play();
						}
						this._emissionModule = particleSystem.emission;
						this._mainModule = particleSystem.main;
						this.vc.powertrain.engine.GetLoad();
						float rpmpercent = this.vc.powertrain.engine.RPMPercent;
						if (!this._emissionModule.enabled)
						{
							this._emissionModule.enabled = true;
						}
						float b = (this._vehicleSpeed < 0.2f && this._vehicleSpeed > -0.2f) ? 0f : (this.lifetimeDistance / this._vehicleSpeed);
						float constant = Mathf.Lerp(this._initLifetime, b, this._absVehicleSpeed * 0.35f);
						this._mainModule.startLifetime = constant;
						this._sootAmount = this.vc.powertrain.engine.throttlePosition * this.sootIntensity;
						this._mainModule.startColor = Color.Lerp(this._mainModule.startColor.color, Color.Lerp(this.normalColor, this.sootColor, this._sootAmount), Time.deltaTime * 7f);
						float num = this.maxSpeedMultiplier - 1f;
						this._minMaxCurve = this._mainModule.startSpeed;
						this._minMaxCurve.constantMin = this._initStartSpeedMin + rpmpercent * num;
						this._minMaxCurve.constantMax = this._initStartSpeedMax + rpmpercent * num;
						this._mainModule.startSpeed = this._minMaxCurve;
						float num2 = this.maxSizeMultiplier - 1f;
						this._minMaxCurve = this._mainModule.startSize;
						this._minMaxCurve.constantMin = this._initStartSizeMin + rpmpercent * num2;
						this._minMaxCurve.constantMax = this._initStartSizeMax + rpmpercent * num2;
						this._mainModule.startSize = this._minMaxCurve;
						if (this.vc.damageHandler.IsEnabled)
						{
							this._sootAmount += this.vc.damageHandler.Damage;
						}
					}
					return;
				}
			}
			foreach (ParticleSystem particleSystem2 in this.particleSystems)
			{
				if (particleSystem2.isPlaying)
				{
					particleSystem2.Stop();
				}
				particleSystem2.emission.enabled = false;
			}
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x000C6480 File Offset: 0x000C4680
		public override void Enable()
		{
			base.Enable();
			foreach (ParticleSystem particleSystem in this.particleSystems)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				particleSystem.Play();
			}
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x000C64E0 File Offset: 0x000C46E0
		public override void Disable()
		{
			base.Disable();
			foreach (ParticleSystem particleSystem in this.particleSystems)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				particleSystem.Stop();
			}
		}

		// Token: 0x040022D7 RID: 8919
		[Range(0f, 1f)]
		public float lifetimeDistance = 0.4f;

		// Token: 0x040022D8 RID: 8920
		[Range(0f, 1f)]
		public float sootIntensity = 0.4f;

		// Token: 0x040022D9 RID: 8921
		[Range(1f, 5f)]
		public float maxSpeedMultiplier = 1.4f;

		// Token: 0x040022DA RID: 8922
		[Range(1f, 5f)]
		public float maxSizeMultiplier = 1.2f;

		// Token: 0x040022DB RID: 8923
		[Tooltip("Normal particle start color. Used when there is no throttle - engine is under no load.")]
		public Color normalColor = new Color(0.6f, 0.6f, 0.6f, 0.3f);

		// Token: 0x040022DC RID: 8924
		[Tooltip("Soot particle start color. Used under heavy throttle - engine is under load.")]
		public Color sootColor = new Color(0.1f, 0.1f, 0.8f);

		// Token: 0x040022DD RID: 8925
		[Tooltip("List of exhaust particle systems.")]
		public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

		// Token: 0x040022DE RID: 8926
		private float _initLifetime;

		// Token: 0x040022DF RID: 8927
		private float _initStartSpeedMin;

		// Token: 0x040022E0 RID: 8928
		private float _initStartSpeedMax;

		// Token: 0x040022E1 RID: 8929
		private float _initStartSizeMin;

		// Token: 0x040022E2 RID: 8930
		private float _initStartSizeMax;

		// Token: 0x040022E3 RID: 8931
		private float _sootAmount;

		// Token: 0x040022E4 RID: 8932
		private ParticleSystem.EmissionModule _emissionModule;

		// Token: 0x040022E5 RID: 8933
		private ParticleSystem.MainModule _mainModule;

		// Token: 0x040022E6 RID: 8934
		private ParticleSystem.MinMaxCurve _minMaxCurve;

		// Token: 0x040022E7 RID: 8935
		private float _vehicleSpeed;

		// Token: 0x040022E8 RID: 8936
		private float _absVehicleSpeed;
	}
}
