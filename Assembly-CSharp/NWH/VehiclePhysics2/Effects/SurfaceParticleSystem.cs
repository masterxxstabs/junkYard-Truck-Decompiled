using System;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002BC RID: 700
	[Serializable]
	public class SurfaceParticleSystem
	{
		// Token: 0x0600129C RID: 4764 RVA: 0x000C84E4 File Offset: 0x000C66E4
		public void Initialize(VehicleController vc, WheelComponent wheelComponent)
		{
			this._vc = vc;
			this._wheelComponent = wheelComponent;
			if (!vc.groundDetection.IsEnabled || vc.groundDetection.groundDetectionPreset == null)
			{
				return;
			}
			if (vc.groundDetection.groundDetectionPreset.particlePrefab != null)
			{
				this.particlePrefab = Object.Instantiate<GameObject>(vc.groundDetection.groundDetectionPreset.particlePrefab, wheelComponent.ControllerTransform, true);
				this.particlePrefab.transform.position = wheelComponent.wheelController.wheel.worldPosition - wheelComponent.wheelController.cachedTransform.up * (wheelComponent.wheelController.radius * 0.5f);
				this.particlePS = this.particlePrefab.GetComponent<ParticleSystem>();
				this.particlePS.name = "SurfaceParticles";
				this.particlePS.shape.radius = wheelComponent.Width * 1.5f;
				this._shapeModule = this.particlePS.shape;
				this._shapeModule.radius = wheelComponent.Width;
			}
			else
			{
				Debug.LogWarning("Smoke Prefab is null, wheel slip will not produce particles.");
			}
			if (vc.groundDetection.groundDetectionPreset.chunkPrefab != null)
			{
				this.chunkPrefab = Object.Instantiate<GameObject>(vc.groundDetection.groundDetectionPreset.chunkPrefab, wheelComponent.ControllerTransform, true);
				this.chunkPrefab.transform.position = wheelComponent.wheelController.wheel.worldPosition - wheelComponent.wheelController.cachedTransform.up * wheelComponent.wheelController.radius - wheelComponent.wheelController.cachedTransform.forward * (wheelComponent.wheelController.radius * 0.7f);
				this.chunkPS = this.chunkPrefab.GetComponent<ParticleSystem>();
				this.chunkPS.name = "SurfaceChunks";
				this._shapeModule = this.chunkPS.shape;
				this._shapeModule.radius = wheelComponent.Width;
			}
			else
			{
				Debug.LogWarning("Dust Prefab is null, there will be no surface dust.");
			}
			this._initialized = true;
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x000C871C File Offset: 0x000C691C
		public void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			bool isGrounded = this._wheelComponent.IsGrounded;
			this.surfacePreset = this._wheelComponent.surfacePreset;
			this.particleCount = 0;
			if (!isGrounded || this.surfacePreset == null)
			{
				this.StopParticleEmission();
				this.StopChunkEmission();
				return;
			}
			this.UpdateParticles();
			this.UpdateChunks();
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x000C8780 File Offset: 0x000C6980
		private void UpdateParticles()
		{
			if (!this.surfacePreset.emitParticles)
			{
				this.StopParticleEmission();
				return;
			}
			this._mainModule = this.particlePS.main;
			this._emissionModule = this.particlePS.emission;
			this._mainModule.startColor = this.surfacePreset.particleColor;
			this._mainModule.startSize = this.surfacePreset.particleSize * this.particleSizeCoeff;
			float num = this.surfacePreset.particleLifeDistance / this._wheelComponent.wheelController.speed;
			num = Mathf.Clamp(num, 2f, this.surfacePreset.maxParticleLifetime);
			this._mainModule.startLifetime = num;
			if (this.surfacePreset.particleType == SurfacePreset.ParticleType.Smoke)
			{
				if (!this._wheelComponent.HasLateralSlip && !this._wheelComponent.HasLongitudinalSlip)
				{
					this.StopParticleEmission();
					return;
				}
				float num2 = this._wheelComponent.HasLateralSlip ? (this._wheelComponent.NormalizedLateralSlip * this.lateralSlipCoeff) : 0f;
				float num3 = this._wheelComponent.HasLongitudinalSlip ? (this._wheelComponent.NormalizedLongitudinalSlip * this.longitudinalSlipCoeff) : 0f;
				float num4 = num2 + num3;
				num4 = Mathf.Clamp01(num4) * this.surfacePreset.maxParticleEmissionRateOverDistance;
				this._smokeEmissionRate = Mathf.SmoothDamp(this._smokeEmissionRate, num4, ref this._smokeEmissionRateVelocity, 1f);
				this._particleColor = this._mainModule.startColor.color;
				this._minMaxGradient = this._mainModule.startColor;
				this._minMaxGradient.color = new Color(this._particleColor.r, this._particleColor.g, this._particleColor.b, Mathf.Clamp01(this._smokeEmissionRate) * this.surfacePreset.particleMaxAlpha);
				this._mainModule.startColor = this._minMaxGradient;
				float num5 = Mathf.Clamp01(this._vc.Speed / 3f);
				this._rateOverDistance = num5 * this._smokeEmissionRate;
				this._rateOverTime = (1f - num5) * this._smokeEmissionRate;
				this._emissionModule.rateOverDistance = this._rateOverDistance * this.emissionRateCoeff;
				this._emissionModule.rateOverTime = this._rateOverTime * this.emissionRateCoeff;
			}
			else
			{
				float num6 = 0f;
				if (this._wheelComponent.IsGrounded)
				{
					num6 = Mathf.Clamp01(this._vc.Speed / 8f - 0.05f) * this.surfacePreset.maxParticleEmissionRateOverDistance;
				}
				this._particleColor = this._mainModule.startColor.color;
				this._minMaxGradient = this._mainModule.startColor;
				this._minMaxGradient.color = new Color(this._particleColor.r, this._particleColor.g, this._particleColor.b, Mathf.Clamp01(num6 * 2f) * this.surfacePreset.particleMaxAlpha);
				this._mainModule.startColor = this._minMaxGradient;
				this._emissionModule.rateOverTime = 0f;
				this._emissionModule.rateOverDistance = num6 * this.emissionRateCoeff;
			}
			this.particleCount += this.particlePS.particleCount;
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x000C8AFC File Offset: 0x000C6CFC
		private void UpdateChunks()
		{
			if (!this.surfacePreset.emitChunks)
			{
				this.StopChunkEmission();
				return;
			}
			this._mainModule = this.chunkPS.main;
			this._emissionModule = this.chunkPS.emission;
			float num = this.surfacePreset.chunkLifeDistance / this._wheelComponent.wheelController.speed;
			num = Mathf.Clamp(num, 0.2f, this.surfacePreset.maxChunkLifetime);
			this._mainModule.startLifetime = num;
			float angularVelocity = this._wheelComponent.angularVelocity;
			if (((angularVelocity < 0f) ? (-angularVelocity) : angularVelocity) < 5f)
			{
				this._emissionModule.rateOverTime = 0f;
				this._emissionModule.rateOverDistance = 0f;
			}
			else
			{
				float num2 = this._wheelComponent.angularVelocity * this._wheelComponent.wheelController.radius;
				this._mainModule.startSpeed = num2 * 0.2f;
				this._emissionModule.rateOverTime = 0f;
				this._emissionModule.rateOverDistance = (this._wheelComponent.NormalizedLongitudinalSlip * 0.7f + this._wheelComponent.NormalizedLateralSlip * 0.3f) * this.surfacePreset.maxChunkEmissionRateOverDistance;
			}
			this.particleCount += this.chunkPS.particleCount;
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x000C8C73 File Offset: 0x000C6E73
		private void StopParticleEmission()
		{
			this._emissionModule = this.particlePS.emission;
			this._emissionModule.rateOverDistance = 0f;
			this._emissionModule.rateOverTime = 0f;
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000C8CB0 File Offset: 0x000C6EB0
		private void StopChunkEmission()
		{
			this._emissionModule = this.chunkPS.emission;
			this._emissionModule.rateOverDistance = 0f;
			this._emissionModule.rateOverTime = 0f;
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x000C8CF0 File Offset: 0x000C6EF0
		public void Enable()
		{
			if (this.particlePS != null && this.particlePS.isStopped)
			{
				this.particlePS.Play();
			}
			if (this.chunkPS != null && this.chunkPS.isStopped)
			{
				this.chunkPS.Play();
			}
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x000C8D4C File Offset: 0x000C6F4C
		public void Disable()
		{
			if (this.particlePS != null && !this.particlePS.isStopped)
			{
				this.particlePS.Stop();
			}
			if (this.chunkPS != null && !this.chunkPS.isStopped)
			{
				this.chunkPS.Stop();
			}
		}

		// Token: 0x0400234B RID: 9035
		public float lateralSlipCoeff = 0.5f;

		// Token: 0x0400234C RID: 9036
		public float longitudinalSlipCoeff = 0.5f;

		// Token: 0x0400234D RID: 9037
		public float particleSizeCoeff = 1f;

		// Token: 0x0400234E RID: 9038
		public float emissionRateCoeff = 1f;

		// Token: 0x0400234F RID: 9039
		public int particleCount;

		// Token: 0x04002350 RID: 9040
		public ParticleSystem particlePS;

		// Token: 0x04002351 RID: 9041
		public ParticleSystem chunkPS;

		// Token: 0x04002352 RID: 9042
		public GameObject particlePrefab;

		// Token: 0x04002353 RID: 9043
		public GameObject chunkPrefab;

		// Token: 0x04002354 RID: 9044
		private ParticleSystem.MainModule _mainModule;

		// Token: 0x04002355 RID: 9045
		private ParticleSystem.EmissionModule _emissionModule;

		// Token: 0x04002356 RID: 9046
		private ParticleSystem.ShapeModule _shapeModule;

		// Token: 0x04002357 RID: 9047
		private float _rateOverDistance;

		// Token: 0x04002358 RID: 9048
		private float _rateOverTime;

		// Token: 0x04002359 RID: 9049
		private float _smokeEmissionRateVelocity;

		// Token: 0x0400235A RID: 9050
		private VehicleController _vc;

		// Token: 0x0400235B RID: 9051
		private WheelComponent _wheelComponent;

		// Token: 0x0400235C RID: 9052
		private Color _particleColor;

		// Token: 0x0400235D RID: 9053
		private ParticleSystem.MinMaxGradient _minMaxGradient;

		// Token: 0x0400235E RID: 9054
		[SerializeField]
		private SurfacePreset surfacePreset;

		// Token: 0x0400235F RID: 9055
		private float _smokeEmissionRate;

		// Token: 0x04002360 RID: 9056
		private bool _initialized;
	}
}
