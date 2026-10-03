using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B9 RID: 697
	[Serializable]
	public class SkidmarkManager : Effect
	{
		// Token: 0x06001290 RID: 4752 RVA: 0x000C7DB0 File Offset: 0x000C5FB0
		public override void Initialize()
		{
			if (this.vc.groundDetection.groundDetectionPreset == null)
			{
				return;
			}
			this.skidmarkContainer = GameObject.Find("SkidContainer");
			if (this.skidmarkContainer == null)
			{
				this.skidmarkContainer = new GameObject("SkidContainer");
				this.skidmarkContainer.isStatic = true;
			}
			foreach (WheelComponent wheelComponent in this.vc.powertrain.wheels)
			{
				SkidmarkGenerator skidmarkGenerator = new SkidmarkGenerator();
				skidmarkGenerator.Initialize(wheelComponent, this.skidmarkContainer, this.minDistance, this.vc.groundDetection.groundDetectionPreset.surfaceMaps.Count, this.maxMarksPerSection, this.persistentSkidmarks, this.persistentSkidmarkDestroyDistance, this.groundOffset, this.smoothing, this.lowerIntensityThreshold, this.fadeOverDistance, this.vc.groundDetection.groundDetectionPreset.fallbackSurfacePreset.skidmarkMaterial);
				this.skidmarkGenerators.Add(skidmarkGenerator);
			}
			float num = (float)this.maxMarksPerSection * this.minDistance * 1.5f;
			if (this.persistentSkidmarkDestroyDistance < num)
			{
				this.persistentSkidmarkDestroyDistance = num;
			}
			this.prevWheelCount = this.vc.Wheels.Count;
			this.initialized = true;
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x000C7F28 File Offset: 0x000C6128
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this.vc.groundDetection == null || !this.vc.groundDetection.IsEnabled)
			{
				return;
			}
			int count = this.vc.powertrain.wheels.Count;
			if (this.prevWheelCount != count)
			{
				this.initialized = false;
				this.Initialize();
			}
			this.prevWheelCount = count;
			int count2 = this.skidmarkGenerators.Count;
			for (int i = 0; i < count2; i++)
			{
				WheelComponent wheelComponent = this.vc.powertrain.wheels[i];
				SurfacePreset surfacePreset = wheelComponent.surfacePreset;
				if (!(surfacePreset == null) && surfacePreset.drawSkidmarks)
				{
					bool flag = surfacePreset == null;
					int num = -1;
					if (!flag)
					{
						num = wheelComponent.surfaceMapIndex;
					}
					float num2 = 1f;
					if (num >= 0)
					{
						float num3 = wheelComponent.NormalizedLateralSlip;
						num3 = ((num3 < this.vc.lateralSlipThreshold) ? 0f : (num3 - this.vc.lateralSlipThreshold));
						float num4 = wheelComponent.NormalizedLongitudinalSlip;
						num4 = ((num4 < this.vc.lateralSlipThreshold) ? 0f : (num4 - this.vc.lateralSlipThreshold));
						float num5 = num3 + num4;
						float num6 = wheelComponent.wheelController.wheel.load * 2f / wheelComponent.wheelController.maximumTireLoad;
						num6 = ((num6 < 0f) ? 0f : ((num6 > 1f) ? 1f : num6));
						num5 *= wheelComponent.surfacePreset.slipFactor * num6;
						num2 = wheelComponent.surfacePreset.skidmarkBaseIntensity + num5;
						num2 = ((num2 > 1f) ? 1f : ((num2 < 0f) ? 0f : num2));
					}
					num2 *= this.globalSkidmarkIntensity;
					num2 = ((num2 < 0f) ? 0f : ((num2 > this.maxSkidmarkAlpha) ? this.maxSkidmarkAlpha : num2));
					float albedoIntensity = 0f;
					float normalIntensity = 0f;
					this.skidmarkGenerators[i].Update(num, num2, albedoIntensity, normalIntensity, wheelComponent.wheelController.pointVelocity, this.vc.fixedDeltaTime);
				}
			}
		}

		// Token: 0x04002333 RID: 9011
		[Tooltip("Should skidmarks fade as they get nearer to getting destroyed. This results in a soft alpha transition rather than hard\r\ncut\r\nat the end of the skidmark. Ignored when persistent skidmarks are used.")]
		public bool fadeOverDistance = true;

		// Token: 0x04002334 RID: 9012
		[Range(0f, 1f)]
		[Tooltip("Higher value will give darker skidmarks for the same slip. Check corresponding SurfacePreset (GroundDetection -> Presets)\r\nfor per-surface settings.")]
		public float globalSkidmarkIntensity = 0.6f;

		// Token: 0x04002335 RID: 9013
		[Tooltip("Height above ground at which skidmarks will be drawn. If too low clipping between skidmark and ground surface will\r\noccur.")]
		public float groundOffset = 0.025f;

		// Token: 0x04002336 RID: 9014
		[Tooltip("    When skidmark alpha value is below this value skidmark mesh will not be generated.")]
		public float lowerIntensityThreshold = 0.05f;

		// Token: 0x04002337 RID: 9015
		[Tooltip("Number of skidmarks that will be drawn per one section, before mesh is saved and new one is generated.")]
		public int maxMarksPerSection = 180;

		// Token: 0x04002338 RID: 9016
		[Range(0f, 1f)]
		[Tooltip("    Max skidmark texture alpha.")]
		public float maxSkidmarkAlpha = 0.6f;

		// Token: 0x04002339 RID: 9017
		[Tooltip("    Distance from the last skidmark section needed to generate a new one.")]
		public float minDistance = 0.12f;

		// Token: 0x0400233A RID: 9018
		[Tooltip("    Persistent skidmarks get deleted when distance from the parent vehicle is higher than this.")]
		public float persistentSkidmarkDestroyDistance = 100f;

		// Token: 0x0400233B RID: 9019
		[Tooltip("If enabled skidmarks will stay on the ground until distance from the vehicle becomes greater than persistentSkidmarkDistance. If disabled skidmarks will stay on the ground until maxMarksPerSection is reached and then will start getting deleted from the oldest skidmark.")]
		public bool persistentSkidmarks;

		// Token: 0x0400233C RID: 9020
		[Range(0.01f, 0.1f)]
		[Tooltip("    Smoothing between skidmark triangles. Value represents time required for alpha to go from 0 to 1.")]
		public float smoothing = 0.07f;

		// Token: 0x0400233D RID: 9021
		private int prevWheelCount;

		// Token: 0x0400233E RID: 9022
		private GameObject skidmarkContainer;

		// Token: 0x0400233F RID: 9023
		private List<SkidmarkGenerator> skidmarkGenerators = new List<SkidmarkGenerator>();
	}
}
