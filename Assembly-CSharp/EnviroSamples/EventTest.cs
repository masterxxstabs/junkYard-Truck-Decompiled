using System;
using UnityEngine;

namespace EnviroSamples
{
	// Token: 0x020002D3 RID: 723
	public class EventTest : MonoBehaviour
	{
		// Token: 0x0600138E RID: 5006 RVA: 0x000CC4A0 File Offset: 0x000CA6A0
		private void Start()
		{
			EnviroSkyMgr.instance.OnWeatherChanged += delegate(EnviroWeatherPreset type)
			{
				this.DoOnWeatherChange(type);
				Debug.Log("Weather changed to: " + type.Name);
			};
			EnviroSkyMgr.instance.OnZoneChanged += delegate(EnviroZone z)
			{
				this.DoOnZoneChange(z);
				Debug.Log("ChangedZone: " + z.zoneName);
			};
			EnviroSkyMgr.instance.OnSeasonChanged += delegate(EnviroSeasons.Seasons season)
			{
				Debug.Log("Season changed");
			};
			EnviroSkyMgr.instance.OnHourPassed += delegate()
			{
				Debug.Log("Hour Passed!");
			};
			EnviroSkyMgr.instance.OnDayPassed += delegate()
			{
				Debug.Log("New Day!");
			};
			EnviroSkyMgr.instance.OnYearPassed += delegate()
			{
				Debug.Log("New Year!");
			};
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x000CC57D File Offset: 0x000CA77D
		private void DoOnWeatherChange(EnviroWeatherPreset type)
		{
			type.Name == "Light Rain";
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x000CC590 File Offset: 0x000CA790
		private void DoOnZoneChange(EnviroZone type)
		{
			type.zoneName == "Swamp";
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x000CC5A3 File Offset: 0x000CA7A3
		public void TestEventsWWeather()
		{
			MonoBehaviour.print("Weather Changed though interface!");
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x000CC5AF File Offset: 0x000CA7AF
		public void TestEventsNight()
		{
			MonoBehaviour.print("Night now!!");
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x000CC5BB File Offset: 0x000CA7BB
		public void TestEventsDay()
		{
			MonoBehaviour.print("Day now!!");
		}
	}
}
