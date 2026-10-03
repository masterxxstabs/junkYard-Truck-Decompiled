using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
[AddComponentMenu("Enviro/Utility/Seasons for GameObjects")]
public class EnviroSeasonObjectSwitcher : MonoBehaviour
{
	// Token: 0x06000366 RID: 870 RVA: 0x0001F030 File Offset: 0x0001D230
	private void Start()
	{
		this.SwitchSeasonObject();
		EnviroSkyMgr.instance.OnSeasonChanged += delegate(EnviroSeasons.Seasons season)
		{
			this.SwitchSeasonObject();
		};
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0001F050 File Offset: 0x0001D250
	private void OnEnable()
	{
		if (this.SpringObject == null)
		{
			Debug.LogError("Please assign a spring Object in Inspector!");
			base.enabled = false;
		}
		if (this.SummerObject == null)
		{
			Debug.LogError("Please assign a summer Object in Inspector!");
			base.enabled = false;
		}
		if (this.AutumnObject == null)
		{
			Debug.LogError("Please assign a autumn Object in Inspector!");
			base.enabled = false;
		}
		if (this.WinterObject == null)
		{
			Debug.LogError("Please assign a winter Object in Inspector!");
			base.enabled = false;
		}
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0001F0DC File Offset: 0x0001D2DC
	private void SwitchSeasonObject()
	{
		switch (EnviroSkyMgr.instance.GetCurrentSeason())
		{
		case EnviroSeasons.Seasons.Spring:
			this.SummerObject.SetActive(false);
			this.AutumnObject.SetActive(false);
			this.WinterObject.SetActive(false);
			this.SpringObject.SetActive(true);
			return;
		case EnviroSeasons.Seasons.Summer:
			this.SpringObject.SetActive(false);
			this.AutumnObject.SetActive(false);
			this.WinterObject.SetActive(false);
			this.SummerObject.SetActive(true);
			return;
		case EnviroSeasons.Seasons.Autumn:
			this.SpringObject.SetActive(false);
			this.SummerObject.SetActive(false);
			this.WinterObject.SetActive(false);
			this.AutumnObject.SetActive(true);
			return;
		case EnviroSeasons.Seasons.Winter:
			this.SpringObject.SetActive(false);
			this.SummerObject.SetActive(false);
			this.AutumnObject.SetActive(false);
			this.WinterObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x0400078C RID: 1932
	public GameObject SpringObject;

	// Token: 0x0400078D RID: 1933
	public GameObject SummerObject;

	// Token: 0x0400078E RID: 1934
	public GameObject AutumnObject;

	// Token: 0x0400078F RID: 1935
	public GameObject WinterObject;
}
