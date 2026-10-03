using System;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B4 RID: 692
	[Serializable]
	public class LightSource
	{
		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x000C65C3 File Offset: 0x000C47C3
		// (set) Token: 0x0600126F RID: 4719 RVA: 0x000C65CB File Offset: 0x000C47CB
		public bool IsOn { get; private set; }

		// Token: 0x06001270 RID: 4720 RVA: 0x000C65D4 File Offset: 0x000C47D4
		public virtual void TurnOff()
		{
			if (this.IsOn)
			{
				this.onLightTurnedOff.Invoke();
			}
			if (this.type == LightSource.LightType.Light && this.light != null)
			{
				this.light.enabled = false;
			}
			else if (Application.isPlaying)
			{
				if (this.meshRenderer == null || this.meshRenderer.material == null)
				{
					return;
				}
				this.meshRenderer.materials[this.rendererMaterialIndex].DisableKeyword("_EMISSION");
			}
			this.IsOn = false;
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x000C6664 File Offset: 0x000C4864
		public virtual void TurnOn()
		{
			if (!this.IsOn)
			{
				this.onLightTurnedOn.Invoke();
			}
			if (this.type == LightSource.LightType.Light && this.light != null)
			{
				this.light.enabled = true;
			}
			else if (Application.isPlaying)
			{
				if (this.meshRenderer == null || this.meshRenderer.material == null)
				{
					return;
				}
				Material material = this.meshRenderer.materials[this.rendererMaterialIndex];
				material.EnableKeyword("_EMISSION");
				material.SetColor("_EmissionColor", this.emissionColor);
			}
			this.IsOn = true;
		}

		// Token: 0x040022E9 RID: 8937
		[ColorUsage(true, true)]
		[Tooltip("    Color of the emitted light.")]
		public Color emissionColor;

		// Token: 0x040022EA RID: 8938
		[Tooltip("Light (point/spot/directional/etc.) representing the vehicle light. Will only be used if light type is set to\r\nLight.")]
		public Light light;

		// Token: 0x040022EB RID: 8939
		[Tooltip("Mesh renderer using standard shader. Emission on the material will be turned on or off depending on light state.")]
		public MeshRenderer meshRenderer;

		// Token: 0x040022EC RID: 8940
		[Tooltip("    If your mesh has more than one material set this number to the index of required material.")]
		public int rendererMaterialIndex;

		// Token: 0x040022ED RID: 8941
		[Tooltip("    Type of the light.")]
		public LightSource.LightType type;

		// Token: 0x040022EE RID: 8942
		[NonSerialized]
		public UnityEvent onLightTurnedOn = new UnityEvent();

		// Token: 0x040022EF RID: 8943
		[NonSerialized]
		public UnityEvent onLightTurnedOff = new UnityEvent();

		// Token: 0x020004C8 RID: 1224
		public enum LightType
		{
			// Token: 0x04002C29 RID: 11305
			Light,
			// Token: 0x04002C2A RID: 11306
			Mesh
		}
	}
}
