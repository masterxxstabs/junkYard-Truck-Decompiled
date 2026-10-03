using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
[Serializable]
public class EnviroInteriorZoneSettings
{
	// Token: 0x04000519 RID: 1305
	[HideInInspector]
	public Color currentInteriorDirectLightMod;

	// Token: 0x0400051A RID: 1306
	[HideInInspector]
	public Color currentInteriorAmbientLightMod;

	// Token: 0x0400051B RID: 1307
	[HideInInspector]
	public Color currentInteriorAmbientEQLightMod;

	// Token: 0x0400051C RID: 1308
	[HideInInspector]
	public Color currentInteriorAmbientGRLightMod;

	// Token: 0x0400051D RID: 1309
	[HideInInspector]
	public Color currentInteriorSkyboxMod;

	// Token: 0x0400051E RID: 1310
	[HideInInspector]
	public Color currentInteriorFogColorMod = new Color(0f, 0f, 0f, 0f);

	// Token: 0x0400051F RID: 1311
	[HideInInspector]
	public float currentInteriorFogMod = 1f;

	// Token: 0x04000520 RID: 1312
	[HideInInspector]
	public float currentInteriorWeatherEffectMod = 1f;

	// Token: 0x04000521 RID: 1313
	[HideInInspector]
	public float currentInteriorZoneAudioVolume = 1f;

	// Token: 0x04000522 RID: 1314
	[HideInInspector]
	public float currentInteriorZoneAudioFadingSpeed = 1f;
}
