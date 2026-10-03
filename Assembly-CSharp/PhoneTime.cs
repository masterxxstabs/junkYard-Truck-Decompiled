using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000111 RID: 273
public class PhoneTime : MonoBehaviour
{
	// Token: 0x0600073A RID: 1850 RVA: 0x0005E61C File Offset: 0x0005C81C
	private void Update()
	{
		if (Time.time >= (float)this.nextUpdate)
		{
			this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
			this.currTime = EnviroSkyMgr.instance.GetTimeOfDay().ToString();
			string[] array = this.currTime.Split(new char[]
			{
				char.Parse(".")
			});
			string str = array[0];
			float num = float.Parse(array[1].Substring(0, 2));
			this.min = Mathf.Round(num * 60f / 100f).ToString();
			if (this.min.Length < 2)
			{
				this.min = "0" + this.min;
			}
			else if (this.min == "60")
			{
				this.min = "00";
			}
			this.clockText.text = str + ":" + this.min;
		}
	}

	// Token: 0x04001030 RID: 4144
	public Text clockText;

	// Token: 0x04001031 RID: 4145
	private int nextUpdate;

	// Token: 0x04001032 RID: 4146
	private string currTime;

	// Token: 0x04001033 RID: 4147
	private string min;
}
