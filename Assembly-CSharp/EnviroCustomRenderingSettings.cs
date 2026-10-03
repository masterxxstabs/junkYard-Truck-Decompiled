using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
[Serializable]
public class EnviroCustomRenderingSettings
{
	// Token: 0x040007D6 RID: 2006
	[Header("Feature Control")]
	public bool useVolumeClouds = true;

	// Token: 0x040007D7 RID: 2007
	public bool useVolumeLighting = true;

	// Token: 0x040007D8 RID: 2008
	public bool useDistanceBlur = true;

	// Token: 0x040007D9 RID: 2009
	public bool useFog = true;

	// Token: 0x040007DA RID: 2010
	public EnviroVolumeCloudsQuality customCloudsQuality;
}
