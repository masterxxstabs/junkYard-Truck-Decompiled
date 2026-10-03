using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x020002CD RID: 717
	[ExecuteInEditMode]
	[RequireComponent(typeof(Light))]
	public class GlitterLight : MonoBehaviour
	{
		// Token: 0x0600135F RID: 4959 RVA: 0x000CB671 File Offset: 0x000C9871
		private void OnEnable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x000CB671 File Offset: 0x000C9871
		private void OnDisable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x000CB680 File Offset: 0x000C9880
		private void Update()
		{
			Shader.SetGlobalVector("_gGlitterLightDir", -base.transform.forward);
			Shader.SetGlobalVector("_gGlitterLightWorldPos", base.transform.position);
			if (this.lght != null)
			{
				Shader.SetGlobalColor("_gGlitterLightColor", this.lght.color);
			}
		}

		// Token: 0x040023E3 RID: 9187
		private Light lght;
	}
}
