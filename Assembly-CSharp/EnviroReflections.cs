using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class EnviroReflections : MonoBehaviour
{
	// Token: 0x06000362 RID: 866 RVA: 0x0001EF82 File Offset: 0x0001D182
	private void Start()
	{
		if (this.probe == null)
		{
			this.probe = base.GetComponent<ReflectionProbe>();
		}
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0001EF9E File Offset: 0x0001D19E
	private void UpdateProbe()
	{
		this.probe.RenderProbe();
		this.lastUpdate = EnviroSkyMgr.instance.GetCurrentTimeInHours();
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0001EFBC File Offset: 0x0001D1BC
	private void Update()
	{
		if (EnviroSkyMgr.instance != null && !EnviroSkyMgr.instance.IsAvailable())
		{
			return;
		}
		if (EnviroSkyMgr.instance.GetCurrentTimeInHours() > this.lastUpdate + (double)this.ReflectionUpdateInGameHours || EnviroSkyMgr.instance.GetCurrentTimeInHours() < this.lastUpdate - (double)this.ReflectionUpdateInGameHours)
		{
			this.UpdateProbe();
		}
	}

	// Token: 0x04000789 RID: 1929
	public ReflectionProbe probe;

	// Token: 0x0400078A RID: 1930
	public float ReflectionUpdateInGameHours = 1f;

	// Token: 0x0400078B RID: 1931
	private double lastUpdate;
}
