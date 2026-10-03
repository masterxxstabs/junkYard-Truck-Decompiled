using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
	// Token: 0x020002B2 RID: 690
	[Serializable]
	public class ExhaustFlash : Effect
	{
		// Token: 0x0600125E RID: 4702 RVA: 0x000B61C6 File Offset: 0x000B43C6
		public override void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x000C5D09 File Offset: 0x000C3F09
		public override void Awake(VehicleController vc)
		{
			base.Awake(vc);
			this.DisableEffects();
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00002188 File Offset: 0x00000388
		public override void FixedUpdate()
		{
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x000C5D18 File Offset: 0x000C3F18
		public override void Update()
		{
			if (!base.Active)
			{
				return;
			}
			if (this._hasFlashed)
			{
				this.DisableEffects();
			}
			this.flash |= (this.vc.powertrain.engine.revLimiterActive && this.flashOnRevLimiter);
			this.flash |= (this.vc.powertrain.transmission.IsShifting && this.flashOnShift && this.vc.powertrain.engine.RPMPercent > 0.6f);
			if (this.flash)
			{
				this.EnableEffects();
			}
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x000C5DC2 File Offset: 0x000C3FC2
		public override void Enable()
		{
			base.Enable();
			this.EnableEffects();
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x000C5DD0 File Offset: 0x000C3FD0
		private void EnableEffects()
		{
			if (this._hasFlashed)
			{
				return;
			}
			int count = this.flashTextures.Count;
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				meshRenderer.material.SetTexture("_MainTex", this.flashTextures[Random.Range(0, count)]);
				float num = Random.Range(0.2f, 0.6f);
				meshRenderer.transform.localScale = new Vector3(num, num, num);
				meshRenderer.enabled = true;
			}
			foreach (Light light in this.flashLights)
			{
				light.enabled = true;
			}
			this.flash = false;
			this._hasFlashed = true;
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x000C5ECC File Offset: 0x000C40CC
		public override void Disable()
		{
			base.Disable();
			this.DisableEffects();
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x000C5EDC File Offset: 0x000C40DC
		private void DisableEffects()
		{
			foreach (MeshRenderer meshRenderer in this.meshRenderers)
			{
				meshRenderer.enabled = false;
			}
			foreach (Light light in this.flashLights)
			{
				light.enabled = false;
			}
			this._hasFlashed = false;
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x000C5F74 File Offset: 0x000C4174
		public void Flash()
		{
			this.flash = true;
		}

		// Token: 0x040022D0 RID: 8912
		public bool flash;

		// Token: 0x040022D1 RID: 8913
		public List<Light> flashLights = new List<Light>();

		// Token: 0x040022D2 RID: 8914
		public bool flashOnRevLimiter = true;

		// Token: 0x040022D3 RID: 8915
		public bool flashOnShift = true;

		// Token: 0x040022D4 RID: 8916
		[Tooltip("Textures representing exhaust flash. If multiple are assigned a random texture will be chosen for each flash.")]
		public List<Texture2D> flashTextures = new List<Texture2D>();

		// Token: 0x040022D5 RID: 8917
		[Tooltip("    Mesh renderer(s) for the exhaust flash meshes. Materials used should have '_TintColor' property.")]
		public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

		// Token: 0x040022D6 RID: 8918
		private bool _hasFlashed;
	}
}
