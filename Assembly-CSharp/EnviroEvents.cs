using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000077 RID: 119
public class EnviroEvents : MonoBehaviour
{
	// Token: 0x06000216 RID: 534 RVA: 0x00016E08 File Offset: 0x00015008
	private void Start()
	{
		EnviroSkyMgr.instance.OnHourPassed += delegate()
		{
			this.HourPassed();
		};
		EnviroSkyMgr.instance.OnDayPassed += delegate()
		{
			this.DayPassed();
		};
		EnviroSkyMgr.instance.OnYearPassed += delegate()
		{
			this.YearPassed();
		};
		EnviroSkyMgr.instance.OnWeatherChanged += delegate(EnviroWeatherPreset type)
		{
			this.WeatherChanged();
		};
		EnviroSkyMgr.instance.OnSeasonChanged += delegate(EnviroSeasons.Seasons season)
		{
			this.SeasonsChanged();
		};
		EnviroSkyMgr.instance.OnNightTime += delegate()
		{
			this.NightTime();
		};
		EnviroSkyMgr.instance.OnDayTime += delegate()
		{
			this.DayTime();
		};
		EnviroSkyMgr.instance.OnZoneChanged += delegate(EnviroZone zone)
		{
			this.ZoneChanged();
		};
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00016EC8 File Offset: 0x000150C8
	private void HourPassed()
	{
		this.onHourPassedActions.Invoke();
		if ((EnviroSky.instance.internalHour >= 16f && EnviroSky.instance.internalHour < 24f) || (EnviroSky.instance.internalHour >= 0f && EnviroSky.instance.internalHour < 7f))
		{
			this.lightscript.TurnOn();
		}
		else if (EnviroSky.instance.internalHour >= 7f && EnviroSky.instance.internalHour < 16f)
		{
			this.lightscript.TurnOff();
		}
		this.trashscript.CreateTrash();
		this.tobaccoMgr.UpdatePlants();
		this.bucket1.Ferment();
		this.bucket2.Ferment();
		this.bucket3.Ferment();
		this.junkgrid.NewJunk();
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00002188 File Offset: 0x00000388
	public void CheckThings()
	{
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00016FA0 File Offset: 0x000151A0
	private void DayPassed()
	{
		this.onDayPassedActions.Invoke();
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00016FAD File Offset: 0x000151AD
	private void YearPassed()
	{
		this.onYearPassedActions.Invoke();
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00016FBA File Offset: 0x000151BA
	private void WeatherChanged()
	{
		this.onWeatherChangedActions.Invoke();
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00016FC7 File Offset: 0x000151C7
	private void SeasonsChanged()
	{
		this.onSeasonChangedActions.Invoke();
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00016FD4 File Offset: 0x000151D4
	private void NightTime()
	{
		this.onNightActions.Invoke();
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00016FE1 File Offset: 0x000151E1
	private void DayTime()
	{
		this.onDayActions.Invoke();
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00016FEE File Offset: 0x000151EE
	private void ZoneChanged()
	{
		this.onZoneChangedActions.Invoke();
	}

	// Token: 0x04000594 RID: 1428
	public EnviroEvents.EnviroActionEvent onHourPassedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x04000595 RID: 1429
	public EnviroEvents.EnviroActionEvent onDayPassedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x04000596 RID: 1430
	public EnviroEvents.EnviroActionEvent onYearPassedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x04000597 RID: 1431
	public EnviroEvents.EnviroActionEvent onWeatherChangedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x04000598 RID: 1432
	public EnviroEvents.EnviroActionEvent onSeasonChangedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x04000599 RID: 1433
	public EnviroEvents.EnviroActionEvent onNightActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x0400059A RID: 1434
	public EnviroEvents.EnviroActionEvent onDayActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x0400059B RID: 1435
	public EnviroEvents.EnviroActionEvent onZoneChangedActions = new EnviroEvents.EnviroActionEvent();

	// Token: 0x0400059C RID: 1436
	public streetlights lightscript;

	// Token: 0x0400059D RID: 1437
	public TrashEmitter trashscript;

	// Token: 0x0400059E RID: 1438
	public Bucket bucket1;

	// Token: 0x0400059F RID: 1439
	public Bucket bucket2;

	// Token: 0x040005A0 RID: 1440
	public Bucket bucket3;

	// Token: 0x040005A1 RID: 1441
	public TobaccoManager tobaccoMgr;

	// Token: 0x040005A2 RID: 1442
	public JunkGrid junkgrid;

	// Token: 0x02000378 RID: 888
	[Serializable]
	public class EnviroActionEvent : UnityEvent
	{
	}
}
