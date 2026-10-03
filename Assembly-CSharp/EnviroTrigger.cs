using System;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class EnviroTrigger : MonoBehaviour
{
	// Token: 0x0600036B RID: 875 RVA: 0x00002188 File Offset: 0x00000388
	private void Start()
	{
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00002188 File Offset: 0x00000388
	private void Update()
	{
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0001F1D8 File Offset: 0x0001D3D8
	private void OnTriggerEnter(Collider col)
	{
		if (EnviroSkyMgr.instance.GetUseWeatherTag())
		{
			if (col.gameObject.tag == EnviroSkyMgr.instance.GetEnviroSkyTag())
			{
				this.EnterExit();
				return;
			}
		}
		else if (EnviroSkyMgr.instance.IsEnviroSkyAttached(col.gameObject))
		{
			this.EnterExit();
		}
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0001F22C File Offset: 0x0001D42C
	private void OnTriggerExit(Collider col)
	{
		if (this.myZone.zoneTriggerType == EnviroInterior.ZoneTriggerType.Zone)
		{
			if (EnviroSkyMgr.instance.GetUseWeatherTag())
			{
				if (col.gameObject.tag == EnviroSkyMgr.instance.GetEnviroSkyTag())
				{
					this.EnterExit();
					return;
				}
			}
			else if (EnviroSkyMgr.instance.IsEnviroSkyAttached(col.gameObject))
			{
				this.EnterExit();
			}
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0001F290 File Offset: 0x0001D490
	private void EnterExit()
	{
		if (EnviroSkyMgr.instance.lastInteriorZone != this.myZone)
		{
			if (EnviroSkyMgr.instance.lastInteriorZone != null)
			{
				EnviroSkyMgr.instance.lastInteriorZone.StopAllFading();
			}
			this.myZone.Enter();
			return;
		}
		if (!EnviroSkyMgr.instance.IsInterior())
		{
			this.myZone.Enter();
			return;
		}
		this.myZone.Exit();
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0001F304 File Offset: 0x0001D504
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0.2f, 0.2f, 1f, 0.5f);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	// Token: 0x04000790 RID: 1936
	public EnviroInterior myZone;

	// Token: 0x04000791 RID: 1937
	public string Name;
}
