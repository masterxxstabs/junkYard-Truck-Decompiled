using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class EnviroDayNightSwitch : MonoBehaviour
{
	// Token: 0x0600034B RID: 843 RVA: 0x0001E2EC File Offset: 0x0001C4EC
	private void Start()
	{
		this.lightsArray = base.GetComponentsInChildren<Light>();
		EnviroSkyMgr.instance.OnDayTime += delegate()
		{
			this.Deactivate();
		};
		EnviroSkyMgr.instance.OnNightTime += delegate()
		{
			this.Activate();
		};
		if (EnviroSkyMgr.instance.IsNight())
		{
			this.Activate();
			return;
		}
		this.Deactivate();
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0001E34C File Offset: 0x0001C54C
	private void Activate()
	{
		for (int i = 0; i < this.lightsArray.Length; i++)
		{
			this.lightsArray[i].enabled = true;
		}
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0001E37C File Offset: 0x0001C57C
	private void Deactivate()
	{
		for (int i = 0; i < this.lightsArray.Length; i++)
		{
			this.lightsArray[i].enabled = false;
		}
	}

	// Token: 0x04000755 RID: 1877
	private Light[] lightsArray;
}
