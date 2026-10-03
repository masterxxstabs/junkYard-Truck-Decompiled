using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
[Serializable]
public class EnviroComponents
{
	// Token: 0x04000500 RID: 1280
	[Tooltip("The Enviro sun object.")]
	public GameObject Sun;

	// Token: 0x04000501 RID: 1281
	[Tooltip("The Enviro moon object.")]
	public GameObject Moon;

	// Token: 0x04000502 RID: 1282
	[Tooltip("The directional light for directional sun lighting when using dual mode. Used for sun and moon in single mode.")]
	public Transform DirectLight;

	// Token: 0x04000503 RID: 1283
	[Tooltip("The directional light for directional moon lighting when using the dual mode.")]
	public Transform AdditionalDirectLight;

	// Token: 0x04000504 RID: 1284
	[Tooltip("The Enviro global reflection probe for dynamic reflections.")]
	public EnviroReflectionProbe GlobalReflectionProbe;

	// Token: 0x04000505 RID: 1285
	[Tooltip("Your WindZone that reflect our weather wind settings.")]
	public WindZone windZone;

	// Token: 0x04000506 RID: 1286
	[Tooltip("The Enviro Lighting Flash Component.")]
	public EnviroLightning LightningGenerator;

	// Token: 0x04000507 RID: 1287
	[Tooltip("Link to the object that hold all additional satellites as childs.")]
	public Transform satellites;

	// Token: 0x04000508 RID: 1288
	[Tooltip("Just a transform for stars rotation calculations. ")]
	public Transform starsRotation;

	// Token: 0x04000509 RID: 1289
	[Tooltip("Plane to cast cloud shadows.")]
	public GameObject particleClouds;
}
