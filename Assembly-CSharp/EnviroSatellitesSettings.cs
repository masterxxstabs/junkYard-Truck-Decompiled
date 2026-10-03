using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007D RID: 125
[Serializable]
public class EnviroSatellitesSettings
{
	// Token: 0x040005EE RID: 1518
	[Tooltip("List of satellites.")]
	public List<EnviroSatellite> additionalSatellites = new List<EnviroSatellite>();
}
